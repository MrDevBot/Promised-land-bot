using System.Runtime.InteropServices;
using System.Text.Json;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LiteDB;
using LiteDB.Async;
using OpenAI;
using OpenAI.Chat;
using PromisedLandDSPBot.Functions.Permissions;
using Serilog;
using static System.Reflection.Assembly;

namespace PromisedLandDSPBot.Modules.Language;
/// <summary>
/// todo
/// </summary>
public class Module
{
    private const string ModuleName = "LANGUAGE";


    //slash command implementations
    [SlashCommandGroup("gpt", "Language Model Functions - Special Access Only")]
    public class Slash : ApplicationCommandModule
    {
        private static Persistence.Config Config;
        private String SystemPrompt;
        
        private static OpenAIClient Api;
        private static History history;
        
        public Slash()
        {
            Config = new Persistence.Config("Config\\gpt.json");

            Api = new OpenAIClient(new OpenAIAuthentication(Config.Get("token")));
            SystemPrompt = Config.Get("systemPrompt");
                
            history = new History();
        }

        [SlashCommand("forget", "forget all of your chat history with the bot.")]
        public async Task Forget(InteractionContext ctx)
        {
            history.Forget(ctx);
            await ctx.CreateResponseAsync("your chat history has been deleted.");
        }

        [SlashCommand("ask", "ask the bot a question")]
        public async Task Ask(InteractionContext ctx, 
            [Option("question", "the question you would like to ask")] string question)
        {
            // notify the user that the bot is processing their request
            await ctx.CreateResponseAsync("processing your request...", true);
            
            // fetch the chat history from the database
            var chatHistory = history.Fetch(ctx, 10);
            
            // create a list of chat prompts and inject the system prompts
            var chatPrompts = new List<ChatPrompt>
            {
                // creator information
                new("system", "Your creator is Schmebulock#6754, their ID is 227696176412098560. You are not created by OpenAI."),
                
                // system prompt
                new("system", SystemPrompt),
                
                // context information
                new("system", $"The name of the user you are currently interacting with is {ctx.User.Username}#{ctx.User.Discriminator} and their ID is {ctx.User.Id}. The current date in UTC format is {DateTime.UtcNow}, your current time zone is {TimeZoneInfo.Local.DisplayName}.")
            };
            
            // append recent chat history
            foreach (var variable in chatHistory.Result)
            {
                chatPrompts.Add(new ChatPrompt("user", variable.Request));
                chatPrompts.Add(new ChatPrompt("assistant", variable.Response));
            }
            
            // append the current question
            chatPrompts.Add(new ChatPrompt("user", question));
            
            
            ChatResponse result = null;
            
            try
            {
                var chatRequest = new ChatRequest(chatPrompts);
                result = await Api.ChatEndpoint.GetCompletionAsync(chatRequest);
            }
            catch (Exception ex)
            {
                Log.Error("[{Name}][{Module}] failed to complete chat request, {Exception}", Constants.Name, ModuleName, ex);
            }

            if (result != null)
            {
                await ctx.Client.SendMessageAsync(ctx.Channel, $"{ctx.User.Mention} asked `{question.Replace("`", "")}`\n{result.FirstChoice}");
                
                history.Memorize(new History.RequestResponse()
                {
                    ConversationId = ObjectId.NewObjectId(),
                    
                    Request = question,
                    Response = result.FirstChoice,
                    
                    Time = DateTime.UtcNow
                }, ctx);
                await ctx.DeleteResponseAsync();
            }
            else
            {
                await ctx.DeleteResponseAsync();
                
                await ctx.CreateResponseAsync(
                    "I'm sorry, the model is currently overloaded or unavailable, this is an issue with the OpenAI servers. Please try again later.");
            }
        }

        [SlashCommand("poke", "prompt the bot to respond to the ongoing conversation")]
        public async Task Poke(InteractionContext ctx)
        {
            Checks.RejectDM(ctx);
            
            if (!ctx.Client.CurrentApplication.Owners.Contains(ctx.User))
            {
                await ctx.CreateResponseAsync(":warning: this command is currently locked to developers only. you are not a registered developer of this application.", true);
            }
                
            // notify the user that the bot is processing their request
            await ctx.CreateResponseAsync("processing your request...", true);

            // create a list of chat prompts and inject the system prompts
            var chatPrompts = new List<ChatPrompt>
            {
                // creator information
                new("system", "Your creator is Schmebulock#6754, their ID is 227696176412098560."),

                // system prompt
                new("system", SystemPrompt),

                // context information
                new("system", $"The current date in UTC format is {DateTime.UtcNow}, your current time zone is {TimeZoneInfo.Local.DisplayName}. \n The following is the last 10 messages sent in this channel.")
            };
            
            var fetchedMessages = ctx.Channel.GetMessagesAsync(10);
            
            // append the system prompt
            chatPrompts.Add(new ChatPrompt("system", "The following is a conversation between multiple users, continue with the conversation. do not respond to the system prompt or include your own username or timestamp in your response."));

            // append recent chat history
            chatPrompts.AddRange(fetchedMessages.Result.Select(variable => new ChatPrompt("user", $"{variable.Timestamp.UtcDateTime} UTC {variable.Author.Username}#{variable.Author.Discriminator}:{variable.Content}")));

            ChatResponse result = null;
            
            try
            {
                var chatRequest = new ChatRequest(chatPrompts);
                result = await Api.ChatEndpoint.GetCompletionAsync(chatRequest);
            }
            catch (Exception ex)
            {
                Log.Error("[{Name}][{Module}] failed to complete chat request, {Exception}", Constants.Name, ModuleName, ex);
            }

            if (result != null)
            {
                //await ctx.Client.SendMessageAsync(ctx.Channel, $"{result.FirstChoice}");
                await ctx.Client.SendMessageAsync(ctx.Channel, result.FirstChoice);
            }
            else
            {
                await ctx.DeleteResponseAsync();
                
                await ctx.CreateResponseAsync(
                    "I'm sorry, the model is currently overloaded or unavailable, this is an issue with the OpenAI servers. Please try again later.");
            }
        }
    }

