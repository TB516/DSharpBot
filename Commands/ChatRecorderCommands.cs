using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DSharpBot
{
    class ChatRecorderCommands : BaseCommandModule
    {
        [Command("startRec")]
        public async Task RecordChannel(CommandContext ctx, [RemainingText] string conversationTopic)
        {
            for (int i = 0; i < ChatRecorder.chatRecorders.Count; i++)
            {
                if(ChatRecorder.chatRecorders[i].conversationTopic == conversationTopic)
                {
                    ctx.RespondAsync($"Recording with the topic \"{conversationTopic}\" is already running");
                    return;
                }    
            }
            ChatRecorder.chatRecorders.Add(new ChatRecorder(ctx.Channel, conversationTopic));
            ctx.RespondAsync($"Started recording this chat! \nThis recording is about \"{conversationTopic}\".");
        }

        [Command("stopRec")]
        public async Task EndRecording(CommandContext ctx, [RemainingText] string conversationTopic)
        {
            for(int i = 0; i < ChatRecorder.chatRecorders.Count; i++)
            {
                if(ChatRecorder.chatRecorders[i].channel == ctx.Channel && ChatRecorder.chatRecorders[i].conversationTopic == conversationTopic)
                {
                    ChatRecorder.chatRecorders[i].Dispose();
                    ChatRecorder.chatRecorders.RemoveAt(i);
                    ctx.RespondAsync($"Recording about \"{conversationTopic}\" ended!");
                    return;
                }
            }
        }
    }
}