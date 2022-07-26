namespace PromisedLandDSPBot;
using Newtonsoft.Json;

class Config
{
    internal class Context
    {
        internal string Token = null!;
    }

    public static Context ReadConfig(string path = "config.json")
    {
        if (File.Exists(path))
        {
            File.ReadAllText(path);
            var deserializeObject = JsonConvert.DeserializeObject<Context>(File.ReadAllText(path));

            using var file = File.OpenText(path);
            
            var serializer = new JsonSerializer();
            var context = (Context)serializer.Deserialize(file, typeof(Context))!;

            return context;
        }
        else
        {
            return CreateConfig(path);
        }
    }

    private static Context CreateConfig(string path)
    {
        Console.Write("please input your bot token: ");
        var ctx = new Context
        {
            Token = Console.ReadLine()!
        };

        File.WriteAllText(path, JsonConvert.SerializeObject(path));

        using var streamWriter = File.CreateText(path);
        var serializer = new JsonSerializer();
        serializer.Serialize(streamWriter, path);

        return ctx;
    }
}