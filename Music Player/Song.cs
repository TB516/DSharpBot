using DSharpPlus.Entities;
using System;
using System.Linq;
using YTSearch.NET;

namespace DSharpBot.Music_Player
{ 
    internal class Song
    {
        /// <summary>
        /// Name of the song
        /// </summary>
        public string SongName { get; private set; }
        /// <summary>
        /// Url of the song
        /// </summary>
        public Uri SongUrl { get; private set; }
        /// <summary>
        /// Length of the song
        /// </summary>
        public TimeSpan SongLength { get; private set; }

        /// <summary>
        /// Takes either a search term or a url and gets all values based on either one
        /// </summary>
        /// <param name="input">Search term or Url in string form</param>
        public Song(string input)
        {
            if (Uri.IsWellFormedUriString(input, UriKind.Absolute) && input.Contains("youtube.com"))
            {
                SongUrl = new(input);
            }
            else
            {
                SongUrl = GetUrl(input);
            }

            SongName = GetTitle(SongUrl);
            SongLength = GetSongLength(SongUrl);
        }
        
        /// <summary>
        /// Takes a search term and returns a URI of the first video
        /// </summary>
        /// <param name="searchTerm">Term to search youtube for</param>
        /// <returns>Url in form of URI</returns>
        private static Uri GetUrl(string searchTerm)
        {
            return new(new YouTubeSearchClient().SearchYoutubeAsync(searchTerm).Result.Results.ToArray()[0].Url);
        }
        /// <summary>
        /// Takes a URI that holds the url and returns the title of the youtube video
        /// </summary>
        /// <param name="url">Url in URI form</param>
        /// <returns>String of video title</returns>
        private static string GetTitle(Uri url)
        {
            return new YouTubeSearchClient().GetVideoMetadataAsync(url).Result.Result.Title;
        }
        /// <summary>
        /// Takes URL in URI form and gets length of video
        /// </summary>
        /// <param name="url">URL in URI form</param>
        /// <returns>Video length as a TimeSpan</returns>
        private static TimeSpan GetSongLength(Uri url)
        {
            return new YouTubeSearchClient().GetVideoMetadataAsync(url).Result.Result.Length;
        }
        /// <summary>
        /// Name of the song and length of the song
        /// </summary>
        /// <returns>String storing data about the song</returns>
        public override string ToString()
        {
            return $"{SongName} ({SongLength})";
        }
    }
}
