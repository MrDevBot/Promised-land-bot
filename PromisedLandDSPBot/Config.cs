namespace PromisedLandDSPBot;
using Newtonsoft.Json;

internal static class Config
{
    private static bool _configLock = false;
    //Jerry, do NOT make this public, its encapsulated for a reason.
    private class Data
    {
        //dont put your token here, edit config.json
        [JsonProperty] internal string Token = "put your token here!";
        [JsonProperty] internal DateTime LastWrite = DateTime.UtcNow;
        [JsonProperty] internal List<ulong> Whitelist = new List<ulong>();
        [JsonProperty] internal bool WhitelistEnabled;
        
        
    }

    private static Task AwaitLock()
    {
        while (_configLock)
        {
            Task.Delay(10);
        }

        return Task.CompletedTask;
    }
    
    internal static async Task SetToken(string token, string path = "config.json")
    {
        await AwaitLock();
        
        //declare these values outside of branch
        Data deserializeObject;
        
        //check if config file exists
        if (File.Exists(path))
        {
            //read content from disk
            var json = await File.ReadAllTextAsync(path);

            //deserialize content, if json object is not present, fallback to new object
            deserializeObject = JsonConvert.DeserializeObject<Data>(json) ?? new Data();
        }
        else
        {
            //if the file does not exist, fall back to new config file
            deserializeObject = new Data();
        }

        //set new value
        deserializeObject.Token = token;
        deserializeObject.LastWrite = DateTime.UtcNow;

        //write new value to disk 
        await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(deserializeObject));
    }
    internal static async Task<string> GetToken(string path = "config.json")
    {
        await AwaitLock();
        
        //declare these values outside of branch
        Data deserializeObject;
        
        //check if config file exists
        if (File.Exists(path))
        {
            //read content from disk
            var json = await File.ReadAllTextAsync(path);

            //deserialize content, if json object is not present, fallback to new object
            deserializeObject = JsonConvert.DeserializeObject<Data>(json) ?? new Data();
        }
        else
        {
            //if the file does not exist, fall back to new config file
            deserializeObject = new Data();
        }

        //write new value to disk 
        await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(deserializeObject));
        
        //return value of token class
        return deserializeObject.Token;
    }
    public static class Whitelist
    {
        private static List<ulong> _whitelistCache = new List<ulong>();
        private static DateTime _whitelistLastAccess;
        private static bool _whitelistFirstAccess = true;
        internal static async Task<List<ulong>> Get(string path = "config.json")
        {
            await AwaitLock();
            
            //Whitelist.Add(476730054890618892); add sci-fi group to the config file
        
        
            //if the whitelist was last accessed less than 5 seconds ago
            if (_whitelistFirstAccess == false && _whitelistLastAccess.Subtract(DateTime.UtcNow).TotalSeconds <= 5)
            {
                _whitelistFirstAccess = false;
            
                //return cached whitelist
                return await Task.FromResult(_whitelistCache);
            }

            _whitelistLastAccess = DateTime.UtcNow;
            
            
            //declare these values outside of branch
            Data deserializeObject;
        
            //check if config file exists
            if (File.Exists(path))
            {
                //read content from disk
                var json = File.ReadAllText(path);

                //deserialize content, if json object is not present, fallback to new object
                deserializeObject = JsonConvert.DeserializeObject<Data>(json) ?? new Data();
            }
            else
            {
                //if the file does not exist, fall back to new config file
                deserializeObject = new Data();
            }

            //copy current whitelist to cache
            _whitelistCache = deserializeObject.Whitelist;
        
            //return current whitelist
            return await Task.FromResult(deserializeObject.Whitelist);
        }
        internal static async Task Add(ulong id, string path = "config.json")
        {
            await AwaitLock();
            
            //declare these values outside of branch
            Data deserializeObject;
        
            //check if config file exists
            if (File.Exists(path))
            {
                //read content from disk
                var json = await File.ReadAllTextAsync(path);

                //deserialize content, if json object is not present, fallback to new object
                deserializeObject = JsonConvert.DeserializeObject<Data>(json) ?? new Data();
            }
            else
            {
                //if the file does not exist, fall back to new config file
                deserializeObject = new Data();
            }

            //set new value, check for duplicates 
            if(!deserializeObject.Whitelist.Contains(id))
                deserializeObject.Whitelist.Add(id);
            
            deserializeObject.LastWrite = DateTime.UtcNow;

            //write new value to disk 
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(deserializeObject));
        }
        internal static async void Remove(ulong id, string path = "config.json")
        {
            await AwaitLock();
            
            //declare these values outside of branch
            Data deserializeObject;
        
            //check if config file exists
            if (File.Exists(path))
            {
                //read content from disk
                var json = await File.ReadAllTextAsync(path);

                //deserialize content, if json object is not present, fallback to new object
                deserializeObject = JsonConvert.DeserializeObject<Data>(json) ?? new Data();
            }
            else
            {
                //if the file does not exist, fall back to new config file
                deserializeObject = new Data();
            }

            //set new value

            //foreach (var guild in deserializeObject.Whitelist.Where(guild => guild == id))
            //{
            //    deserializeObject.Whitelist.Remove(guild);
            //}

            deserializeObject.Whitelist.Remove(id);

            deserializeObject.LastWrite = DateTime.UtcNow;

            //write new value to disk 
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(deserializeObject));
        }
    }
}