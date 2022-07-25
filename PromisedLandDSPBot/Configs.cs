namespace PromisedLandDSPBot;
using Newtonsoft.Json;

// this needs to be serializable to JSON - no complex objects. Keep it compliant.
public class MyServerConfig
{
    // 0 = unset
    public ulong GuildId { get; set; } = 0;
    /// <summary>
    /// If set aside from 0, this role defines the only role allowed to adjust permissions for the bot on this server.
    /// This is of course besides roles that have the Administrator role.
    /// </summary>
    public ulong MasterAdminRoleId { get; set; } = 0;
    public ulong DesignatedSuggestionChannelId { get; set; } = 0;
    
}

// for reaction role messages, we define 
public class MyReactionRoleConfig // to instance for Reaction role messages
{
    public ulong GuildId { get; } = 0;
    public ulong MessageId { get;  }= 0;

    /// <summary>
    /// String is ":xxxx:" to refer to reaction names as according to the guild.
    /// ulong refers to the id of the role to toggle.
    /// </summary>
    public Dictionary<string, ulong> ReactionRoles {get;} = new Dictionary<string, ulong>();

    /// <summary>
    /// 0 = Uninitialised
    /// 1 = Toggle - Add / Remove as context permits
    /// 2 = Add Only
    /// 3 = Remove Only
    /// </summary>
    public int Mode { get; set; }= 0;

    /// <summary>
    /// If defined, this role reaction message will disquallify users without this role from using the reaction.
    /// (In short, remove the reaction and prevent action)
    /// </summary>
    public ulong RoleRequiredId { get; set; } = 0;
    // to stop the reaction role, just remove the config file.
}


public class MySuggestionConfig // to instance once made
{
    public ulong SuggesterMessageId { get; } = 0;
    public ulong SuggestionMessageId { get; } = 0;
    public ulong ReviewerRoleId { get; } = 0;
    public float PercentAgreeMinMargin { get; set; } = 0.6f;
}