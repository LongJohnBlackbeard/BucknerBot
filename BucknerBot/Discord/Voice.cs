using ElevenLabs.Voices;

namespace BucknerBot.Discord;

public class Voice
{
    public Voices VoiceEnum { get; set; }
    public string? ApiName { get; set; }
    public string? ApiKey { get; set; }
}