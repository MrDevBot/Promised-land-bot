namespace PromisedLandDSPBot.modules;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
// put useful stuff here

public static class AssistantModule
{
    public static DiscordEmbedBuilder GetEmbedBuilder()
    {
        return new DiscordEmbedBuilder();
    }
}

public class RequireUserIdAttribute : SlashCheckBaseAttribute
{
    private ulong[] UserIds;

    public RequireUserIdAttribute(ulong[] userId)
    {
        this.UserIds = userId;
    }

    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        return UserIds.Any(id => ctx.User.Id == id);
    }
}

/// <summary>
/// Intended for combining in a logical OR fashion, two other Attribute checks.
/// </summary>
public class RequireOrAttribute : SlashCheckBaseAttribute
{
    private SlashCheckBaseAttribute[] _checksToDo;
    public RequireOrAttribute(SlashCheckBaseAttribute[] checks)
    {
        _checksToDo = checks;
    }

    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        bool success = false;
        foreach (SlashCheckBaseAttribute scba in _checksToDo)
        {
            success |= await scba.ExecuteChecksAsync(ctx);
        }
        return success;
    }
}
