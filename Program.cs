using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;

namespace TranscriptMakerBot
{
    static class Program
    {   
        public static Dictionary<string, chatRecorder> channelRecorderList = new Dictionary<string, chatRecorder>();

        static void Main(string[] args)
        {
            Console.WriteLine("Running bot.");
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            DiscordClient discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "ODg3ODA5MjM3MDA5NDQwODAw.YUJiyA.w - oJYAb9TVhGNzPmwrCEsY5eqmo",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });
            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] {"!"}
            });

            commands.RegisterCommands<ChatRecorderCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
