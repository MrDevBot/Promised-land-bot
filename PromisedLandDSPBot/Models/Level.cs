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

            [BsonIgnore]
            public int CalculateLevel(int xp) => xp < 35 ? 0 : (xp < 55 ? 1 : (xp - 15) / 40);
        }

        public User Get(ulong userId)
        {
            return _levelDataCollection.FindOne(x => x.Id == userId);
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