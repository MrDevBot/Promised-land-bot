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
                var level = lookupResult.CalculateLevel((int)xp);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} has `{xp}` XP and is at level `{level}`."));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} does not have any XP"));
            }
        }

        [SlashCommand("Add", "Made to test how modals work in this framework. Requires the Manage Server permission.")]
        [RequireGuild] [RequireUserPermissions(Permissions.Administrator)]
        public async Task Add(InteractionContext ctx, [Option("user", "the user to target")] DiscordUser user,
            [Option("xp", "the amount of xp to add")] long xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");

            var databaseCollection = db.GetCollection<Level.User>("levelData");
            var levelUser = new Level(databaseCollection);

            if (levelUser.Get(ctx.User.Id) is { } lookupResult)
            {
                // grab the user's current xp
                var currentXp = lookupResult.Xp;

                // add the new xp to the current xp
                var newXp = currentXp + xp;

                // write the new xp to the database
                levelUser.Set(ctx.User.Id, newXp);

                var newLevel = lookupResult.CalculateLevel((int)newXp);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} has {newXp} XP and is now at level {newLevel}."));
            }
            else
            {
                levelUser.Set(ctx.User.Id, xp);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} has {xp} XP and is now at level {xp}."));
            }
        }

        [SlashCommand("Remove", "Made to test how modals work in this framework. Requires the Manage Server permission.")]
        [RequireGuild] [RequireUserPermissions(Permissions.Administrator)]
        public async Task Remove(InteractionContext ctx,
            [Option("user", "the user to target")] DiscordUser user,
            [Option("xp", "the amount of xp to remove")] long xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");

            var databaseCollection = db.GetCollection<Level.User>("levelData");
            var levelUser = new Level(databaseCollection);

            if (levelUser.Get(ctx.User.Id) is { } lookupResult)
            {
                // grab the user's current xp
                var currentXp = lookupResult.Xp;

                // add the new xp to the current xp
                var newXp = currentXp - xp;

                // write the new xp to the database
                levelUser.Set(ctx.User.Id, newXp);

                var newLevel = lookupResult.CalculateLevel((int)newXp);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} has {newXp} XP and is now at level {newLevel}."));
            }
            else
            {
                levelUser.Set(ctx.User.Id, xp);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} has {xp} XP and is now at level {xp}."));
            }
        }

        [SlashCommand("Set", "Sets the user's xp to the specified amount. Requires the Manage Server permission.")]
        [RequireGuild] [RequireUserPermissions(Permissions.Administrator)]
        public async Task Set(InteractionContext ctx,
            [Option("user", "the user to target")] DiscordUser user,
            [Option("xp", "the amount of xp to set")] long xp)
        {
            using var db = new LiteDatabase($"Filename={ctx.Guild.Id}.db;Mode=Shared");

            var databaseCollection = db.GetCollection<Level.User>("levelData");
            var levelUser = new Level(databaseCollection);

            if (levelUser.Get(ctx.User.Id) is { } lookupResult)
            {
                // grab the user's current xp

                // add the new xp to the current xp
                var newXp = xp;

                // write the new xp to the database
                levelUser.Set(ctx.User.Id, newXp);

                var newLevel = lookupResult.CalculateLevel((int)newXp);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} has {newXp} XP and is now at level {newLevel}."));
            }
            else
            {
                levelUser.Set(ctx.User.Id, xp);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(
                        $"User {ctx.User.Mention} has {xp} XP and is now at level {xp}."));
            }
        }
    }
}