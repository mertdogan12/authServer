namespace authServer.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }

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
