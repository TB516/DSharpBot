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
        private DiscordChannel channel;
        private string conversationTopic;
        private string path;

        public ChatRecorder(DiscordChannel _channel, string topic)
        {
            channel = _channel;
            conversationTopic = topic;
            path = @$"D:\Programing_projects\C#\TranscriptMakerBot\{channel.Id}_{conversationTopic}.txt";

            //transcriptFile = new StreamWriter(path);
            //transcriptFile.WriteLine($"This conversation is on {conversationTopic}");

            Program.discord.MessageCreated += WriteToTextFile;
        }

        private async Task WriteToTextFile(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Channel == channel && !e.Message.Author.IsBot)
            {
                //sus
                using (StreamWriter transcriptFile = new StreamWriter(path))
                {
                    Console.WriteLine($"<{e.Author.Username}> {e.Message.Content}");
                    transcriptFile.WriteLine($"<{e.Author.Username}> {e.Message.Content}");
                }
            }
        }
    }
}