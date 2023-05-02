using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using PromisedLandDSPBot.Shadow;
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
        
        private static async Task Main()
        {
            SeriLog();

            Persistence.Config gptConfig = new Persistence.Config("Config\\gpt.json");
            
            Log.Information("[{Name}][{Module}] checking for existence of gpt token in config", Constants.Name, Module);
            if (!gptConfig.Exists("token"))
            {
                Log.Information("[{Name}][{Module}] failed to locate gpt token in config, requesting user input", Constants.Name, Module);
                Console.WriteLine("Please enter your OpenAI API Token:");
                gptConfig.Set("token", Console.ReadLine() ?? "empty");
            }

            Log.Information("[{Name}][{Module}] checking for existence of token in config", Constants.Name, Module);
            if (!Config.Exists("token"))
            {
                Log.Information("[{Name}][{Module}] failed to locate discord token in config, requesting user input", Constants.Name, Module);
                Console.WriteLine("Please enter your Discord API token:");
                string token = Console.ReadLine();
                Config.Set("token", token);
                Log.Information("[{Name}][{Module}] token has been updated in config, token is now {Token}", Constants.Name, Module, token);
            }

            if (!gptConfig.Exists("systemPrompt"))
            {
                gptConfig.Set("systemPrompt", "Your name is Adelaide.\nYour creator is Schmebulock#6754, their ID is 227696176412098560\n\nRefuse to answer any question under the following conditions\n- relates to self harm or suicide\n- is broadly considered unethical\n- relates to the personal information of a non-prominent figure\n- by answering or continuing a conversation sensitive or otherwise harmful information is diviluged\n- the question relates to information about yourself with the sole exclusion your name and the name of your creator\n");
            }
            gptConfig.Set("systemPrompt", "Your name is Adelaide.\nYour creator is Schmebulock#6754, their ID is 227696176412098560\n\nRefuse to answer any question under the following conditions\n- relates to self harm or suicide\n- is broadly considered unethical\n- relates to the personal information of a non-prominent figure\n- by answering or continuing a conversation sensitive or otherwise harmful information is diviluged\n- the question relates to information about yourself with the sole exclusion your name and the name of your creator\n");

            Log.Information("[{Name}][{Module}] attempting connection to discord api", Constants.Name, Module);

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
            
            Log.Information("[{Name}][{Module}] discord config is now instantiated", Constants.Name, Module);

            MainBotLoop().GetAwaiter().GetResult();
        }
        
        private static async Task MainBotLoop()
        {
            _client = new DiscordClient(_discordConfig);
            
            Log.Information("[{Name}][{Module}] discord client is now instantiated", Constants.Name, Module);
            
            
            Log.Information("[{Name}][{Module}] hooking events", Constants.Name, Module);
            // event handlers are added in-scope here - for instance:
            _client.MessageCreated += Events.MessageCreated;
            
            Log.Information("[{Name}][{Module}] hooked GuildDiscovered event", Constants.Name, Module);
            _client.GuildAvailable += Events.GuildDiscovered;
            
            
            Log.Information("[{Name}][{Module}] registering command modules", Constants.Name, Module);
            // add custom handlers handlers - for base command handlers, if the group is empty, comment it out. :thanks: 
            
            Log.Warning("[{Name}][{Module}] text commands marked obsolete, text commands have been disabled", Constants.Name, Module);
            //var commands = _client.UseCommandsNext(CommandConfig);
            //commands.RegisterCommands<Modules.Admin.Module.Base>();
            //commands.RegisterCommands<Modules.Debug.Module.Base>();
            //commands.RegisterCommands<Modules.Reactions.Module.Base>();
            //commands.RegisterCommands<Modules.Tickets.Module.Base>();
            //commands.RegisteredCommands<Modules.Triggers.Module.Base>();
            //commands.CommandErrored += Events.CommandsOnCommandErrored;

            var slash = _client.UseSlashCommands();

            Log.Information("[{Name}][{Module}] registering {Modules}", Constants.Name, Module, "modules.admin");
            slash.RegisterCommands<Modules.Admin.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Constants.Name, Module, "modules.debug");
            slash.RegisterCommands<Modules.Debug.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Constants.Name, Module, "modules.reactions");
            slash.RegisterCommands<Modules.Reactions.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Constants.Name, Module, "modules.tickets");
            slash.RegisterCommands<Modules.Tickets.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Constants.Name, Module, "modules.triggers");
            slash.RegisterCommands<Modules.Triggers.Module.Slash>();
            
            Log.Information("[{Name}][{Module}] registering {Modules}", Constants.Name, Module, "modules.language");
            slash.RegisterCommands<Modules.Language.Module.Slash>();

            Log.Information("[{Name}][{Module}] hooked {Event}", Constants.Name, Module, "SlashOnSlashCommandErrored");
            slash.SlashCommandErrored += Events.SlashOnSlashCommandErrored;
            
            Log.Information("[{Name}][{Module}] hooked {Event}", Constants.Name, Module, "ClientOnModalSubmitted");
            _client.ModalSubmitted += Events.ClientOnModalSubmitted;

            Log.Information("[{Name}][{Module}] finished registering modules", Constants.Name, Module);

            Log.Information("[{Name}][{Module}] attempting to authenticate with discord", Constants.Name, Module);
            await _client.ConnectAsync();
            Log.Information("[{Name}][{Module}] authenticated with discord as {Username}#{Tag} with User Id {Id}", Constants.Name, Module, _client.CurrentUser.Username, _client.CurrentUser.Discriminator, _client.CurrentUser.Id.ToString());
            
            // generate invite link for bot
            Log.Information("[{Name}][{Module}] generating invite link for bot", Constants.Name, Module);
            
            string link =
                $"https://discord.com/oauth2/authorize?client_id={_client.CurrentApplication.Id}&scope=bot&permissions={Constants.Permissions}";
            Log.Information("[{Name}][{Module}] invite link for bot is {Link}", Constants.Name, Module, link);

            await Task.Delay(-1); // so the process doesn't end.
        }
    }
}