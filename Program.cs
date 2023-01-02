using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;

namespace DSharpBot
{
    static class Program
    {
        public static DiscordClient discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = System.Environment.GetEnvironmentVariable("BotKey"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });
        
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            discord.UseVoiceNext();
            SlashCommandsExtension slash = discord.UseSlashCommands();

            slash.RegisterCommands<Chat_Recorder.ChatRecorderCommands>();
            slash.RegisterCommands<Music_Player.MusicPlayerCommands>(724358800517365851);
            
            await discord.ConnectAsync();
            Console.WriteLine("Connected");
            while(Console.ReadLine() != "stop");
        }
    }
}
