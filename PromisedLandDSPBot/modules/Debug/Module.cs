using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Debug;

public class Module
{
    //slash command implementations
    public class Slash : ApplicationCommandModule
    {
        [SlashCommand("ping", "The bot will let you know.")]
        public async Task Ping(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($":ping_pong: Pong! {ctx.Client.Ping}ms"));
        }
    }

    //legacy command implementations
    public class Base : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx) // ADD command parameters as if they are parameters in code. 
        {
            await ctx.RespondAsync($":ping_pong: Pong! {ctx.Client.Ping}ms");
        }
        
        [Command("debug")]
        public async Task Debug(CommandContext ctx) // ADD command parameters as if they are parameters in code. 
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Name = ctx.Client.CurrentUser.Username,
                    Url = ""
                },
                Color = new Optional<DiscordColor>(DiscordColor.Orange),
                Description = string.Empty,
                Timestamp = DateTimeOffset.UtcNow
            };

            embedBuilder.AddField("OS", Environment.OSVersion.VersionString, true);
            embedBuilder.AddField("CLR", Environment.Version.ToString(), true);
            embedBuilder.AddField("Stacktrace", Environment.StackTrace, false);

            await ctx.RespondAsync(embedBuilder.Build());
        }
    }
}