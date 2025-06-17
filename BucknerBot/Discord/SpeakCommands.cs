using System.Diagnostics;
using System.Net.WebSockets;
using BucknerBot.ElevenLabs;
using NetCord;
using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Rest;

namespace BucknerBot.Discord;


public class SpeakCommands(TtsQueueService queues) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("speak", "Have the bot join your voice channel and speak your text.", Contexts = [InteractionContextType.Guild])]
    public async Task Voice(
        [SlashCommandParameter(
            Name        = "voice",
            Description = "Which ElevenLabs voice to use.")]
        Voices voiceEnum,

        [SlashCommandParameter(
            Name        = "message",
            Description = "Text you want the bot to say.")]
        string message)
    {
        
        var guild = Context.Guild;

        // Ensure Slash user is connected to a voice channel
        if (!guild.VoiceStates.TryGetValue(Context.User.Id, out var vs) || vs.ChannelId is null)
        {
            // immediate error reply
            var failProps = new InteractionMessageProperties {
                Content = "❌ You must be in a voice channel first.",
                Flags   = MessageFlags.Ephemeral
            };
            await RespondAsync(InteractionCallback.Message(failProps));                   // :contentReference[oaicite:0]{index=0}
            return;
        }
        
        await RespondAsync(
            InteractionCallback.DeferredMessage(
                MessageFlags.Ephemeral));  // Defer the response (so the user knows the bot is processing the command)
        
        queues.Enqueue(
            guild.Id, 
            vs.ChannelId.Value, 
            voiceEnum, 
            message);  // Enqueue the message to be spoken
        
        // Send a follow-up message to confirm the request
        await FollowupAsync(new InteractionMessageProperties
        {
            Content = "✅ Your TTS request has been added to the queue!",
            Flags = MessageFlags.Ephemeral
        });
    }
}
