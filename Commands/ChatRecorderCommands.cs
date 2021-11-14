using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

class ChatRecorderCommands : BaseCommandModule
{
    [Command("startRecording")]
    public async Task RecordChannel(CommandContext ctx, [RemainingText] string topic)
    {
        if (topic != null)
        {
            ctx.RespondAsync($"Started recording this chat! \nThis recording is about {topic}.");
        }
        else
        {
            ctx.RespondAsync("Started recording this chat! \nThe topic of this recording wasn't specified.");
        }


    }
}
