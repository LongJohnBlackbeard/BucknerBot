using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using BucknerBot.ElevenLabs;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Rest;
using Channel = System.Threading.Channels.Channel;

namespace BucknerBot.Discord;

public class TtsQueueService
{
    record QueueItem(ulong GuildId, ulong ChannelId, Voices Voice, string Message);

    private readonly ElevenLabsTtsService _tts;
    private readonly GatewayClient _client;
    private readonly ConcurrentDictionary<ulong, Channel<QueueItem>> _queues =
        new ConcurrentDictionary<ulong, Channel<QueueItem>>();

    public TtsQueueService(ElevenLabsTtsService tts, GatewayClient client)
    {
        _tts = tts;
        _client = client;
    }
    
    public void Enqueue(ulong guildId, ulong channelId, Voices voice, string message)
    {
        var channel = _queues.GetOrAdd(guildId, _ =>
        {
            var ch = Channel.CreateUnbounded<QueueItem>();
            ProcessQueueAsync(ch.Reader); // Fire-and-forget
            return ch; // Return the channel, not the Task
        });
        channel.Writer.TryWrite(new QueueItem(guildId, channelId, voice, message));
    }

private async Task ProcessQueueAsync(ChannelReader<QueueItem> reader)
{
    await foreach (var item in reader.ReadAllAsync())
    {
        VoiceClient? vc = null;
        try
        {
            vc = await _client.JoinVoiceChannelAsync(item.GuildId, item.ChannelId,
                new VoiceClientConfiguration { Logger = new ConsoleLogger() });
            await vc.StartAsync();
            await vc.EnterSpeakingStateAsync(new SpeakingProperties(SpeakingFlags.Microphone));

            // Get Provided Voice
            var voiceList = new VoiceCollection();
            var voice = voiceList.VoiceList.Find(v => v.VoiceEnum == item.Voice);

            if (voice == null)
                throw new InvalidOperationException("Voice not found.");

            // Get Audio
            var audio = await _tts.GetTts(voice.ApiKey!, item.Message);

            // Configure ffmpeg
            var startInfo = new ProcessStartInfo("ffmpeg")
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            startInfo.ArgumentList.Add("-re");
            startInfo.ArgumentList.Add("-i");
            startInfo.ArgumentList.Add("pipe:0");
            startInfo.ArgumentList.Add("-f");
            startInfo.ArgumentList.Add("s16le");
            startInfo.ArgumentList.Add("-ac");
            startInfo.ArgumentList.Add("2");
            startInfo.ArgumentList.Add("-ar");
            startInfo.ArgumentList.Add("48000");
            startInfo.ArgumentList.Add("pipe:1");

            using var ffmpeg = Process.Start(startInfo) ??
                               throw new InvalidOperationException("Failed to start ffmpeg.");

            // Drain stderr to prevent blocking
            _ = Task.Run(async () =>
            {
                while (!ffmpeg.StandardError.EndOfStream)
                    _ = await ffmpeg.StandardError.ReadLineAsync();
            });

            // Pump MP3 into ffmpeg stdin
            _ = Task.Run(async () =>
            {
                await audio.CopyToAsync(ffmpeg.StandardInput.BaseStream);
                ffmpeg.StandardInput.Close();
            });

            // Stream audio to Discord
            var outStream = vc.CreateOutputStream();
            using var opusStream = new OpusEncodeStream(outStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(opusStream);
            await opusStream.FlushAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing TTS: {ex.Message}");
        }
        finally
        {
            if (vc is not null)
            {
                await _client.UpdateVoiceStateAsync(new VoiceStateProperties(item.GuildId, null));
                vc.Dispose();
            }
        }
    }
}
}