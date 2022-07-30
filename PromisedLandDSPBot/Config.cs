using System.Diagnostics;

namespace PromisedLandDSPBot;
using Newtonsoft.Json;
// todo add error handling in case of IO exception.
internal class Config
{
    //Jerry, do NOT make this public, its encapsulated for a reason.
    internal class Token
    {
        public string value;
    }
    
    public static void SetToken(Token token, string path = "config.json", bool writeNew=false)
    {
        if (File.Exists(path) == false)
        {
            switch (writeNew)
            {
                case true:
                    File.WriteAllText(path, JsonConvert.SerializeObject(token));
                    break;
                default:
                    throw new FileNotFoundException($"'{path}' does not exist to read from to alter; not allowed to write new.");
                
            }
        }
        else // assume the file exists. 
        {
            //read content from disk
            var json = File.ReadAllText(path);
        
            //deserialize content
            var value = JsonConvert.DeserializeObject<Token>(json);
        
            //set new value
            value = token;
        
            //write new value to disk
            File.WriteAllText(path, JsonConvert.SerializeObject(value));    
        }
        
        
    }
    
    public static string GetToken(string path = "config.json")
    {
        // Check if the file exists, if not, create it
        if (!File.Exists(path))
        {
            Console.WriteLine("Please Enter your Token: ");
            string tkn = Console.ReadLine()!;
            Token t = new Token() {value = tkn};
            File.WriteAllText(path, JsonConvert.SerializeObject(t));
        }
        
        //read content from disk
        var json = File.ReadAllText(path);
        
        //deserialize content
        var deserializeObject = JsonConvert.DeserializeObject<Token>(json);
        
        //instantiate token class
        var token = deserializeObject;

        //return value of token class
        return token.value;
    }
}