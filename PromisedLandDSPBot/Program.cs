using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace PromisedLandDSPBot // Note: actual namespace depends on the project name.
{
    // Program Entry
    internal static class Program
    {
        private static DiscordConfiguration? _config;
        
        /*
        private static readonly CommandsNextConfiguration CommandConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = new[] {">", "//"}
        };
        */
        
        private static DiscordClient? _client;


        private static async Task Main()
        {
            await Config.Whitelist.Add(476730054890618892);
            await Config.Whitelist.Add(962968548072906783);
            
            
            
            var token = Config.GetToken();

            Console.WriteLine("Bot Init...");

            _config = new DiscordConfiguration()
            {
                Token = token.Result,
                TokenType = TokenType.Bot, // How the bots API access is defined. (Leave as is)
                Intents = DiscordIntents.Guilds 
                          | DiscordIntents.GuildEmojis 
                          | DiscordIntents.GuildMembers 
                          | DiscordIntents.GuildMessageReactions 
                          | DiscordIntents.DirectMessages
                          | DiscordIntents.AllUnprivileged,
                          // Remember to add these on the bot page!
                //Intents = DiscordIntents.AllUnprivileged
            };
            
            //Log(Logger.Type.Debug, "Main Loop Achieved");
            Console.WriteLine("MainLoopAwait...");
            
            MainBotLoop().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Main Bot Loop - this is where we initiate the bot client and handle initialisation stuff
        /// </summary>
        private static async Task MainBotLoop()
        {
            _client = new DiscordClient(_config);
            
            //Log(Logger.Type.Debug, "Instantiated Client Object");
            Console.WriteLine("Instantiated Client Object");
            // event handlers are added in-scope here - for instance:

            //_client.MessageCreated += OnMessage;
            _client.GuildAvailable += Events.GuildDiscovered;
            
            // add custom handlers handlers - for base command handlers, if the group is empty, comment it out. :thanks: 
            
            //var commands = _client.UseCommandsNext(CommandConfig);
            //commands.RegisterCommands<Modules.Admin.Module.Base>();
            //commands.RegisterCommands<Modules.Debug.Module.Base>();
            //commands.RegisterCommands<Modules.Reactions.Module.Base>();
            //commands.RegisterCommands<Modules.Tickets.Module.Base>();
            //commands.RegisteredCommands<Modules.Triggers.Module.Base>();
            //commands.CommandErrored += Events.CommandsOnCommandErrored;
            
            //Log(Logger.Type.Debug, "Registered Legacy Commands");
            
            // add slash command handlers
            var slash = _client.UseSlashCommands();
            slash.RegisterCommands<Modules.Admin.Module.Slash>();
            slash.RegisterCommands<Modules.Debug.Module.Slash>();
            slash.RegisterCommands<Modules.Reactions.Module.Slash>();
            slash.RegisterCommands<Modules.Tickets.Module.Slash>();
            slash.RegisterCommands<Modules.Triggers.Module.Slash>();
            slash.SlashCommandErrored += Events.SlashOnSlashCommandErrored;
            
            //Log(Logger.Type.Debug, "Registered Slash Commands");
            
            
            // hook modal submitted stuffs
            _client.ModalSubmitted += Events.ClientOnModalSubmitted;

            Console.WriteLine("Modules added successfully!");
            
            // login to client and await this success.
            Console.WriteLine("Attempting Login...");
            await _client.ConnectAsync();
            //Log(Logger.Type.Debug, "Connected to Discord");
            
            Console.WriteLine("Login Complete!");
            await Task.Delay(-1); // so the process doesn't end.
        }
    }
}