using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace PromisedLandDSPBot
{
    /// <summary>
    /// Endpoint configuration object
    /// </summary>
    public record Config
    {
        public Config()
        {
            // parameterless constructor for record
        }

        [JsonPropertyName("Token")] public string Token { get; init; } = "put_your_token_here";

        [JsonPropertyName("Name")] public string Name { get; init; } = "Adelaide V2";

        [JsonPropertyName("Owner")] public ulong Owner { get; init; } = 227696176412098560;
        
        [JsonPropertyName("Description")] public string Description { get; init; } = "Utility focused bot with `tickets`, `levels`, `moderation` and more!";
        
        [JsonPropertyName("Version")] public string Version { get; init; } = "V2 Indev, Electric Boogaloo";
        
        [JsonPropertyName("Permissions")] public ulong Permissions { get; init; } = 1497298594902; // generate using https://discordapi.com/permissions.html
        
        internal static Config New(string path)
        {
            // create a new config object
            var retVal = new Config { };
            
            Log.Fatal("Please edit the config file at {Path} and restart the bot to continue", path);
            
            File.WriteAllText(path, JsonSerializer.Serialize(retVal));
        
            return retVal;
        }
        
        internal static Config Load(string path, string? publicKeyOverride = null)
        {
            try
            {
                // this function while similar to Scylla.Node/Config.cs but has a "fail lethal" design due to the nature
                // of client public keys. It is impossible to securely authenticate or verify a client without a public key.
        
                // check if config file exists, throw exception if not
                if (!File.Exists(path)) return New(path);
        
                // read config file
                var configText = File.ReadAllText(path);

                // check for empty config file, throw exception if empty
                if (configText == string.Empty) return New(path);
        
                // deserialize config file
                Config? retVal = JsonSerializer.Deserialize<Config>(configText);

                // check for null config file, throw exception if null
                if (retVal is null) return New(path);
                
                // return config
                return retVal;
            }
            catch (Exception e)
            {
                Log.Error("Failed to load config with exception: {Exception}", e.Message);
                throw new Exception("Failed to load config", e);
            }
        }
    
        public void Save(string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(this));
        }
    }
}