using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace TranscriptMakerBot
{
    static class Program
    {
        public static DiscordClient discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = "ODg3ODA5MjM3MDA5NDQwODAw.YUJiyA.w - oJYAb9TVhGNzPmwrCEsY5eqmo",
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });
        public static List<ChatRecorder> chatRecorders = new List<ChatRecorder>();

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] {"!"}
            }).RegisterCommands<ChatRecorderCommands>();

            await discord.ConnectAsync();
            Console.WriteLine("Connected");
            while(Console.ReadLine() != "stop");
            //await Task.Delay(-1);
        }
    }
}