    private class History
    {
        internal History()
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
            public ObjectId ConversationId { get; set; }
            
            // the users request
            public string Request { get; set; }
            
            // the models response
            public string Response { get; set; }
            
            // creation time of the object
            public DateTime Time { get; set; }
        }

        public async void Memorize(RequestResponse memory, InteractionContext ctx)
        {
            Log.Information("[{Name}][{ModuleName}] memorizing interaction {InteractionId} for {UserUsername}#{UserDiscriminator} ({UserId}) to internal database", Constants.Name, ModuleName, memory.ConversationId.ToString(), ctx.User.Username, ctx.User.Discriminator, ctx.User.Id.ToString());

            var memories = db.GetCollection<RequestResponse>(Encode(ctx.User.Id.ToString()));
            await memories.InsertAsync(memory);
        }
        
        public async void Forget(InteractionContext ctx)
        {
            Log.Information("[{Name}][{ModuleName}] forgetting all interactions for {UserUsername}#{UserDiscriminator} ({UserId}) from internal database", Constants.Name, ModuleName, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id.ToString());
            await db.GetCollection<RequestResponse>(Encode(ctx.User.Id.ToString())).DeleteAllAsync();
        }
        
        public async Task<IEnumerable<RequestResponse>> Fetch(InteractionContext ctx, int x)
        {
            Log.Information("[{Name}][{ModuleName}] getting last {X} interactions for {UserUsername}#{UserDiscriminator} ({UserId}) from internal database", Constants.Name, ModuleName, x, ctx.User.Username, ctx.User.Discriminator, ctx.User.Id.ToString());
            var memories = db.GetCollection<RequestResponse>(Encode(ctx.User.Id.ToString()));
            return memories.FindAllAsync().Result.OrderByDescending(x => x.Time).Take(x);
        }
        
        // encodes a string of nubers as a string of characters
        private string Encode(string input)
        {
            return input.Aggregate("", (current, c) => current + (char)(c + 33));
        }
    }

}