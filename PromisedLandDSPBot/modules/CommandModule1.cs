using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace PromisedLandDSPBot.modules;

/// <summary>
/// Base Command system. This is the kind that is written into chat rather than slash commands, thats a different system.
/// </summary>
public class CommandModule1 : BaseCommandModule
{
    [Command("greet")]
    public async Task GreetCommand(CommandContext ctx) // ADD command parameters as if they are parameters in code. 
    {
        await ctx.RespondAsync("Greetings! Thank you for executing me!");
    }
}