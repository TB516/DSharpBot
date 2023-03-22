using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace DSharpBot.Music_Player
{
    /// <summary>
    /// Music player class that represents the discord bot in a voice call in a discord server. 
    /// </summary>
    class MusicPlayer
    {
        /// <summary>
        /// Dictionary storing all the active music players
        /// </summary>
        public static Dictionary<DiscordGuild, MusicPlayer> Players = new();

        #region Fields
        /// <summary>
        /// Voice connection of the bot
        /// </summary>
        private VoiceNextConnection _connection;
        /// <summary>
        /// Song playlist
        /// </summary>
        private List<Song> _playlist;
        /// <summary>
        /// Id of channel that ConnectAsync was called in
        /// </summary>
        private ulong _interactionChannel;
        /// <summary>
        /// Youtube DLP downloader
        /// </summary>
        private YoutubeDL _ytdl;
        /// <summary>
        /// Stream that will store song data
        /// </summary>
        private Stream _pcm;
        /// <summary>
        /// Token source to cancel Pcm transmition
        /// </summary>
        private CancellationTokenSource _pcmCTS;
        #endregion

        #region Properties
        /// <summary>
        /// If the bot is currently playing through a playlist
        /// </summary>
        public bool IsPlaying { get; private set; }
        /// <summary>
        /// Voice connection of the bot
        /// </summary>
        public VoiceNextConnection Connection { get { return _connection; } }
        #endregion

        /// <summary>
        /// Creates new music player using information from context passed in
        /// </summary>
        /// <param name="ctx">Interaction context of the command that creates the object</param>
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
                OverwriteFiles = true,
                IgnoreDownloadErrors = false,
            };

            Program.discord.VoiceStateUpdated += CheckForAutoDisconnectAsync;
        }
        /// <summary>
        /// Connects Discord bot to the voice channel given by interaction context and updates the music player's connection information
        /// </summary>
        /// <param name="ctx">Interaction context to connect the bot and gain information on the connection</param>
        /// <returns>Task representing async process</returns>
        public async Task ConnectAsync(InteractionContext ctx)
        {
            _connection = await ctx.Member.VoiceState.Channel.ConnectAsync();
        }
        /// <summary>
        /// Cancels any stream transmitions if any are occuring, then disconnects the bot from call and removes any leftover song files from the drive
        /// </summary>
        public void Disconnect()
        {
            if (_pcm != null && _pcmCTS != null)
            {
                _pcmCTS.Cancel();
                _pcm.Flush();
                _pcm.Close();
                _playlist.RemoveRange(1, _playlist.Count-1);
            }
            _connection.Disconnect();
            RemoveSongFromDrive();
        }
        /// <summary>
        /// Attempts to add a song to the playlist, fails if song is greater than 1 hour and 5 min
        /// </summary>
        /// <param name="song">Song that will be added to playlist</param>
        /// <returns>True if song was added sucsesfully, False if song was too long</returns>
        public bool AddToPlaylist(Song song)
        {
            if (song.SongLength.TotalMinutes > 65)
            {
                return false;
            }

            _playlist.Add(song);
            return true;
        }
        /// <summary>
        /// Play through all songs in the playlist. Will wait for play song to finish transmiting data before continuing the play process.
        /// </summary>
        public void PlayThroughPlaylist()
        {
            IsPlaying = true;

            RemoveSongFromDrive();

            while (_playlist.Count >= 1)
            {
                DownloadSong(_playlist[0]);

                new DiscordMessageBuilder().WithContent($"Now playing {_playlist[0].SongName}!").SendAsync(_connection.TargetChannel.Guild.Channels[_interactionChannel]);
                
                PlaySong();
                
                _playlist.RemoveAt(0);
                RemoveSongFromDrive();
            }

            IsPlaying = false;
            Console.WriteLine("Finished playlist");
        }
        /// <summary>
        /// Skips song by ending stream data transmition
        /// </summary>
        /// <returns>True of song is skipped, False if it could not be skipped due to missing cancelation token and pcm stream</returns>
        public bool SkipSong()
        {
            if (_pcmCTS == null && _pcm == null)
            {
                return false;
            }

            _pcmCTS.Cancel();
            _pcm.Flush();
            _pcm.Close();
            return true;    
        }
        /// <summary>
        /// Gets data on each song and converts it into a discord embed
        /// </summary>
        /// <returns>Discord embed containing data on songs in playlist</returns>
        public DiscordEmbed GetQueueEmbed()
        {
            DiscordEmbedBuilder queue = new();

            if (_playlist.Count == 0)
            {
                return queue.AddField("Playing now", "Nothing :(");
            }

            queue.AddField("Playing now", _playlist[0].ToString());

            for (int i = 1; i < _playlist.Count; i++)
            {
                queue.AddField($"{i}.", _playlist[i].ToString());
            }
            
            return queue;
        }
        /// <summary>
        /// Removes song based on if title of song passed in matches the title of a song in the playlist. 
        /// Will only remove the first occurence. Cannot remove song that is currently playing.
        /// </summary>
        /// <param name="song">Song to be removed</param>
        /// <returns>If sucsesfull: Sonm that was removed, else: null</returns>
        public Song RemoveSong(Song song)
        {
            for(int i = 1; i < _playlist.Count; i++)
            {
                if (_playlist[i].SongName == song.SongName)
                {
                    return RemoveAt(i);
                }
            }
            return null;
        }
        /// <summary>
        /// Removes song based on position. Position must be in bounds and cannot be 0.
        /// </summary>
        /// <param name="position">Position of song in playlist</param>
        /// <returns>If sucsesfull: Sonm that was removed, else: null</returns>
        public Song RemoveAt(int position)
        {
            if (position >= _playlist.Count || position <= 0)
            {
                return null;
            }

            Song song = _playlist[position];
            _playlist.RemoveAt(position);
            return song;
        }
        /// <summary>
        /// Plays song file with name of the guildId that the bot is in. Will wait untill data is finished transmiting,
        /// or untill process is cancled to finish method execution
        /// </summary>
        private void PlaySong()
        {
            VoiceTransmitSink transmitSink = _connection.GetTransmitSink();
            _pcmCTS = new();

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
                _pcmCTS.Dispose();
            }
            _pcmCTS = null;
            _pcm = null;
        }
        /// <summary>
        /// Downloads the requested song. Will wait untill song is downloaded to finish method execution
        /// </summary>
        /// <param name="song">Song to download</param>
        private void DownloadSong(Song song)
        {
            _ytdl.RunAudioDownload(song.SongUrl.ToString(), AudioConversionFormat.Mp3).GetAwaiter().GetResult();
            Console.WriteLine("Finished downloading " + song.SongName);
        }
        /// <summary>
        /// Tries to remove the current guild song from the drive
        /// </summary>
        private void RemoveSongFromDrive()
        {
            try
            {
                File.Delete(_ytdl.OutputFolder + "\\" + _ytdl.OutputFileTemplate);
                Console.WriteLine("Removed song file.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("There was no song file found!");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Songs folder was not found!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured deleting the file: {e.Message}");
            }
        }
        /// <summary>
        /// Event method that will check to see if bot is alone in call and disconnect if it is.
        /// </summary>
        /// <param name="client">Discord client</param>
        /// <param name="e">Voice update event arguments</param>
        /// <returns></returns>
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
