using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;

namespace DSharpBot
{
    static class Program
    {
        public static DiscordClient discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = File.ReadAllLines("BotKey.txt")[0],
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
            slash.RegisterCommands<Music_Player.MusicPlayerCommands>();
            
            await discord.ConnectAsync();
            Console.WriteLine("Connected");
            while(Console.ReadLine() != "stop");
        }
    }
}
