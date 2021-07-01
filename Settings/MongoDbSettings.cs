using System;

namespace authServer.Settings
{
    public class MongoDbSettings
    {
        public string Host = Environment.GetEnvironmentVariable("ASHOST");
        public string Database = Environment.GetEnvironmentVariable("ASDATABASE");
        public string User = Environment.GetEnvironmentVariable("ASUSER");
        public string Password = Environment.GetEnvironmentVariable("ASPW");
        public string Url = Environment.GetEnvironmentVariable("ASURL");

        public int Port
        {
            get
            {
                try
                {
                    return Int32.Parse(Environment.GetEnvironmentVariable("ASPORT"));
                }
                catch (FormatException e)
                {
                    Console.WriteLine($"Error while parsing ASPORT env var (Port set to 27017) Error : {e}");
                    return 27017;
                }
            }
        }

        public string connectionString
        {
            get
            {
                return Url.Replace("<user>", User)
                    .Replace("<password>", Password)
                    .Replace("<host>", Host)
                    .Replace("<port>", Port.ToString())
                    .Replace("<database>", Database);
            }
        }
    }
}
