using DSharpPlus.SlashCommands;
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

            Players.Add(ctx.Guild, new(ctx));
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

        [SlashCommand("Play", "Plays a song")]
        public async Task PlaySong(InteractionContext ctx, [Option("Song", "Url or search quary")] string input)
        {
            if (!Players.ContainsKey(ctx.Guild))
            {
                await JoinCall(ctx);
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
                Players[ctx.Guild].PlayThroughPlaylist();
            }
        }

        [SlashCommand("Skip", "Skips the current song")]
        public async Task SkipSong(InteractionContext ctx)
        {
            if (!Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is not in a call!");
                return;
            }
            else if (ctx.Member.VoiceState == null ||
                ctx.Member.VoiceState.Channel != Players[ctx.Guild].Connection.TargetChannel)
            {
                ctx.CreateResponseAsync("You must be in the same call as the bot in order to skip a song!");
                return;
            }
            else if (!Players[ctx.Guild].IsPlaying)
            {
                ctx.CreateResponseAsync("The bot is not currently playing a song!");
                return;
            }
            else if (!Players[ctx.Guild].SkipSong())
            {
                ctx.CreateResponseAsync("The bot hasn't started playing the next song yet! Try again when the song has stated.");
                return;
            }

            ctx.CreateResponseAsync("Skiped song!");
        }

        [SlashCommand("Remove", "Removes a song from the playlist")]
        public async Task RemoveSong(InteractionContext ctx, [Option("Song", "Title of the song")] string songName)
        {
            if (!Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is not in a call!");
                return;
            }
            else if (ctx.Member.VoiceState == null ||
                ctx.Member.VoiceState.Channel != Players[ctx.Guild].Connection.TargetChannel)
            {
                ctx.CreateResponseAsync("You must be in the same call as the bot in order to remove a song from the playlist!");
                return;
            }

            Song song = Players[ctx.Guild].RemoveSong(new(songName));

            if (song == null)
            {
                ctx.CreateResponseAsync("The song couldn't be found! Check to make sure the names match up properly.");
                return;
            }

            ctx.CreateResponseAsync($"Removed {song.SongName} from the playlist!");
        }

        [SlashCommand("RemoveAt", "Removes song at requested position")]
        public async Task RemoveAt(InteractionContext ctx, [Option("Position", "Position of song to remove")] long position)
        {
            if (!Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is not in a call!");
                return;
            }
            else if (ctx.Member.VoiceState == null ||
                ctx.Member.VoiceState.Channel != Players[ctx.Guild].Connection.TargetChannel)
            {
                ctx.CreateResponseAsync("You must be in the same call as the bot in order to remove a song from the playlist!");
                return;
            }

            Song song = Players[ctx.Guild].RemoveAt((int)position);

            if(song == null)
            {
                ctx.CreateResponseAsync($"Invalid position, please enter a valid position.");
                return;
            }

            ctx.CreateResponseAsync($"Removed {song.SongName} from the playlist");
        }

        [SlashCommand("Queue", "Displays the songs in song queue")]
        public async Task SendQueue(InteractionContext ctx)
        {
            if (!Players.ContainsKey(ctx.Guild))
            {
                ctx.CreateResponseAsync("The bot is not in a call!");
                return;
            }

            ctx.CreateResponseAsync(Players[ctx.Guild].GetQueueEmbed());
        }
    }
}
