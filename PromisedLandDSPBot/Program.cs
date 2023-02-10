using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;

namespace PromisedLandDSPBot
{
    internal static class Program
    {
        private static void SeriLog()
        {
            SelfLog.Enable(message => Trace.WriteLine($"INTERNAL ERROR: {message}"));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        
            Log.Information("[{Name}] serilog sink started", "Logger");
        }
        
        public static Persistence.Config Config = new();
        private static DiscordConfiguration? _discordConfig;

        private static DiscordClient? _client;
        
        private static async Task Main()
        {
            SeriLog();
            
            Log.Information("[{Name}] checking for existence of token in config", Constants.Name);
            if (Config.Exists("token"))
            {
                Log.Information("[{Name}] token located", Constants.Name);
            }
            else
            {
                Log.Information("[{Name}] failed to locate token in config, requesting user input", Constants.Name);
                Console.WriteLine("Please enter your bot token:");
                string token = Console.ReadLine();
                Config.Set("token", token);
                Log.Information("[{Name}] token has been updated in config, token is now {Token}", Constants.Name, token);
            }
            

            // Change the token value
            //(token as dynamic).Token = "ODEwNTMzNjA1OTIyNTcwMzAw.GMHfq1.s5ScyPvEO94RtA8wj5myOfsca-8Pb0Z7xSNV4s";


            //var token = Config.GetToken();

            Log.Information("[{Name}] attempting connection to discord api", Constants.Name);

            _discordConfig = new DiscordConfiguration()
            {
                Token = Config.Get("token"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.Guilds 
                          | DiscordIntents.GuildEmojis 
                          | DiscordIntents.GuildMembers 
                          | DiscordIntents.GuildMessageReactions 
                          | DiscordIntents.DirectMessages
                          | DiscordIntents.AllUnprivileged,
                          // Remember to add these on the bot page!
                MinimumLogLevel = LogLevel.Warning
            };
            
            Log.Information("[{Name}] discord config is now instantiated", Constants.Name);

            MainBotLoop().GetAwaiter().GetResult();
        }
        
        private static async Task MainBotLoop()
        {
            _client = new DiscordClient(_discordConfig);
            
            Log.Information("[{Name}] discord client is now instantiated", Constants.Name);
            
            
            Log.Information("[{Name}] hooking events", Constants.Name);
            // event handlers are added in-scope here - for instance:
            //_client.MessageCreated += OnMessage;
            
            Log.Information("[{Name}] hooked GuildDiscovered event", Constants.Name);
            _client.GuildAvailable += Events.GuildDiscovered;
            
            
            Log.Information("[{Name}] registering command modules", Constants.Name);
            // add custom handlers handlers - for base command handlers, if the group is empty, comment it out. :thanks: 
            
            Log.Warning("[{Name}] text commands marked obsolete, text commands have been disabled", Constants.Name);
            //var commands = _client.UseCommandsNext(CommandConfig);
            //commands.RegisterCommands<Modules.Admin.Module.Base>();
            //commands.RegisterCommands<Modules.Debug.Module.Base>();
            //commands.RegisterCommands<Modules.Reactions.Module.Base>();
            //commands.RegisterCommands<Modules.Tickets.Module.Base>();
            //commands.RegisteredCommands<Modules.Triggers.Module.Base>();
            //commands.CommandErrored += Events.CommandsOnCommandErrored;

            var slash = _client.UseSlashCommands();
            
            Log.Information("[{Name}] registering {Modules}", Constants.Name, "modules.admin");
            slash.RegisterCommands<Modules.Admin.Module.Slash>();
            
            Log.Information("[{Name}] registering {Modules}", Constants.Name, "modules.debug");
            slash.RegisterCommands<Modules.Debug.Module.Slash>();
            
            Log.Information("[{Name}] registering {Modules}", Constants.Name, "modules.reactions");
            slash.RegisterCommands<Modules.Reactions.Module.Slash>();
            
            Log.Information("[{Name}] registering {Modules}", Constants.Name, "modules.tickets");
            slash.RegisterCommands<Modules.Tickets.Module.Slash>();
            
            Log.Information("[{Name}] registering {Modules}", Constants.Name, "modules.triggers");
            slash.RegisterCommands<Modules.Triggers.Module.Slash>();
            
            Log.Information("[{Name}] hooked {Event}", Constants.Name, "SlashOnSlashCommandErrored");
            slash.SlashCommandErrored += Events.SlashOnSlashCommandErrored;
            
            Log.Information("[{Name}] hooked {Event}", Constants.Name, "ClientOnModalSubmitted");
            _client.ModalSubmitted += Events.ClientOnModalSubmitted;

            Log.Information("[{Name}] finished registering modules", Constants.Name);

            Log.Information("[{Name}] attempting to authenticate with discord", Constants.Name);
            await _client.ConnectAsync();
            Log.Information("[{Name}] authenticated with discord as {Username}#{Tag} with User Id {Id}", Constants.Name, _client.CurrentUser.Username, _client.CurrentUser.Discriminator, _client.CurrentUser.Id.ToString());
            
            await Task.Delay(-1); // so the process doesn't end.
        }
    }
}