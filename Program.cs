using System.Threading;
using DataProcessor.Logging;
using DataProcessor.Essentials;
using DataProcessor.Essentials.Plugins;
using DataProcessor.Essentials.Network.Entities;
public static class Program
{
    public static ListenHandler Listener = null;

    public static void Main(string[] args)
    {
        Logging.Info("Program", "Starting Up Essentials");

        Properties.LoadProperties();

        PluginBus.EnableBus();

        Listener = new ListenHandler();
        Listener.Start();
        

        PluginLoader.LoadedPlugins.Capacity = Properties.MaxAllowedPlugins;
        PluginLoader.AbstractPlugins.Capacity = Properties.MaxAllowedPlugins;
           

        Logging.Info("Program", String.Format($"Loaded {PluginLoader.LoadedPlugins.Count} Plugins"));
        
    }
}