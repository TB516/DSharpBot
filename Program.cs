using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace TranscriptMakerBot
{
    static class Program
    {
        public static DiscordClient discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = System.Environment.GetEnvironmentVariable("MegaBot"),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });
        

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            CommandsNextExtension cne = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });
            cne.RegisterCommands<ChatRecorderCommands>();

            await discord.ConnectAsync();
            Console.WriteLine("Connected");
            while(Console.ReadLine() != "stop");
            //await Task.Delay(-1);
        }
    }
}
