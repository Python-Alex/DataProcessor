using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataProcessor.Logging;
using Newtonsoft.Json;

namespace DataProcessor.Essentials
{
    public class JsonProperties
    {
        public int MaxAllowedPlugins;
        public int MaxAllowedConnections;
        public int NotifyKeepAliveInterval;
        public string BindAddress;
        public int BindPort;
        public int ClientReceiveTimeout;
        public int ClientSendTimeout;
        public int PluginBusMinSleep;
        public int PluginMaxEventResultsFramed;
    }

    public static class Properties
    {
        // these values are overrode
        public static int MaxAllowedPlugins = 32;
        public static int MaxAllowedConnections = 128;
        public static int NotifyKeepAliveInterval = 30;
        public static string BindAddress = "127.0.0.1";
        public static int BindPort = 65500;
        public static int ClientReceiveTimeout = 300;
        public static int ClientSendTimeout = 300;
        public static int PluginBusMinSleep = 100;
        public static int PluginMaxEventResultsFramed = 4096;

        public static void LoadProperties()
        {
            string property_json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "properties.json"));

            JsonProperties? json_properties = JsonConvert.DeserializeObject<JsonProperties>(property_json);

            if (json_properties == null)
            {
                Logging.Logging.Error("PropertyLoader", "Failed to Deserialize Properties Properly", new Exception("Non-Fatal; Attempting Possible Null Assignment"));
            }
            else
            {
                try
                {
                    
                    MaxAllowedPlugins = json_properties.MaxAllowedPlugins;
                    MaxAllowedConnections = json_properties.MaxAllowedConnections;
                    BindAddress = json_properties.BindAddress;
                    BindPort = json_properties.BindPort;
                    ClientReceiveTimeout = json_properties.ClientReceiveTimeout;
                    ClientSendTimeout = json_properties.ClientSendTimeout;
                    PluginBusMinSleep = json_properties.PluginBusMinSleep;
                    PluginMaxEventResultsFramed = json_properties.PluginMaxEventResultsFramed;
                }
                catch (System.NullReferenceException null_reference)
                {
                    Logging.Logging.Error("PropertyLoader", "Fatal; Null Assignment Occured", null_reference);
                }
            }
            Logging.Logging.Info("PropertyLoader", "Loaded Properties Data");
           
        }
    }
}
