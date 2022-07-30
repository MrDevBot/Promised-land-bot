using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;
using PromisedLandDSPBot.Logging;

namespace PromisedLandDSPBot;

public class Events
{
    internal static async Task ClientOnModalSubmitted(DiscordClient sender, ModalSubmitEventArgs e)
    {
        // Check modal id - if "suggestion-XXXX", delegate to a handler.
        //throw new NotImplementedException();
        
        var guildId = e.Interaction.GuildId ?? 0;
        Logger.Log(Logger.Type.Command, $"M>S Modal Submitted by User | ", e.Interaction.ChannelId, guildId);
        await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Thanks for Submitting!"));
        
    }

    internal static async Task CommandsOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        Console.WriteLine($"\nNormal Command Error: \n{e.Exception.ToString()}\n\n and {e.Context.Command} was the culprit.");
        Logger.Log(Logger.Type.Debug, $"CN>E CommandNext Error: {e.Context.Command} | {e.Exception.Message} | {e.Exception.StackTrace?.ToString()}", e.Context);
        await OnErrorChannelReport(e.Context.Member, e.Context.Channel, e.Exception);
        //throw new NotImplementedException();
    }

    internal static async Task SlashOnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        Console.WriteLine($"Slash Command Error: \n{e.Exception.ToString()}\n\n and {e.Context.CommandName} was the culprit.");
        Logger.Log(Logger.Type.Debug, $"SC>E SlashCommand Error: {e.Context.CommandName} | {e.Exception.Message} | {e.Exception.StackTrace?.ToString()} | InteractionId:{e.Context.InteractionId}", e.Context.Channel.Id, e.Context.Guild.Id);
        await OnErrorChannelReport(e.Context.Member, e.Context.Channel, e.Exception);
        //throw new NotImplementedException();
    }
    
    
    //@Jerry, I dont know what the fuck this is... but I'm going to let you fix it...
    
    /// <summary>
    /// To report back to the user regarding the error that occured. We do not tell what happened, just the fact that it did.
    /// This function responds to a user in a channel about an error that occured to the bot.
    /// </summary>
    /// <param name="m">The Guild Member Invoker</param>
    /// <param name="dc">The Channel to post this message in. </param>
    /// <returns></returns>
    private static async Task OnErrorChannelReport(DiscordMember? m, DiscordChannel dc, Exception e)
    {
        if (m == null) return;

        switch (e)
        {
            case CommandNotFoundException:
                await dc.SendMessageAsync(
                    $"{m.Mention} sorry but that command is not recognised");
                break;
            case InvalidOperationException:
                await dc.SendMessageAsync(
                    $"Hi {m.Username}, you attempted to perform an invalid command. This may be because the"+
                    " command is not fully implemented. Please contact a bot administrator.");
                break;
            default:
                await dc.SendMessageAsync(
                    $"Hi {m.Username}... So uh.... there seems to be a problem in paradise... Please contact a bot administrator....");
                break;
        }
    }
}