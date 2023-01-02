using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using System;
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
        private ulong _interactionChannel;

        public bool IsPlaying { get; private set; }
        public VoiceNextConnection Connection { get { return _connection; } }
        
        public MusicPlayer()
        {
            _playlist = new();
            Program.discord.VoiceStateUpdated += CheckForAutoDisconnectAsync;
        }
        public async Task ConnectAsync(InteractionContext ctx)
        {
            await ctx.Member.VoiceState.Channel.ConnectAsync();

            _connection = ctx.Client.GetVoiceNext().GetConnection(ctx.Guild);
            _interactionChannel = ctx.Channel.Id;
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
        public void PlayThroughPlaylist(InteractionContext ctx)
        {
            IsPlaying = true;

            while (_playlist.Count >= 1)
            {

                PlaySong(_playlist[0]);
                _playlist.RemoveAt(0);
            }

            IsPlaying = false;
        }
        private void PlaySong(Song song)
        {

        }
        private void DownloadSong(Song song)
        {

        }
        private async Task CheckForAutoDisconnectAsync(DiscordClient client, VoiceStateUpdateEventArgs e)
        {
            if(e.User != Program.discord.CurrentUser && _connection.TargetChannel.Users.Count == 1)
            {
                Program.discord.VoiceStateUpdated -= CheckForAutoDisconnectAsync;

                Disconnect();

                await new DiscordMessageBuilder().WithContent("Bot disconnected due to lack of users in call").SendAsync(e.Guild.Channels[_interactionChannel]);

                Players.Remove(e.Guild);
            }
        }
    }
}
