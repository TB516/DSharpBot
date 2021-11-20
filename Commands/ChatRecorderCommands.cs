using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace TranscriptMakerBot
{
    class ChatRecorderCommands : BaseCommandModule
    {
        [Command("startRec")]
        public async Task RecordChannel(CommandContext ctx, [RemainingText] string conversationTopic)
        {
            for (int i = 0; i < Program.chatRecorders.Count; i++)
            {
                if(Program.chatRecorders[i].conversationTopic == conversationTopic)
                {
                    ctx.RespondAsync($"Recording with the topic \"{conversationTopic}\" is already running");
                    return;
                }    
            }
            Program.chatRecorders.Add(new ChatRecorder(ctx.Channel, conversationTopic));
            ctx.RespondAsync($"Started recording this chat! \nThis recording is about {conversationTopic}.");
        }

        [Command("stopRec")]
        public async Task EndRecording(CommandContext ctx, [RemainingText] string conversationTopic)
        {
            for(int i = 0; i < Program.chatRecorders.Count; i++)
            {
                if(
                    Program.chatRecorders[i].channel == ctx.Channel && 
                    Program.chatRecorders[i].conversationTopic == conversationTopic
                )
                {
                    Program.chatRecorders[i].Dispose();
                    Program.chatRecorders.RemoveAt(i);
                    ctx.RespondAsync($"Recording about \"{conversationTopic}\" ended!");
                    return;
                }
            }
        }
    }
}