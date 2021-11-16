using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using TranscriptMakerBot;
class ChatRecorderCommands : BaseCommandModule
{
    [Command("startRecording")]
    public async Task RecordChannel(CommandContext ctx, [RemainingText] string topic)
    {
        ctx.RespondAsync($"Started recording this chat! \nThis recording is about {topic}.");

        Program.channelRecorderList


    }
}
