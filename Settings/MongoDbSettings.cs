using System;

namespace authServer.Settings
{
    public class MongoDbSettings
    {
        public string Host = Environment.GetEnvironmentVariable("ASHOST").ToString();
        public string Database = Environment.GetEnvironmentVariable("ASDATABASE").ToString();
        public string User = Environment.GetEnvironmentVariable("ASUSER").ToString();
        public string Password = Environment.GetEnvironmentVariable("ASPW").ToString();
        public string Url = Environment.GetEnvironmentVariable("ASURL").ToString();

        public int Port
        {
            get
            {
                try
                {
                    return Int32.Parse(Environment.GetEnvironmentVariable("ASPORT").ToString());
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
