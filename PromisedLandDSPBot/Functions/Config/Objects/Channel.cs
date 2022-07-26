namespace PromisedLandDSPBot.Functions.Config.Objects;

enum ChannelType
{
    Default, // indicates the channel is not special. should not be used. 
    Announcement, // indicates the channel is for announcements, multiple or null channels can be supplied.
    Log, // indicates the channel should be used for logs, multiple or null channels can be supplied.
    Debug, // indicates the channel is for debug, multiple or null channels can be supplied.
    Ticket // indicates the channel is a ticket channel created by the bot. its highly likely multiple exist
}

public class Channel
{
    internal ulong Id; // the channel id, should correlate Discord Id
    internal ChannelType Type; // the channel type
}