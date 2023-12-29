using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using PromisedLandDSPBot.Functions.Permissions;
using static System.Reflection.Assembly;

namespace PromisedLandDSPBot.Modules.Debug;

/// <summary>
/// Bot-specific debug commands - this is supposed to be used by bot developers to test functionality and debug errors.
/// </summary>
public class Module
{
    //slash command implementations
    [SlashCommandGroup("dev", "Debug Functions - Special Access Only")]
    public class Slash : ApplicationCommandModule
    {
        private Config _config = Program.Config;
        
        [SlashCommand("ping", "the latency between the bot and discord")]
        public async Task Ping(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($":ping_pong: Pong! {ctx.Client.Ping}ms"));
        }

        [SlashCommand("about", "general information about the bot")]
        public async Task About(InteractionContext ctx)
        {
            string result = (ctx.Client.CurrentApplication.Owners ?? Array.Empty<DiscordUser>()).Aggregate(string.Empty, (current, owner) => current + (owner.Id == 227696176412098560 ? ":crown: **" : ":tools: ") + owner.Username + (owner.Id == 227696176412098560 ? "**" : String.Empty) + "\n");

            var de = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                        Name = ctx.Client.CurrentUser.Username,
                    },
                    Title = $"About {_config.Name}", Description = $"{_config.Description}",
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                        { Text = $"Application ID: {ctx.Client.CurrentApplication.Name}" }
                }
                .AddField("Entry Point", $"`{GetExecutingAssembly().EntryPoint!.Name}`", true)
                .AddField("Loaded Modules",
                    $"{GetExecutingAssembly().Modules.Aggregate(string.Empty, (current, module) => $"{current}\n`{module}`")}",
                    true)
                .AddField("Registered Developers",
                    result, true)
                .AddField("Version", _config.Version, true)
                //.AddField("In Line", "This is in line", true)
                .AddField("Created by", "**Kayarh**", false)
                .Build();
            
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(de));
        }

        [SlashCommand("Say", "developer command, not available to the public")]
        public async Task Say(InteractionContext ctx, [Option("message", "the message to send")] string message)
        {
            Checks.RejectNonDeveloper(ctx);

            await ctx.Client.SendMessageAsync(ctx.Channel, message);
            await ctx.CreateResponseAsync("Message Sent.", true);

            await Task.Delay(2000);

            await ctx.DeleteResponseAsync();
        }

        [SlashCommand("Embed", "developer command, not available to the public")]
        public async Task Embed(InteractionContext ctx,
            [Option("Title", "the title of the embed")]
            string title = "Title",
            [Option("Description", "the description of the embed")]
            string description = "Description")
        {
            Checks.RejectNonDeveloper(ctx);

            var de = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                        Name = ctx.Client.CurrentUser.Username,
                    },

                    Title = title, Description = description,
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                        { Text = $"Current Application: {ctx.Client.CurrentApplication.Name}" }
                }
                .Build();

            await ctx.Client.SendMessageAsync(ctx.Channel, de);

            await ctx.CreateResponseAsync("Embed Sent.", true);

            await Task.Delay(2000);
            await ctx.DeleteResponseAsync();
        }
    }

    [SlashCommand("Reply", "developer command, not available to the public")]
    public async Task Reply(InteractionContext ctx, [Option("id", "the message id to reply to")] string message,
        [Option("reply", "the message to send")] string reply)
    {
        Checks.RejectNonDeveloper(ctx);

        // try to cast the "message" string as a ulong
        if (!ulong.TryParse(message, out var id))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(":warning: invalid message id."));

            await Task.Delay(2000);
            await ctx.DeleteResponseAsync();

            return;
        }

        var builder = new DiscordMessageBuilder();
        builder.WithReply(ulong.Parse(message), true);
        builder.WithContent(reply);

        await ctx.Client.SendMessageAsync(ctx.Channel, builder);

        await ctx.CreateResponseAsync("Reply Sent.", true);

        await Task.Delay(2000);
        await ctx.DeleteResponseAsync();
    }
}