using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace DSharpBot
{
    class ChatRecorder : IDisposable
    {
        public static List<ChatRecorder> chatRecorders = new List<ChatRecorder>();
        public DiscordChannel channel { get; set; }
        public string conversationTopic { get; set; }
        private string transcriptFilePath { get; set; }
        private StreamWriter transcriptFileWriter { get; set; }

        public ChatRecorder(DiscordChannel _channel, string _conversationTopic)
        {
            channel = _channel;
            conversationTopic = _conversationTopic;

            Directory.CreateDirectory(@$"{Directory.GetCurrentDirectory()}\Records");
            transcriptFilePath = @$"{Directory.GetCurrentDirectory()}\Records\{conversationTopic}_{channel.Id}.txt";
            transcriptFileWriter = new StreamWriter(transcriptFilePath);
            Program.discord.MessageCreated += WriteToTextFile;
        }
        private async Task WriteToTextFile(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Channel == channel && !e.Message.Author.IsBot)
            {
                DiscordMember messageAuthorMember = await e.Guild.GetMemberAsync(e.Author.Id);
                transcriptFileWriter.WriteLine($"<{messageAuthorMember.DisplayName}> {e.Message.Content}");
            }
        }
        public void Dispose()
        {
            Program.discord.MessageCreated -= WriteToTextFile;
            transcriptFileWriter.Flush();
            transcriptFileWriter.Close();
            transcriptFileWriter.Dispose();
        }
    }
}