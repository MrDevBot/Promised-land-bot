using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LiteDB;
using PromisedLandDSPBot.Models;

namespace PromisedLandDSPBot.Modules.Levels;

public class Module
{
    private Config _config = Program.Config;
    
    //slash command implementations
    [SlashCommandGroup("xp", "Level and experience related commands"), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
        [SlashCommand("Query", "Gets the user's current xp and level.")]
        public async Task Query(InteractionContext ctx)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");
            
            var levelDataCollection = db.GetCollection<Level.User>("levelData");
            var levelX = new Level(levelDataCollection);
            var userLevelInfo = levelX.Get(ctx.User.Id);

            if (userLevelInfo != null)
            {
                var xp = userLevelInfo.Xp;
                var level = userLevelInfo.CalculateLevel((int)xp);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} has `{xp}` XP and is at level `{level}`."));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} does not have any XP"));
            }
        }
        
        [SlashCommand("Add", "Made to test how modals work in this framework. Requires the Manage Server permission.")]
        public async Task Add(InteractionContext ctx, [Option("xp", "the amount of xp to add")] int xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");
            
            var levelDataCollection = db.GetCollection<Level.User>("levelData");
            var XpData = new Level(levelDataCollection);
            var userLevelInfo = XpData.Get(ctx.User.Id);

            if (userLevelInfo != null)
            {
                // grab the user's current xp
                var currentXp = userLevelInfo.Xp;
                
                // add the new xp to the current xp
                var newXp = currentXp + xp;
                
                // write the new xp to the database
                XpData.Set(ctx.User.Id, newXp);
                
                var newLevel = userLevelInfo.CalculateLevel((int)newXp);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} has {newXp} XP and is now at level {newLevel}."));
            }
            else
            {
                XpData.Set(ctx.User.Id, xp);
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} has {xp} XP and is now at level {xp}."));
            }
        }
        
        [SlashCommand("Remove", "Made to test how modals work in this framework. Requires the Manage Server permission.")]
        public async Task Remove(InteractionContext ctx, [Option("xp", "the amount of xp to remove")] int xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");
            
            var levelDataCollection = db.GetCollection<Level.User>("levelData");
            var XpData = new Level(levelDataCollection);
            var userLevelInfo = XpData.Get(ctx.User.Id);

            if (userLevelInfo != null)
            {
                // grab the user's current xp
                var currentXp = userLevelInfo.Xp;
                
                // add the new xp to the current xp
                var newXp = currentXp - xp;
                
                // write the new xp to the database
                XpData.Set(ctx.User.Id, newXp);
                
                var newLevel = userLevelInfo.CalculateLevel((int)newXp);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} has {newXp} XP and is now at level {newLevel}."));
            }
            else
            {
                XpData.Set(ctx.User.Id, xp);
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} has {xp} XP and is now at level {xp}."));
            }
        }
        
        [SlashCommand("Set", "Sets the user's xp to the specified amount. Requires the Manage Server permission.")]
        public async Task Set(InteractionContext ctx, [Option("xp", "the amount of xp to set")] int xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");
            
            var levelDataCollection = db.GetCollection<Level.User>("levelData");
            var XpData = new Level(levelDataCollection);
            var userLevelInfo = XpData.Get(ctx.User.Id);

            if (userLevelInfo != null)
            {
                // grab the user's current xp
                var currentXp = userLevelInfo.Xp;
                
                // add the new xp to the current xp
                var newXp = xp;
                
                // write the new xp to the database
                XpData.Set(ctx.User.Id, newXp);
                
                var newLevel = userLevelInfo.CalculateLevel((int)newXp);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} has {newXp} XP and is now at level {newLevel}."));
            }
            else
            {
                XpData.Set(ctx.User.Id, xp);
                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"User {ctx.User.Mention} has {xp} XP and is now at level {xp}."));
            }
        }
    }
}