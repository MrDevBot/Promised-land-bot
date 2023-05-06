using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LiteDB;
using LiteDB.Async;

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
        
        [SlashCommand("close", "close the current ticket, can only be used in ticket channels"), RequireGuild()]
        public async Task CloseTicket(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(":warning: this feature is still under development."));

            await Task.Delay(2000);
            await ctx.DeleteResponseAsync();
        }

        class Ticket
        {
            internal Ticket()
            {
                db = new LiteDatabaseAsync("Filename=Conversations.db;Connection=shared;");
                epoch = DateTime.UtcNow;
                lastAccess = DateTime.UtcNow;
            }

            // the last time the conversation was accessed
            private DateTime lastAccess;
        
            // the time at which the conversation was instantiated into memory
            private readonly DateTime epoch;
        
            // database reference
            private LiteDatabaseAsync db;

            public class RequestResponse
            {
                // the id of the conversation
                public ObjectId TicketId { get; set; }
            
                // the users request
                public ulong TicketChannel { get; set; }
            
                // the models response
                public ulong TicketCreator { get; set; }
            
                // creation time of the object
                public DateTime Time { get; set; }

                // message history of the ticket
                public List<String> History { get; set; }
            }

        }
    }
}