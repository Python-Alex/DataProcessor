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
    public class ObjectDisposer : Plugin
    {
        public ObjectDisposer(string name, string[] description, string version)
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
            List<EventHandler.Tasks.DataTask> Tasks = EventHandler.Tasks.TaskRegistry.Tasks.ToList();
            foreach (EventHandler.Tasks.DataTask task in Tasks)
            {
                if(task.Dispose)
                {
                    task.Running = false;
                    EventHandler.Tasks.TaskRegistry.Tasks.Remove(task);
                }
            }

            Thread.Sleep(EventDelay);


            return 0;
        }
    }
}
