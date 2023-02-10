using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using static System.Reflection.Assembly;

namespace PromisedLandDSPBot.Modules.Debug;
/// <summary>
/// Bot-specific debug commands - this is supposed to be used by bot admins to test functionality and debug errors.
/// </summary>
public class Module
{
    //slash command implementations
    [SlashCommandGroup("info", "Debug Functions - Special Access Only")]
    public class Slash : ApplicationCommandModule
    {
        [SlashCommand("ping", "the latency between the bot and discord")]
        public async Task Ping(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($":ping_pong: Pong! {ctx.Client.Ping}ms"));
        }
        
        [SlashCommand("about", "general information about the bot")]
        public async Task About(InteractionContext ctx)
        {
            var de = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                        Name = ctx.Client.CurrentUser.Username,
                    },
                    Title = $"About {Constants.Name}", Description = $"{Constants.Description}",
                    Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = $"Application ID: {ctx.Client.CurrentApplication.Name}" }
                }
                .AddField("Entry Point", $"`{GetExecutingAssembly().EntryPoint!.Name}`", true)
                .AddField("Loaded Modules", $"{GetExecutingAssembly().Modules.Aggregate(string.Empty, (current, module) => $"{current}\n`{module}`")}", true)
                .AddField("Developers", Constants.Developers.Aggregate(string.Empty, (current, developer) => $"{current}\n{developer}"), true)
                .AddField("Version", Constants.Version, true)
                //.AddField("In Line", "This is in line", true)
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
                .WithTitle("Super cool modal!")
                .WithCustomId("my-modal")
                .AddComponents(new TextInputComponent(label: "Favorite food", customId: "fav-food",
                    placeholder: "Pizza, Icecream, etc", max_length: 30))
                .AddComponents(new TextInputComponent("Why?", "why-fav", "Because it tastes good", required: false,
                    style: TextInputStyle.Paragraph));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, response);

        }

        
    }
}