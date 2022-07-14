using System.IO;
using Newtonsoft.Json;

namespace L_Commander.App.Infrastructure
{
    public interface ISettingsProvider
    {
        void Save(ClientSettings settings);

        ClientSettings Get();
    }

    public sealed class ClientSettingsProvider : ISettingsProvider
    {
        private const string ClientSettingsFileName = "ClientSettings.json";

        public void Save(ClientSettings settings)
        {
            var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(ClientSettingsFileName, settingsJson);
        }

        public ClientSettings Get()
        {
            if (!File.Exists(ClientSettingsFileName))
                return null;

            return JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText(ClientSettingsFileName));
        }
    }
}
