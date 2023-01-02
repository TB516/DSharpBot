using DSharpPlus;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace DSharpBot.Chat_Recorder
{
    class ChatRecorderCommands : ApplicationCommandModule
    {
        [SlashCommand("startRec", "Starts recording the channel it is called in.")]
        public async Task RecordChannel(InteractionContext ctx, [Option("Topic", "The topic of the conversation being recorded.")] string conversationTopic)
        {   
            for (int i = 0; i < ChatRecorder.ChatRecorders.Count; i++)
            {
                if(ChatRecorder.ChatRecorders[i].ConversationTopic == conversationTopic)
                {
                    await ctx.CreateResponseAsync($"Recording with the topic \"{conversationTopic}\" is already running");
                    return;
                }    
            }
            ChatRecorder.ChatRecorders.Add(new ChatRecorder(ctx.Channel, conversationTopic));
            await ctx.CreateResponseAsync($"Started recording this chat! \nThis recording is about \"{conversationTopic}\".");
        }
        
        [SlashCommand("stopRec", "Ends the recording of the specified topic in the channel called in.")]
        public async Task EndRecording(InteractionContext ctx, [Option("Topic", "The topic of the conversation being recorded.")] string conversationTopic)
        {
            for(int i = 0; i < ChatRecorder.ChatRecorders.Count; i++)
            {
                if(ChatRecorder.ChatRecorders[i].Channel == ctx.Channel && ChatRecorder.ChatRecorders[i].ConversationTopic == conversationTopic)
                {
                    ChatRecorder.ChatRecorders[i].Dispose();
                    ChatRecorder.ChatRecorders.RemoveAt(i);
                    await ctx.CreateResponseAsync($"Recording about \"{conversationTopic}\" ended!");
                    return;
                }
            }
        }
    }
}