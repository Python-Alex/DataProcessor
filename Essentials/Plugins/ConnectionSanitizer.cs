using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using DataProcessor.Essentials.Network;
using DataProcessor.Essentials.Network.Entities;


namespace DataProcessor.Essentials.Plugins
{
    public class ConnectionSanitizer : Plugin
    {

        
        public ConnectionSanitizer(string name, string[] description, string version)
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
            } catch (Exception ex)
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
            List<ConnectionThread> copyThreads = Manager.Instances.ToList();

            foreach (ConnectionThread thread in copyThreads)
            {
                Socket? connection = thread.GetConnection();
                if(connection == null || !thread.IsAlive)
                {
                    Logging.Logging.Info("Plugin." + Name, String.Format($"Detected Inactive Connection for Thread[{thread.GetHashCode()}]"));
                    Manager.RemoveInstance(thread);
                }
                else if(thread.ErroredOut())
                {
                    Logging.Logging.Error("Plugin." + Name, String.Format($"Removing Errored Thread"), thread.GetException());
                    Manager.RemoveInstance(thread);
                }
            }


            return 0;
        }
    }
}
