using DSharpPlus.CommandsNext;
using LiteDB;
using LiteDB.Async;

namespace PromisedLandDSPBot.Logging;

public class Logger
{
    /// <summary>
    /// database object
    /// </summary>
    private static readonly LiteDatabaseAsync LiteDatabase = new LiteDatabaseAsync("Filename=LDB.DISQL;Connection=shared");

    /// <summary>
    /// enum structure indicating what type of event is logged, useful when looking up events that have been saved to
    /// the database. new types can freely be added but removing a type can cause errors when deserializing prior data
    /// </summary>
    public enum Type
    {
        Default, // default, should not be used, should only be used as a fallback value.
        Debug, // a debug event, meaningless outside of internal debug / development.
        Command, // a event regarding a command, may require user notification or intervention.
        Permission // a event relating to the changing of or violation of a permission policy. useful for audit logs.
    }
    
    
    /// <summary>
    /// class structure containing the prefab for an event, used in serialization by LiteDB  
    /// </summary>
    class Event
    {
        internal Type Type;
        internal string Description;
        internal ulong Id;
        internal ulong Guild;
        internal ulong Channel;
    }
    
    /// <summary>
    /// create, serialize and writes and event to an internal database, if specified also logs to console.
    /// </summary>
    /// <param name="type">the type of event that has been logged</param>
    /// <param name="description">a human readable description of the event</param>
    /// <param name="ctx">the context in which the event was triggered</param>
    /// <param name="console">if the event should be written to console, defaults to false</param>
    public static void Log(Type type, string description, CommandContext ctx, bool console = false)
    {
        //get event table (no, this cannot be done statically, causes race condition if a single ILiteCollectionAsync is
        //access twice per instance 
        var events = LiteDatabase.GetCollection<Event>("Events");

        // create event object
        var @event = new Event
        {
            Channel = ctx.Channel.Id,
            Guild = ctx.Guild.Id,
            Description = description,
            Id = (ulong)BsonAutoId.Int64,
            Type = type
        };
        
        // if print to console was specified 
        if(console) Console.WriteLine($"[{type}] @{System.DateTime.Now} {description}");
        
        // register event
        events.UpsertAsync(@event);
    }
    
    public static void Log(Type type, string description, bool console = false)
    {
        //get event table (no, this cannot be done statically, causes race condition if a single ILiteCollectionAsync is
        //access twice per instance 
        var events = LiteDatabase.GetCollection<Event>("Events");

        // create event object
        var @event = new Event
        {
            //Channel = ctx.Channel.Id,
            //Guild = ctx.Guild.Id,
            Description = description,
            Id = (ulong)BsonAutoId.Int64,
            Type = type
        };
        
        // if print to console was specified 
        if(console) Console.WriteLine($"[{type}] @{System.DateTime.Now} {description}");
        
        // register event
        events.UpsertAsync(@event);
    }
}