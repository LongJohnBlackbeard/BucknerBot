namespace BucknerBot.Discord;

public class VoiceCollection
{
    public List<Voice> VoiceList { get; set; } =
    [
        new Voice
        {
            VoiceEnum = Voices.Movie,
            ApiName = "David - Epic Movie Trailer",
            ApiKey = "FF7KdobWPaiR0vkcALHF"
        },
        new Voice
        {
            VoiceEnum = Voices.HotChick,
            ApiName = "Sexy Female Villain Voice",
            ApiKey = "eVItLK1UvXctxuaRV2Oq"
        },
        new Voice
        {
            VoiceEnum = Voices.Cowboy,
            ApiName = "Austin - Good ol' Texas boy",
            ApiKey = "Bj9UqZbhQsanLzgalpEG"
        },
        new Voice
        {
            VoiceEnum = Voices.Sassy,
            ApiName = "Sassy Aerisita",
            ApiKey = "03vEurziQfq3V8WZhQvn"
        },
        new Voice
        {
            VoiceEnum = Voices.FatChick,
            ApiName = "Lutz Laugh - Chuckling and Giggly",
            ApiKey = "9yzdeviXkFddZ4Oz8Mok"
        },
        new Voice
        {
            VoiceEnum = Voices.Demon,
            ApiName = "Demon Monster",
            ApiKey = "vfaqCOvlrKi4Zp7C2IAm"
        },
        new Voice
        {
            VoiceEnum = Voices.British,
            ApiName = "Russel - Dramatic British TV",
            ApiKey = "NYC9WEgkq1u4jiqBseQ9",
        },
        new Voice
        {
            VoiceEnum = Voices.Black,
            ApiName = "Young Jamal",
            ApiKey = "6OzrBCQf8cjERkYgzSg8"
        }

    ];
}