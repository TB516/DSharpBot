using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace TranscriptMakerBot
{
    class ChatRecorder
    {
        DiscordGuild guild { get; set; }
        public DiscordChannel channel { get; set; }
        private string conversationTopic { get; set; }
        private string path { get; set; }

        public ChatRecorder(DiscordChannel _channel, string topic)
        {
            channel = _channel;
            conversationTopic = topic;

            guild = channel.Guild;
            path = @$"{Directory.GetCurrentDirectory()}\{conversationTopic}_{channel.Id}.txt";

            //transcriptFile = new StreamWriter(path);
            //transcriptFile.WriteLine($"This conversation is on {conversationTopic}");

            Program.discord.MessageCreated += WriteToTextFile;
        }

        private async Task WriteToTextFile(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Channel == channel && !e.Message.Author.IsBot)
            {
                using (StreamWriter transcriptFile = new StreamWriter(path))
                {
                    Console.WriteLine($"<{e.Author.Username}> {e.Message.Content}");
                    transcriptFile.WriteLine($"<{e.Author.Username}> {e.Message.Content}");
                }
            }
        }
    }
}