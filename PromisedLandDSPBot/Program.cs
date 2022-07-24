// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");
// Written in .NET 5.0 Core notation, but supports all .NET Core 6.0 behaviour - have fun!
using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace PromisedLandDSPBot // Note: actual namespace depends on the project name.
{
    // Program Entry
    internal class Program
    {
        private static DiscordConfiguration? config;
        private static DiscordClient? Client;
        
        
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
            
            config = new DiscordConfiguration()
            {
                Token = args[0],
                TokenType = TokenType.Bot, // How the bot's API access is defined. (Leave as is)
                Intents = DiscordIntents.AllUnprivileged // ATM, this is enough, but we may need to add these for functionality reasons. Remember to add the intents to the discord dev page!
            };
            Console.WriteLine("MainLoopAwait...");
            MainBotLoop().GetAwaiter().GetResult();
            
        }

        /// <summary>
        /// Main Bot Loop - this is where we initiate the bot client and handle initialisation stuff
        /// </summary>
        private static async Task MainBotLoop()
        {
            Client = new DiscordClient(config);
            
            // login to client and await this success.
            Console.WriteLine("Attempting Login...");
            await Client.ConnectAsync();
            Console.WriteLine("Login Complete!");
            // event handlers are added in-scope here - for instance:
            Client.MessageCreated += OnMessage;
            //---- OR ----
            /*
            Client.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping")) 
                    await e.Message.RespondAsync("pong!");
            };
            */
            
            await Task.Delay(-1); // so the process doesn't end.
            
        }

        // Example Async Task Definition for an OnMessage Event - note name can be anything - but best to keep it fairly nominal.
        private static async Task OnMessage(DiscordClient c, MessageCreateEventArgs e)
        {
            if (e.Message.Content.ToLower().StartsWith("ping")) 
                await e.Message.RespondAsync("pong!"); 
        }
        
    }
}