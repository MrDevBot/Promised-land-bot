namespace PromisedLandDSPBot.Functions.Config.Objects;

// guild specific 
public class Guild
{
    enum GuildType
    {
        Production, // the bot is running on a "production" server, debug commands and output should be disabled
        Testing // the bot is running on a "test" server, debug commands, output and commands should be enabled
    }
    
    //internal List<User> Users; // queryable list of users
    internal List<Channel> Channels; // queryable list of channels
}