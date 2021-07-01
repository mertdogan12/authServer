using System;

namespace authServer.Settings
{
    public class MongoDbSettings
    {
        public string Host
        {
            get
            {
                return Environment.GetEnvironmentVariable("ASHOST");
            }
        }

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

        public string Database
        {
            get
            {
                return Environment.GetEnvironmentVariable("ASDATABASE");
            }
        }

        public string User
        {
            get
            {
                return Environment.GetEnvironmentVariable("ASUSER");
            }
        }

        public string Password
        {
            get
            {
                return Environment.GetEnvironmentVariable("ASPW");
            }
        }

        public string Url
        {
            get
            {
                return Environment.GetEnvironmentVariable("ASURL");
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
