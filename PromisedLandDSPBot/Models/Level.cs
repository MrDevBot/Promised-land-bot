using LiteDB;

namespace PromisedLandDSPBot.Models
{
    public class Level
    {
        private ILiteCollection<User> _levelDataCollection;

        public Level(ILiteCollection<User> levelDataCollection)
        {
            _levelDataCollection = levelDataCollection;
        }

        public class User
        {
            public ulong Id { get; init; }
            public long Xp { get; init; }
        }
        
        public static long CalculateLevel(long xp) => xp < 35 ? 0 : (xp < 55 ? 1 : (xp - 15) / 40);
        public static long CalculateXp(long level) => level < 1 ? 0 : (level < 2 ? 35 : 15 + (level - 1) * 40);
        
        public User Get(ulong userId)
        {
            var retVal = _levelDataCollection.FindOne(x => x.Id == userId);
            // null check
            if (retVal != null) return retVal;
            
            // create a new user
            retVal = new User { Id = userId, Xp = 0 };
            // insert the new user into the database
            _levelDataCollection.Insert(retVal);
            return retVal;
        }

        public void Set(ulong userId, long xp)
        {
            _levelDataCollection.Upsert(new User()
            {
                Id = userId,
                Xp = xp
            });
        }
    }
}