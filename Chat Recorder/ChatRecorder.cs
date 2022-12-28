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
        public static List<ChatRecorder> ChatRecorders = new List<ChatRecorder>();
        public DiscordChannel Channel { get; set; }
        public string ConversationTopic { get; set; }
        private string TranscriptFilePath { get; set; }
        private StreamWriter TranscriptFileWriter { get; set; }

        public ChatRecorder(DiscordChannel _channel, string _conversationTopic)
        {
            Channel = _channel;
            ConversationTopic = _conversationTopic;

            Directory.CreateDirectory(@$"{Directory.GetCurrentDirectory()}\Records");
            TranscriptFilePath = @$"{Directory.GetCurrentDirectory()}\Records\{ConversationTopic}_{Channel.Id}.txt";
            TranscriptFileWriter = new StreamWriter(TranscriptFilePath);
            Program.discord.MessageCreated += WriteToTextFile;
        }
        private async Task WriteToTextFile(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Channel != Channel || e.Message.Author.IsBot) return;

            DiscordMember messageAuthorMember = await e.Guild.GetMemberAsync(e.Author.Id);
            TranscriptFileWriter.WriteLine($"<{messageAuthorMember.DisplayName}> {e.Message.Content}");
        }
        public void Dispose()
        {
            Program.discord.MessageCreated -= WriteToTextFile;
            TranscriptFileWriter.Flush();
            TranscriptFileWriter.Close();
            TranscriptFileWriter.Dispose();
        }
    }
}