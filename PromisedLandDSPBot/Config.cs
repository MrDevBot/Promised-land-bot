using Newtonsoft.Json;
using PromisedLandDSPBot;
using Serilog;

public static class Persistence
{
    private struct Entry
    {
        public ulong UUID;
        public string key;
        public object value;
    }
    
    public class Config
    {
        Guid uuid = Guid.NewGuid();
        
        private readonly object _lock = new object();
        private string filepath;

        public Config(string filepath = "Config\\core.json")
        {
            Log.Information("[{Name}][{Module}] config reader {Uuid} bound to path {Path}", Constants.Name, "CONFIG", uuid.ToString(), filepath);
            this.filepath = filepath;
            CheckNull();
        }

        private void CheckNull()
        {
            lock (_lock)
            {
                if (File.Exists(filepath))
                {
                    Log.Information("[{Name}][{Module}] config reader {Uuid} located its bound file", Constants.Name, "CONFIG", uuid.ToString(), filepath);
                    
                    if (JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(this.filepath)) != null) return;
                    
                    Log.Warning("[{Name}][{Module}] config reader {Uuid} contained empty file, injecting blank structure", Constants.Name, "CONFIG", uuid.ToString(), filepath);
                    File.WriteAllText(this.filepath, "[]");
                }
                else
                {
                    Log.Warning("[{Name}][{Module}] config reader {Uuid} failed to locate its bound file, creating new file", Constants.Name, "CONFIG", uuid.ToString(), filepath);
                    File.WriteAllText(this.filepath, "[]");
                }
                

            }
        }

        public string Get(string key)
        {
            Log.Information("[{Name}][{Module}] config reader {Uuid} attempting to get key {Key}", Constants.Name, "CONFIG", uuid.ToString(), key);
            lock (_lock)
            {
                List<Entry> entries = JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(this.filepath)) ?? throw new InvalidOperationException();

                // search for key, if key exists return its value, else return null
                return entries.Where(entry => entry.key == key).Select(entry => entry.value.ToString()).FirstOrDefault() ?? throw new InvalidOperationException();
            }
        }

        public void Set(string key, string value)
        {
            Log.Information("[{Name}][{Module}] config reader {Uuid} attempting to set key {Key} to value {Value}", Constants.Name, "CONFIG", uuid.ToString(), key, value);
            lock (_lock)
            {
                List<Entry> entries = JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(filepath)) ?? throw new InvalidOperationException();

                // search for key, if key exists set its value, else add a new entry
                if (entries.Any(entry => entry.key == key))
                {
                    entries.Where(entry => entry.key == key).Select(entry => entry.value = value).FirstOrDefault();
                }
                else
                {
                    entries.Add(new Entry()
                    {
                        UUID = (ulong)entries.Count,
                        key = key,
                        value = value
                    });

                    File.WriteAllText(this.filepath, JsonConvert.SerializeObject(entries));
                }
            }
        }

        public bool Exists(string key)
        {
            Log.Information("[{Name}][{Module}] config reader {Uuid} attempting to check if key {Key} exists", Constants.Name, "CONFIG", uuid.ToString(), key);
            lock (_lock)
            {
                List<Entry> entries = JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(this.filepath)) ?? throw new InvalidOperationException();
                return entries.Any(entry => entry.key == key);
            }
        }
    }
}