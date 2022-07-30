using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Debug;
/// <summary>
/// Bot-specific debug commands - this is supposed to be used by bot admins to test functionality and debug errors.
/// </summary>
public class Module
{
    //slash command implementations
    [SlashCommandGroup("debug", "Debug Functions - Special Access Only"),
     RequireRoles(RoleCheckMode.All,
         983462019250421800)]
    public class Slash : ApplicationCommandModule
    {
        [SlashCommand("ping", "The bot will let you know.")]
        public async Task Ping(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($":ping_pong: Pong! {ctx.Client.Ping}ms"));
        }
        
        [SlashCommand("embedtest", "Made to test how embeds work in this framework."),
         RequirePermissions(Permissions.SendMessages)]
        //[RequireOwner] // this is owner of the bot, not owner of the server - so only the person who made the bot on the developer portal can access these commands.
        public async Task EmbedTestCommand(InteractionContext ctx,
            [Option("content", "This is the content of your embed post.")] string embedContent = "UNSET")
        {
            // CreateResponseAsync has a few overloads... one which helps here is shown below.
            var de = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        IconUrl = ctx.Member.AvatarUrl,
                        Name = ctx.Member.DisplayName,
                        //rl = ctx.Member.Username
                    },
                    Title = "Embed Test",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = ctx.Token },


                }
                .AddField("Content", $"{embedContent}")
                .AddField("Not In Line", "New Content Zone", true)
                .AddField("In Line", "This is in line", true)
                .Build();

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(de));
        }

        [SlashCommand("modaltest", "Made to test how modals work in this framework.")]
        public async Task ModalTestCommand(InteractionContext ctx,
            [Option("title", "The title of the Modal")]
            string title = "Modal Title")
        {
            var response = new DiscordInteractionResponseBuilder();
            response
                .WithTitle(title)
                .WithCustomId("my-modal-test")
                .AddComponents(new TextInputComponent(label: "Favorite food", customId: "fav-food",
                    placeholder: "Pizza, Icecream, etc", max_length: 30))
                .AddComponents(new TextInputComponent("Why?", "why-fav", "Because it tastes good", required: false,
                    style: TextInputStyle.Paragraph));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, response);

        }

        
    }

    //legacy command implementations
    [Group("debug")]
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