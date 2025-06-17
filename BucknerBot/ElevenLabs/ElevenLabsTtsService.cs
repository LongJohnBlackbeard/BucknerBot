using ElevenLabs;
using ElevenLabs.TextToSpeech;

namespace BucknerBot.ElevenLabs;

public class ElevenLabsTtsService
{
    private readonly ElevenLabsClient _client;

    public ElevenLabsTtsService(IConfiguration config)
    {
        var apiKey = config["ElevenLabs:Api:Key"];
        _client = new ElevenLabsClient(apiKey);
    }

    public async Task<MemoryStream> GetTts(string voiceApiKey, string text)
    {
        var voice = await _client.VoicesEndpoint.GetVoiceAsync(voiceApiKey);
        Console.WriteLine(voice.ToString());
        var request = new TextToSpeechRequest(voice, text);
        var mp3Ms = new MemoryStream();
        await _client.TextToSpeechEndpoint.TextToSpeechAsync(request, chunk =>
        {
            mp3Ms.Write(chunk.ClipData.ToArray(), 0, chunk.ClipData.Length);
            return Task.CompletedTask;
        });
        mp3Ms.Position = 0;
        return mp3Ms;
    }
}