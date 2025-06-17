using ElevenLabs;
using ElevenLabs.TextToSpeech;
using Microsoft.Extensions.Logging;

namespace BucknerBot.ElevenLabs;

public class ElevenLabsTtsService
{
    private readonly ElevenLabsClient _client;
    private readonly ILogger<ElevenLabsTtsService> _logger;

    public ElevenLabsTtsService(IConfiguration config, ILogger<ElevenLabsTtsService> logger)
    {
        _logger = logger;
        var apiKey = config["ElevenLabs:Api:Key"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("ElevenLabs API key is missing from configuration.");
        }
        _client = new ElevenLabsClient(apiKey);
    }

    public async Task<MemoryStream> GetTts(string voiceApiKey, string text)
    {
        var voice = await _client.VoicesEndpoint.GetVoiceAsync(voiceApiKey);
        _logger.LogInformation("Fetched voice: {Voice}", voice.ToString());
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