using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Reactions;

public class Module
{
    //slash command implementations
    [SlashCommandGroup("reactionrole", "For making, editing and deleting reaction role structures."), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
    }
}