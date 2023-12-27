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
        private const string Module = "BOOT";
        private static void SeriLog()
        {
            SelfLog.Enable(message => Trace.WriteLine($"INTERNAL ERROR: {message}"));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        
            Log.Information("[{Name}] serilog sink started", "LOGGER");
        }
        
        public static Persistence.Config Config = new();
        private static DiscordConfiguration? _discordConfig;

        private static DiscordClient? _client;
        
        public static Config Config;
        
        private static Events _events;
        
        private static async Task Main()
        {
            SeriLog();

            Config = Config.Load("config.json");
            _events = new Events(Config);
            
            Log.Information("[{Name}][{Module}] attempting connection to discord api", Config.Name, Module);

            _discordConfig = new DiscordConfiguration()
            {
                Token = Config.Token,
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
            
            Log.Information("[{Name}][{Module}] discord config is now instantiated", Config.Name, Module);

            MainBotLoop().GetAwaiter().GetResult();
        }
        
        private static async Task MainBotLoop()
        {
            _client = new DiscordClient(_discordConfig);
            
            Log.Information("[{Name}][{Module}] discord client is now instantiated", Config.Name, Module);
            
            
            Log.Information("[{Name}][{Module}] hooking events", Config.Name, Module);
            // event handlers are added in-scope here - for instance:
            _client.MessageCreated += _events.MessageCreated;
            
            Log.Information("[{Name}][{Module}] hooked GuildDiscovered event", Config.Name, Module);
            _client.GuildAvailable += _events.GuildDiscovered;
            
            
            Log.Information("[{Name}][{Module}] registering command modules", Config.Name, Module);
            // add custom handlers handlers - for base command handlers, if the group is empty, comment it out. :thanks: 
            
            Log.Warning("[{Name}][{Module}] text commands marked obsolete, text commands have been disabled", Config.Name, Module);
            //var commands = _client.UseCommandsNext(CommandConfig);
            //commands.RegisterCommands<Modules.Admin.Module.Base>();
            //commands.RegisterCommands<Modules.Debug.Module.Base>();
            //commands.RegisterCommands<Modules.Reactions.Module.Base>();
            //commands.RegisterCommands<Modules.Tickets.Module.Base>();
            //commands.RegisteredCommands<Modules.Triggers.Module.Base>();
            //commands.CommandErrored += Events.CommandsOnCommandErrored;

            var slash = _client.UseSlashCommands();

            Log.Information("[{Name}][{Module}] registering {Modules}", Config.Name, Module, "modules.admin");
            slash.RegisterCommands<Modules.Admin.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Config.Name, Module, "modules.debug");
            slash.RegisterCommands<Modules.Debug.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Config.Name, Module, "modules.reactions");
            slash.RegisterCommands<Modules.Reactions.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Config.Name, Module, "modules.tickets");
            slash.RegisterCommands<Modules.Tickets.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Config.Name, Module, "modules.triggers");
            slash.RegisterCommands<Modules.Triggers.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Config.Name, Module, "modules.levels");
            slash.RegisterCommands<Modules.Levels.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] hooked {Event}", Config.Name, Module, "SlashOnSlashCommandErrored");
            slash.SlashCommandErrored += _events.SlashOnSlashCommandErrored;
            
            Log.Information("[{Name}][{Module}] hooked {Event}", Config.Name, Module, "ClientOnModalSubmitted");
            _client.ModalSubmitted += _events.ClientOnModalSubmitted;

            Log.Information("[{Name}][{Module}] finished registering modules", Config.Name, Module);
    
            Log.Information("[{Name}][{Module}] attempting to authenticate with discord", Config.Name, Module);
            await _client.ConnectAsync();
            Log.Information("[{Name}][{Module}] authenticated with discord as {Username}#{Tag} with User Id {Id}", Constants.Name, Module, _client.CurrentUser.Username, _client.CurrentUser.Discriminator, _client.CurrentUser.Id.ToString());
            Log.Information("[{Name}][{Module}] authenticated with discord as {Username}#{Tag} with User Id {Id}", Config.Name, Module, _client.CurrentUser.Username, _client.CurrentUser.Discriminator, _client.CurrentUser.Id.ToString());
            
            // generate invite link for bot
            Log.Information("[{Name}][{Module}] generating invite link for bot", Config.Name, Module);
            
            string link =
                $"https://discord.com/oauth2/authorize?client_id={_client.CurrentApplication.Id}&scope=bot&permissions={Config.Permissions}";
            Log.Information("[{Name}][{Module}] invite link for bot is {Link}", Config.Name, Module, link);

            await Task.Delay(-1); // so the process doesn't end.
        }
    }
}