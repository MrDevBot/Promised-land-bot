using System.Runtime.InteropServices.JavaScript;
using DSharpPlus.Entities;
using LiteDB.Async;

namespace PromisedLandDSPBot.Shadow;

public class User
{
    private static LiteDatabaseAsync Database = new LiteDatabaseAsync("Filename=shadow.lda;Connection=shared;");

    public class ShadowUser
    {
        // public constructor
        public ShadowUser(ulong id, String name, string discriminator, ShadowUserType type)
        {
            this._id = id;
            this._name = name;
            this._discriminator = discriminator;
            this._type = type;
        }
        
        // private fields
        private ulong _id;
        private String _name;
        private string _discriminator;
        private ShadowUserType _type;

        // public properties
        public ulong Id { get => _id; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Task.Run(Update);
            }
        }
        public string Discriminator
        {
            get => _discriminator;
            set
            {
                _discriminator = value;
                Task.Run(Update);
            }
        }
        public ShadowUserType Type
        {
            get => _type;
            set
            {
                _type = value;
                Task.Run(Update);
            }
        }
        
        // private update method
        private async Task Update()
        {
            await Database.GetCollection<ShadowUser>("Users").UpsertAsync(this);
        }
    } 
    
    public class ShadowChannel
    {
        // public constructor
        public ShadowChannel(ulong id, string name, ulong guild, ShadowChannelType type)
        {
            this._id = id;
            this._name = name;
            this._type = type;
        }
        
        // private fields
        private ulong _id;
        private String _name;
        private ShadowChannelType _type;
        
        // public properties
        public ulong Id { get => _id; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Task.Run(Update);
            }
        }
        public ShadowChannelType Type
        {
            get => _type;
            set
            {
                _type = value;
                Task.Run(Update);
            }
        }

        // private update method
        private async Task Update()
        {
            await Database.GetCollection<ShadowChannel>("Channels").UpsertAsync(this);
        }
    }

    class ShadowGuild
    {
        private ulong _id;
        private string _name;
        List<ShadowUser>? _users;
        List<ShadowChannel> _channels;
        ShadowGuildType _type;

        protected ShadowGuild(ulong id, string name, List<ShadowUser>? users, List<ShadowChannel>? channels, ShadowGuildType type)
        {
            this._id = id;
            this._name = name;
            this._users = users ??= new List<ShadowUser>();
            this._channels = channels ??= new List<ShadowChannel>();
            this._type = type;
        }
        public async Task Update()
        {
            await Database.GetCollection<ShadowGuild>("Guilds").UpsertAsync(this);
        }
    }
    
    enum ShadowGuildType
    {
        Normal,
        Untrusted,
        Trusted,
        Development
    }
    public enum ShadowChannelType
    {
        Text,
        Voice,
        Category,
        News,
        Store,
        Stage,
        Ticket,
        Unknown
    }
    public enum ShadowUserType
    {
        User,
        Developer,
        Creator
    }
}