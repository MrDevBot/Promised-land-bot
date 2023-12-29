using System.Text;
using DSharpPlus;
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
        [RequireGuild]
        public async Task Query(InteractionContext ctx, [Option("user", "the user to target")] DiscordUser user)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");

            var databaseCollection = db.GetCollection<Level.User>("levelData");
            var levelUser = new Level(databaseCollection);

            if (levelUser.Get(user.Id) is { } lookupResult)
            {
                var xp = lookupResult.Xp;
                var level = Level.CalculateLevel(xp);
                
                await ctx.CreateResponseAsync($"User {user.Mention} has `{xp}` XP and is at level `{level}`.");
            }
            else
            {
                await ctx.CreateResponseAsync($"User {user.Mention} does not have any XP");
            }
        }

        [SlashCommand("Add", "Adds the specified amount of xp to the user. Requires Administrator permissions.")]
        [RequireGuild] [RequireUserPermissions(Permissions.Administrator)]
        public async Task Add(InteractionContext ctx, [Option("user", "the user to target")] DiscordUser user,
            [Option("xp", "the amount of xp to add")] long xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");

            var databaseCollection = db.GetCollection<Level.User>("levelData");
            var levelUser = new Level(databaseCollection);

            if (levelUser.Get(user.Id) is { } lookupResult)
            {
                // grab the user's current xp
                var currentXp = lookupResult.Xp;

                // add the new xp to the current xp
                var newXp = currentXp + xp;

                // write the new xp to the database
                levelUser.Set(user.Id, newXp);

                var newLevel = Level.CalculateLevel(newXp);
                
                await ctx.CreateResponseAsync($"User {user.Mention} has {newXp} XP and is now at level {newLevel}.");
            }
            else
            {
                levelUser.Set(user.Id, xp);

                await ctx.CreateResponseAsync($"User {user.Mention} has {xp} XP and is now at level {xp}.");
            }
        }

        [SlashCommand("Remove", "Made to test how modals work in this framework. Requires Administrator permissions.")]
        [RequireGuild] [RequireUserPermissions(Permissions.Administrator)]
        public async Task Remove(InteractionContext ctx,
            [Option("user", "the user to target")] DiscordUser user,
            [Option("xp", "the amount of xp to remove")] long xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");

            var databaseCollection = db.GetCollection<Level.User>("levelData");
            var levelUser = new Level(databaseCollection);

            if (levelUser.Get(user.Id) is { } lookupResult)
            {
                // grab the user's current xp
                var currentXp = lookupResult.Xp;

                // add the new xp to the current xp
                var newXp = currentXp - xp;

                if (newXp < 0) newXp = 0;
                
                // write the new xp to the database
                levelUser.Set(user.Id, newXp);

                var newLevel = Level.CalculateLevel(newXp);
                
                await ctx.CreateResponseAsync($"User {user.Mention} has {newXp} XP and is now at level {newLevel}.");
            }
            else
            {
                levelUser.Set(user.Id, xp);

                await ctx.CreateResponseAsync($"User {user.Mention} has {xp} XP and is now at level {xp}.");
            }
        }

        [SlashCommand("Set", "Sets the user's xp to the specified amount. Requires Administrator permissions.")]
        [RequireGuild] [RequireUserPermissions(Permissions.Administrator)]
        public async Task Set(InteractionContext ctx,
            [Option("user", "the user to target")] DiscordUser user,
            [Option("xp", "the amount of xp to set")] long xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");

            var databaseCollection = db.GetCollection<Level.User>("levelData");
            var levelUser = new Level(databaseCollection);

            if (levelUser.Get(user.Id) is { } lookupResult)
            {
                // grab the user's current xp

                // add the new xp to the current xp
                var newXp = xp;

                // write the new xp to the database
                levelUser.Set(user.Id, newXp);

                var newLevel = Level.CalculateLevel(newXp);
                
                await ctx.CreateResponseAsync($"User {user.Mention} has {newXp} XP and is now at level {newLevel}.");
            }
            else
            {
                levelUser.Set(user.Id, xp);

                await ctx.CreateResponseAsync($"User {user.Mention} has {xp} XP and is now at level {xp}.");
            }
        }
        
        [SlashCommand("Calculate", "Calculates the required XP for a given level or the level for a given XP")]
        public async Task Calculate(InteractionContext ctx,
            [Option("XP", "the total XP")] long? xp = null,
            [Option("Level", "the total Level")] long? level = null)
        {
            if (xp.HasValue && level.HasValue)
            {
                await ctx.CreateResponseAsync("Invalid set of parameters. Please provide either XP or Level, not both.", true);
                return;
            }

            if (xp.HasValue)
            {
                var calculatedLevel = Level.CalculateLevel(xp.Value);
                await ctx.CreateResponseAsync($"XP `{xp.Value}` equals Level `{calculatedLevel}`.");
            }
            else if (level.HasValue)
            {
                var calculatedXp = Level.CalculateXp(level.Value);
                await ctx.CreateResponseAsync($"Level `{level.Value}` equals `{calculatedXp}` XP.");
            }
            else
            {
                await ctx.CreateResponseAsync("Invalid set of parameters. Please provide either XP or Level.", true);
            }
        }
    }
}