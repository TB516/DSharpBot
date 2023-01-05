using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace DSharpBot.Music_Player
{
    class MusicPlayer
    {
        public static Dictionary<DiscordGuild, MusicPlayer> Players = new();

        private VoiceNextConnection _connection;
        private List<Song> _playlist;
        private ulong _interactionChannel;
        private YoutubeDL _ytdl;
        private Stream _pcm;
        private CancellationTokenSource _pcmCTS;

        public bool IsPlaying { get; private set; }
        public VoiceNextConnection Connection { get { return _connection; } }
        
        public MusicPlayer(InteractionContext ctx)
        {
            _interactionChannel = ctx.Channel.Id;

            _playlist = new();
            _ytdl = new()
            {
                FFmpegPath = Directory.GetCurrentDirectory() + "\\ffmpeg.exe",
                YoutubeDLPath = Directory.GetCurrentDirectory() + "\\yt-dlp.exe",
                OutputFolder = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Songs").ToString(),
                OutputFileTemplate = ctx.Guild.Id.ToString() + ".mp3",
                OverwriteFiles = true
            };

            _pcmCTS = new();

            Program.discord.VoiceStateUpdated += CheckForAutoDisconnectAsync;
        }
        public async Task ConnectAsync(InteractionContext ctx)
        {
            await ctx.Member.VoiceState.Channel.ConnectAsync();

            _connection = ctx.Client.GetVoiceNext().GetConnection(ctx.Guild);
        }
        public void Disconnect()
        {
            if (_pcm != null)
            {
                _pcmCTS.Cancel();
                _pcm.Flush();
                _pcm.Close();
                _pcm.Dispose();
            }
            _connection.Disconnect();
            RemoveSongFromDrive();
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

            RemoveSongFromDrive();

            do
            {
                DownloadSong(_playlist[0]);

                new DiscordMessageBuilder().WithContent($"Now playing {_playlist[0].SongName}!").SendAsync(_connection.TargetChannel.Guild.Channels[_interactionChannel]);

                PlaySong();

                _playlist.RemoveAt(0);
                RemoveSongFromDrive();
            } while (_playlist.Count >= 1);

            IsPlaying = false;
            Console.WriteLine("Finished playlist");
        }
        public void SkipSong()
        {
            _pcmCTS.Cancel();
            _pcm.Flush();
            _pcm.Close();
        }
        private void PlaySong()
        {
            VoiceTransmitSink transmitSink = _connection.GetTransmitSink();

            //Converts the mp3 file to bite stream
            using (_pcm = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{_ytdl.OutputFolder + "\\" + _ytdl.OutputFileTemplate}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false,
            }).StandardOutput.BaseStream)
            {
                TaskAwaiter awaiter = _pcm.CopyToAsync(transmitSink, cancellationToken: _pcmCTS.Token).GetAwaiter();

                // Wait for the stream to end
                while (!awaiter.IsCompleted)
                {
                    Thread.Sleep(100);
                }
            }
        }
        private void DownloadSong(Song song)
        {
            _ytdl.RunAudioDownload(song.SongUrl.ToString(), AudioConversionFormat.Mp3).GetAwaiter().GetResult();
            Console.WriteLine("Finished downloading " + song.SongName);
        }
        private void RemoveSongFromDrive()
        {
            try
            {
                File.Delete(_ytdl.OutputFolder + "\\" + _ytdl.OutputFileTemplate);
                Console.WriteLine("Removed song file.");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("There was no song file found!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured deleting the file: {e.Message}");
            }
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
