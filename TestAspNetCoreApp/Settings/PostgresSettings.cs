namespace TestAspNetCoreApp.Settings
{
    public record PostgresSettings : IDatabaseSettings
    {
        public string Host { get; set; }

        public string Port { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

        public string Database { get; set; }

        public string ConnectionString
        {

            //"Host=localhost;Port=5432;Database=Departments;Username=postgres;Password=3343"
            //Host=localhost;Port=5432;Database=;Username=postgres;Password=3343

            get
            {
                return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
            }
        }

    }
}
