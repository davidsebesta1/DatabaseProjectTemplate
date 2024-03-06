namespace DatabaseProjectTemplate.Configuration
{
    public static class ConfigHandler
    {
        public static readonly string LocalConfigPath = "Config/config.yaml";
        public static readonly string AbsoluteConfigPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(LocalConfigPath));
        private static Config _config;

        public static Config Config
        {
            get
            {
                if (_config == null)
                {
                    LoadConfig();
                }

                return _config;
            }
        }

        public static void LoadConfig()
        {
            if (!Path.Exists(AbsoluteConfigPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(AbsoluteConfigPath));

                if (!File.Exists(AbsoluteConfigPath))
                {
                    _config = new Config();
                    SaveConfig();
                }

                _config = YamlParser.Deserializer.Deserialize<Config>(AbsoluteConfigPath);
            }
        }

        public static void SaveConfig()
        {
            if (_config == null)
            {
                throw new InvalidOperationException("Unable to save null config");
            }

            if (!Path.Exists(AbsoluteConfigPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(AbsoluteConfigPath));
            }

            File.WriteAllText(AbsoluteConfigPath, YamlParser.Serializer.Serialize(_config));
        }
    }
}
