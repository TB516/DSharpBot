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
        public static Dictionary<DiscordChannel, ChatRecorder> dictionaryOfRecorders = new Dictionary<DiscordChannel, ChatRecorder>();

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] {"!"}
            });

            commands.RegisterCommands<ChatRecorderCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
