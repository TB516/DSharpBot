using DSharpBot.Music_Player;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DSharpBot.Music_Player
{
    class MusicPlayer
    {
        public static Dictionary<DiscordGuild, MusicPlayer> Players = new();

        private VoiceNextConnection _connection;
        private List<Song> _playlist;

        public bool IsPlaying { get; private set; }

        public async Task ConnectAsync(InteractionContext ctx)
        {
            await ctx.Member.VoiceState.Channel.ConnectAsync();

            _connection = ctx.Client.GetVoiceNext().GetConnection(ctx.Guild);
        }
        public void Disconnect()
        {
            _connection.Disconnect();
        }
        public bool AddToPlaylist(Song song)
        {
            if (song.SongLength.TotalMinutes <= 65)
            {
                _playlist.Add(song);
                return true;
            }
            return false;
        }
        public void PlayThroughPlaylist()
        {
            IsPlaying = true;

            while (_playlist.Count >= 1)
            {
                
            }

            IsPlaying = false;
        }
        
    }
}
