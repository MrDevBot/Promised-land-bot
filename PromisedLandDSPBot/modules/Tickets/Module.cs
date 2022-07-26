using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Tickets;

public class Module
{
    //slash command implementations
    [SlashCommandGroup("ticket", "ticket tool functionality group."), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
    }

    //legacy command implementations
    [Group("ticket")]
    public class Base : BaseCommandModule
    {

    }
}