using System;
using System.IO;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;

namespace TranscriptMakerBot
{
    class ChatRecorder
    {
        private DiscordChannel channel;
        private string conversationTopic;
        private StreamWriter transcriptFile;
        private string path;

        public ChatRecorder(DiscordChannel _channel, string topic)
        {
            channel = _channel;
            conversationTopic = topic;
            path = @$"D:\Programing_projects\C#\TranscriptMakerBot\{channel.Id}_{conversationTopic}.txt";
            transcriptFile = File.CreateText(path);
        }
    }
}