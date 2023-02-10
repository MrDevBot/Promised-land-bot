using Newtonsoft.Json;

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
        private readonly object _lock = new object();
        private string filepath;

        public Config(string filepath = "config.json")
        {
            this.filepath = filepath;
            CheckNull();
        }

        private void CheckNull()
        {
            lock (_lock)
            {
                if (JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(this.filepath)) == null)
                {
                    File.WriteAllText(this.filepath, "[]");
                }
            }
        }

        public string Get(string key)
        {
            lock (_lock)
            {
                List<Entry> entries = JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(this.filepath)) ?? throw new InvalidOperationException();

                // search for key, if key exists return its value, else return null
                return entries.Where(entry => entry.key == key).Select(entry => entry.value.ToString()).FirstOrDefault() ?? throw new InvalidOperationException();
            }
        }

        public void Set(string key, string value)
        {
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
            lock (_lock)
            {
                List<Entry> entries = JsonConvert.DeserializeObject<List<Entry>>(File.ReadAllText(this.filepath)) ?? throw new InvalidOperationException();
                return entries.Any(entry => entry.key == key);
            }
        }
    }
}