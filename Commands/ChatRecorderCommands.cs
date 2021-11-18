using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace TranscriptMakerBot
{
    //two
    class ChatRecorderCommands : BaseCommandModule
    {
        [Command("startRecording")]
        public async Task recordChannel(CommandContext ctx, [RemainingText] string topic)
        {
            if (!Program.dictionaryOfRecorders.ContainsKey(ctx.Channel))
            {
                Program.dictionaryOfRecorders.Add(ctx.Channel, new ChatRecorder(ctx.Channel, topic));

                ctx.RespondAsync($"Started recording this chat! \nThis recording is about {topic}.");
            }
            else
            {
                ctx.RespondAsync($"This channel is already being recorded!");
            }
        }

        [Command("endRecording")]
        public async Task endRecording(CommandContext ctx)
        {
            if (Program.dictionaryOfRecorders.ContainsKey(ctx.Channel))
            {
                //program.dictionaryOfRecorders[ctx.Channel].

                Program.dictionaryOfRecorders.Remove(ctx.Channel);

                ctx.RespondAsync("Recording ended!");   
            }
            else
            {
                ctx.RespondAsync("This chat isn't being recorded!");
            }
        }
    }
}