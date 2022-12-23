using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DataProcessor.Essentials.Plugins
{

    public class ExamplePlugin : Plugin
    {
        public ExamplePlugin(string name, string[] description, string version)
        {
            Name = name;
            Description = description;
            Version = version;

            try
            {
                if (PluginLoader.LoadedPlugins.Count<Plugin>() >= Properties.MaxAllowedPlugins)
                {
                    throw new Exception("Max Plugins Exceeded");
                }
                else
                {
                    PluginLoader.LoadedPlugins.Add(this);
                }
            }
            catch (Exception ex)
            {
                Logging.Logging.Error("PluginLoader." + name, "Plugin Failed Loading", ex);
                return;
            }
            Logging.Logging.Info("PluginLoader." + name, String.Format($"Plugin Loaded | Version: ({Version})"));
            EventDelay = Properties.PluginBusMinSleep;

            EventValues = new List<int>();
            EventResults = new List<int>();

        }

        public override int Event()
        {
            // do things here

            Thread.Sleep(EventDelay);
            return 0;
        }
    }
}
