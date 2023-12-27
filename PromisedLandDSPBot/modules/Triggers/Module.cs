using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Triggers;

public class Module
{
    private Config _config = Program.Config;
    
    //slash command implementations
    [SlashCommandGroup("trigger", "For setting, modifying and deleting triggers (@role pings)."), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
        
    }
}