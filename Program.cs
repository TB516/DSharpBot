using DSharpPlus;
using System;
using System.Threading.Tasks;

namespace TranscriptMakerBot{
    class Program{
        static void Main(string[] args){
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync(){
            var discord = new DiscordClient(new DiscordConfiguration(){
                Token = "ODg3ODA5MjM3MDA5NDQwODAw.YUJiyA.w - oJYAb9TVhGNzPmwrCEsY5eqmo",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            discord.MessageCreated += async (s, e) =>{
                if(e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync($"Your ping is {discord.Ping.ToString()}.");
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Discord_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e){
            throw new NotImplementedException();
        }
    }
}
