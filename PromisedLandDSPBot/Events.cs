using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using LiteDB;
using PromisedLandDSPBot.Models;
using Serilog;

namespace PromisedLandDSPBot;

public class Events
{
    private const string Module = "EVENTS";
    private readonly Config _config;

    internal Events(Config config)
    {
        _config = config;
    }
    
    internal Task ClientOnModalSubmitted(DiscordClient sender, ModalSubmitEventArgs e)
    {
        Log.Information("[{Name}][{Module}] ClientOnModalSubmitted event triggered by NOT IMPLEMENTED", _config.Name, Module);
        // Check modal id - if "suggestion-XXXX", delegate to a handler.
        //throw new NotImplementedException();
        return null;
    }

    internal async Task CommandsOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        Log.Error("[{Name}][{Module}] an error occured with {Command} in module {Module} with error {Exception}", _config.Name, Module, e.Command.Name, e.Command.Module.ModuleType.Name, e.Exception.ToString());

        await OnErrorChannelReport(e.Context.Member, e.Context.Channel, e.Exception);
    }

    internal async Task SlashOnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
    {
        Log.Error("[{Name}][{Module}] an error occured with {Command} in module {Module} with error {Exception}", _config.Name, Module, e.Context.CommandName, e.Context.SlashCommandsExtension.ToString(), e.Exception.ToString());

        await OnErrorChannelReport(e.Context.Member, e.Context.Channel, e.Exception);
        //throw new NotImplementedException();
    }
    
    
    //@Jerry, I dont know what the fuck this is... but I'm going to let you fix it...
    
    /// <summary>
    /// To report back to the user regarding the error that occured. We do not tell what happened, just the fact that it did.
    /// </summary>
    /// <param name="m">The Guild Member Invoker</param>
    /// <param name="dc">The Channel to post this message in. </param>
    /// <returns></returns>
    private async Task OnErrorChannelReport(DiscordMember? m, DiscordChannel dc, Exception e)
    {
        if (m == null) return;
        
        Log.Error("[{Name}][{Module}] an error occured with {Command} in module {Module} with error {Exception}", _config.Name, Module, e.Message, e.Source, e.ToString());
        
/*
        switch (e)
        {
            case CommandNotFoundException:
                await dc.SendMessageAsync(
                    $"{m.Mention} sorry but that command is not recognised");
                break;
            case InvalidOperationException:
                await dc.SendMessageAsync(
                    $"${m.Username}, you attempted to perform an invalid command. This may be because the"+
                    " command is not fully implemented. Please contact a bot administrator.");
                break;
            default:
                await dc.SendMessageAsync(
                    $"sorry {m.Mention}, an error occured while trying to execute your command");
                break;
        }
        */
    }
    
    internal Task GuildDiscovered(DiscordClient sender, GuildCreateEventArgs e)
    {
        Log.Information("[{Name}][{Module}] discovered guild {Guild} with Id {Id}", _config.Name, Module, e.Guild.Name, e.Guild.Id.ToString());
        return Task.CompletedTask;
    }

    public async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
    {
        Log.Information("[{Name}][{Module}] message created in {Channel} by {User}#{Discriminator} ({Id}) with content {Content}", _config.Name, Module, e.Channel.Name, e.Author.Username, e.Author.Discriminator, e.Author.Id.ToString(), e.Message.Content);

        if (e.Message.Content.StartsWith(sender.CurrentUser.Mention))
        {
            Log.Information("[{Name}][{Module}] message directly mentioned bot, passing to language model", _config.Name, Module);

            // todo parse message to language model
        }

        if (!e.Author.IsBot)
        {
            using var db = new LiteDatabase($"Filename={e.Guild.Id}.db;Mode=Shared");
            var levelDataCollection = db.GetCollection<Level.User>("levelData");
            var userRef = new Level(levelDataCollection);
            var userLevel = userRef.Get(e.Author.Id);
            
            var xpToAdd = 5 + (e.Message.Attachments.Any() ? 5 : 0);
            
            userLevel = userLevel == null ? new Level.User { Id = e.Author.Id, Xp = xpToAdd } :
                // If the User object exists, add experience points to it
                new Level.User { Id = userLevel.Id, Xp = userLevel.Xp + xpToAdd };

            // Update the User object in the database
            userRef.Set(userLevel.Id, userLevel.Xp);            
        }

    }
}