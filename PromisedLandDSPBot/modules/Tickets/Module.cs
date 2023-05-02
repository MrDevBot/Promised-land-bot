using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot.Modules.Tickets;

public class Module
{
    //slash command implementations
    [SlashCommandGroup("ticket", "ticket tool functionality group."), RequireGuild()]
    public class Slash : ApplicationCommandModule
    {
        
        [SlashCommand("create", "create a ticket from a message"), RequireGuild()]
        public async Task CreateTicket(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(":warning: this feature is still under development."));

            await Task.Delay(2000);
            await ctx.DeleteResponseAsync();
            
            /*
            var response = new DiscordInteractionResponseBuilder();
            response
                .WithTitle("Ticket Submission")
                
                .WithCustomId("Ticket.Create") //todo "database stuff"
                
                .AddComponents(new TextInputComponent(
                    "what is the nature of your ticket?", 
                    "problem",
                    "Hi! I'd like to report or recommend...", 
                    string.Empty, 
                    true, 
                    TextInputStyle.Paragraph, 
                    10, 
                    800 
                ))
                
                .AddComponents(new TextInputComponent(
                    "if applicable, what solution do you propose?", 
                    "solution", 
                    "I think we/you should...", 
                    string.Empty,
                    false, 
                    TextInputStyle.Paragraph, 
                    10, 
                    800));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, response);
        */
            
        }
    }
}