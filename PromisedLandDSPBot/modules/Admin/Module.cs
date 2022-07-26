using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Admin;

public class Module
{
    //slash command implementations
    [SlashCommandGroup("admin", "Methods to assist server moderators."), RequireUserPermissions(Permissions.Administrator), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
        
    }

    //legacy command implementations
    [Group("admin")]
    public class Base : BaseCommandModule
    {
        [Command("adminhelp")]
        public async Task AdminHelp(CommandContext ctx) // ADD command parameters as if they are parameters in code. 
        {
            await ctx.RespondAsync($"YAY ADMIN HELP");
        }
    }
}