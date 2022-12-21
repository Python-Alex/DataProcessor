using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Essentials.Plugins
{
    public static class PluginBus
    {
        public static bool BusEnabled = false;
        public static Thread? BusThread = null;

        public static Dictionary<Plugin, DateTime> BusDelays = new Dictionary<Plugin, DateTime>();

        public static void EnableBus() {
            if (BusThread != null)
                return;

            Thread bus_thread = new Thread(() => ExecuteEvents());
            BusThread = bus_thread;

            BusEnabled = true;

            BusThread.Start();
        }

        // func should be threaded
        public static void ExecuteEvents()
        {
            while(BusEnabled)
            {
                foreach(Plugin plugin in PluginLoader.LoadedPlugins)
                {

                    int plugin_result = plugin.Event();
                    
                    if(plugin.EventResults.Count >= Properties.PluginMaxEventResultsFramed)
                    {
                        
                        plugin.EventResults.RemoveAt(0);
                        plugin.EventResults.Add(plugin_result);
                    }

                    Thread.Sleep(Properties.PluginBusMinSleep);
                }
            }
        }
    }
}
