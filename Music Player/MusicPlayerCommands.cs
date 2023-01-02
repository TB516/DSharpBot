using DSharpBot.Music_Player;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace DSharpBot.Music_Player
{
    class MusicPlayerCommands : ApplicationCommandModule
    {
        [SlashCommand("Join", "Adds the bot to the call.")]
        public async Task JoinCall(InteractionContext ctx)
        {
            if (MusicPlayer.Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is already in a call!");
                return;
            }
            else
            {
                MusicPlayer.Players.Add(ctx.Guild, new());
                await MusicPlayer.Players[ctx.Guild].ConnectAsync(ctx);
                ctx.CreateResponseAsync("Joined call!");
            }
        }

        [SlashCommand("Disconnect", "Removes the bot from the call.")]
        public async Task Disconnect(InteractionContext ctx)
        {
            if (!MusicPlayer.Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is not in a call!");
            }
            else
            {
                MusicPlayer.Players[ctx.Guild].Disconnect();
                MusicPlayer.Players.Remove(ctx.Guild);
                ctx.CreateResponseAsync("Disconnected from call!");
            }
        }
        [SlashCommand("SongTest", "Song test")]
        public async Task SongTest(InteractionContext ctx, [Option("Song", "Url or search quary")] string input)
        {
            ctx.CreateResponseAsync(new Song(input).ToString()));
        }
    }
}
