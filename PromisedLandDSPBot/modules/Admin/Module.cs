using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Admin;

public class Module
{
    //slash command implementations
    public class Slash : ApplicationCommandModule
    {
    }

    //legacy command implementations
    public class Base : BaseCommandModule
    {

    }
}