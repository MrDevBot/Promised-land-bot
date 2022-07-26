// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");
// Written in .NET 5.0 Core notation, but supports all .NET Core 6.0 behaviour - have fun!
using System;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PromisedLandDSPBot.modules;

namespace PromisedLandDSPBot // Note: actual namespace depends on the project name.
{
    // Program Entry
    internal static class Program
    {
        private static DiscordConfiguration? _config;

        private static readonly CommandsNextConfiguration CommandConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = new[] {">"}
        };
        
        private static DiscordClient? _client;
        
        
        static void Main(string[] args)
        // Expects token as arg[0]. - for security reasons
        {
            Console.WriteLine("Bot Init...");
            if (args.Length != 1)
            {
                Console.WriteLine("No Token in Arguments! Please Enter your token: ");
                var inp = Console.ReadLine();
                if (inp == null)
                {
                    Console.WriteLine("Failed to Provide Token - Cancelling...");
                    return;
                }
                args = new[] { inp.ToString() };
                Console.Clear();
            }
            
            _config = new DiscordConfiguration()
            {
                Token = args[0],
                TokenType = TokenType.Bot, // How the bot's API access is defined. (Leave as is)
                Intents = DiscordIntents.Guilds 
                          | DiscordIntents.GuildEmojis 
                          | DiscordIntents.GuildMembers 
                          | DiscordIntents.GuildMessageReactions 
                          | DiscordIntents.DirectMessages
                          | DiscordIntents.AllUnprivileged,
                          // Remember to add these on the bot page!
                //Intents = DiscordIntents.AllUnprivileged
            };
            Console.WriteLine("Make Workspace...");
            
            Console.WriteLine("MainLoopAwait...");
            MainBotLoop().GetAwaiter().GetResult();
            
        }

        /// <summary>
        /// Main Bot Loop - this is where we initiate the bot client and handle initialisation stuff
        /// </summary>
        private static async Task MainBotLoop()
        {
            _client = new DiscordClient(_config);
            Console.WriteLine("Made Client Config. Adding Modules...");
            // event handlers are added in-scope here - for instance:
            _client.MessageCreated += OnMessage;
            
            // add custom handlers handlers
            var commands = _client.UseCommandsNext(CommandConfig);
            commands.RegisterCommands<CommandModule1>();
            commands.CommandErrored += CommandsOnCommandErrored;
            
            // add slash command handlers
            var slash = _client.UseSlashCommands();
            slash.RegisterCommands<SlashCommandModule1>();
            slash.SlashCommandErrored += SlashOnSlashCommandErrored;
            
            
            // hook modal submitted stuffs
            _client.ModalSubmitted += ClientOnModalSubmitted;



            Console.WriteLine("Modules added successfully!");
            // login to client and await this success.
            Console.WriteLine("Attempting Login...");
            await _client.ConnectAsync();
            Console.WriteLine("Login Complete!");
            await Task.Delay(-1); // so the process doesn't end.
            
        }

        private static Task ClientOnModalSubmitted(DiscordClient sender, ModalSubmitEventArgs e)
        {
            // Check modal id - if "suggestion-XXXX", delegate to a handler.
            //throw new NotImplementedException();
            return null;
        }

        private static async Task CommandsOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            Console.WriteLine($"Normal Command Error: \n{e.Exception.ToString()}\n\n and {e.Context.Command} was the culprit.");
            await OnErrorChannelReport(e.Context.Member, e.Context.Channel);
            //throw new NotImplementedException();
        }

        private static async Task SlashOnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            Console.WriteLine($"Slash Command Error: \n{e.Exception.ToString()}\n\n and {e.Context.CommandName} was the culprit.");
            await OnErrorChannelReport(e.Context.Member, e.Context.Channel);
            //throw new NotImplementedException();
        }

        
        // Example Async Task Definition for an OnMessage Event - note name can be anything - but best to keep it fairly nominal.
        private static async Task OnMessage(DiscordClient c, MessageCreateEventArgs e)
        {
            if (e.Message.Content.ToLower().StartsWith("ping")) 
                await e.Message.RespondAsync("pong!");

            if (e.Message.Content.ToLower().StartsWith("get config"))
            {
                
            }
        }


        /// <summary>
        /// To report back to the user regarding the error that occured. We do not tell what happened, just the fact that it did.
        /// </summary>
        /// <param name="m">The Guild Member Invoker</param>
        /// <param name="dc">The Channel to post this message in. </param>
        /// <returns></returns>
        private static async Task OnErrorChannelReport(DiscordMember? m, DiscordChannel dc)
        {
            if (m != null)
            {
                await dc.SendMessageAsync(
                    $"Hi {m.Username}... So uh.... there seems to be a problem in paradise... Please contact a bot administrator....");   
            }
        }
        
    }
}