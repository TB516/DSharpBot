using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;

namespace DSharpBot
{
    static class Program
    {
        public static DiscordClient discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = System.Environment.GetEnvironmentVariable("TBOT516"),
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
            
            //SlashCommandsExtension slash = discord.UseSlashCommands();

            cne.RegisterCommands<ChatRecorderCommands>();
            //slash.RegisterCommands<ChatRecorderCommands>(724358800517365851);

            await discord.ConnectAsync();
            Console.WriteLine("Connected");
            while(Console.ReadLine() != "stop");
            //await Task.Delay(-1);
        }
    }
}
