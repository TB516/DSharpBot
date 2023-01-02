using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Emzi0767.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static DSharpBot.Music_Player.MusicPlayer;

namespace DSharpBot.Music_Player
{
    class MusicPlayerCommands : ApplicationCommandModule
    {
        [SlashCommand("Join", "Adds the bot to the call.")]
        public async Task JoinCall(InteractionContext ctx)
        {
            if (Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is already in a call!");
                return;
            }
            else if (ctx.Member.VoiceState == null)
            {
                ctx.CreateResponseAsync("You must be in a voice channel to have the bot join your call!");
                return;
            }

            Players.Add(ctx.Guild, new());
            await Players[ctx.Guild].ConnectAsync(ctx);

            ctx.CreateResponseAsync("Joined call!");
        }

        [SlashCommand("Disconnect", "Removes the bot from the call.")]
        public async Task Disconnect(InteractionContext ctx)
        {
            if (!Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is not in a call!");
                return;
            }
            else if(ctx.Member.VoiceState == null ||
                ctx.Member.VoiceState.Channel != Players[ctx.Guild].Connection.TargetChannel)
            {
                ctx.CreateResponseAsync("You cannot disconnect the bot if you are not in the same call as the bot!");
                return;
            }

            Players[ctx.Guild].Disconnect();
            Players.Remove(ctx.Guild);

            ctx.CreateResponseAsync("Disconnected from call!");
        }

        [SlashCommand("SongTest", "Song test")]
        public async Task SongTest(InteractionContext ctx, [Option("Song", "Url or search quary")] string input)
        {
            ctx.CreateResponseAsync(new Song(input).ToString());
        }
        
        [SlashCommand("Play", "Plays a song")]
        public async Task PlaySong(InteractionContext ctx, [Option("Song", "Url or search quary")] string input)
        {
            if (!Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is not in a call!");
            }
            else if (ctx.Member.VoiceState == null ||
                ctx.Member.VoiceState.Channel != Players[ctx.Guild].Connection.TargetChannel)
            {
                ctx.CreateResponseAsync("You must be in the same call as the bot in order to play a song through it!");
                return;
            }

            Song song = new(input);

            if (!Players[ctx.Guild].AddToPlaylist(song))
            {
                ctx.CreateResponseAsync($"Couldn't add {song.SongName} to the playlist because it was too long!");
                return;
            }

            ctx.CreateResponseAsync($"Added {song} to the playlist!");

            if (!Players[ctx.Guild].IsPlaying)
            {
                Players[ctx.Guild].PlayThroughPlaylist(ctx);
            }
        }
    }
}
