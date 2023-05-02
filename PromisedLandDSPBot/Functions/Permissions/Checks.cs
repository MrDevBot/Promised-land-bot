using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Functions.Permissions;

public class Checks
{
    public static bool RejectDM(InteractionContext ctx)
    {
        if (!ctx.Channel.IsPrivate) return false;
        ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(":warning: This command cannot be used in a DM channel."));
        return true;

    }
    
    public static bool RejectGuild(InteractionContext ctx)
    {
        if (ctx.Channel.IsPrivate) return false;
        ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(":warning: This command cannot be used in a guild."));
        return true;

    }
    
    public static bool RejectNonDeveloper(InteractionContext ctx)
    {
        if (ctx.Client.CurrentApplication.Owners.Contains(ctx.User)) return false;
        ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(":warning: This command can only be used by the bot developer."));
        return true;

    }
    
}