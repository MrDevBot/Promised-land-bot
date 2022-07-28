namespace PromisedLandDSPBot;
using Newtonsoft.Json;
// todo add error handling in case of IO exception.
class Config
{
    internal class Context
    {
        public string Token = null!;

    }

    public static Context ReadConfig(string path = "config.json")
    {
        if (File.Exists(path))
        {
            File.ReadAllText(path);
            var deserializeObject = JsonConvert.DeserializeObject<Context>(File.ReadAllText(path));
            
            // in case the file read in is not able to be deserialized properly, we ask to remake the config
            // - as the only reason it should fail is either a malformed JSON or other such problem.
            if (deserializeObject == null)
            {
                return CreateConfig(path);
            }
            return (Context) deserializeObject;
        }
        else
        {
            return CreateConfig(path);
        }
    }

    private static Context CreateConfig(string path)
    {
        Console.Write("please input your bot token: ");
        var ctx = new Context()
        {
            Token = Console.ReadLine()!
        };
        //Console.WriteLine(ctx.Token);
        File.WriteAllText(path, JsonConvert.SerializeObject(ctx));
        return ctx;
    }
}