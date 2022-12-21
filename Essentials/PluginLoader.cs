using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Essentials
{
    public abstract class Plugin
    {
        public string? Name;
        public string[]? Description;
        public string? Version;
        public int EventDelay;
        public List<int> EventValues;
        public List<int> EventResults;

        public abstract int Event();

    }

    public static class PluginLoader
    {
        public static List<Plugin> LoadedPlugins = new List<Plugin>();
        public static List<Plugin> AbstractPlugins = new List<Plugin>()
        {
            new Plugins.ConnectionSanitizer("ConnectionSanitizer", new string[]{"Sanitizes Dirty Connections, Poor Headers, Low Security Levels",
                "Handles Timeout Callbacks"}, "0.0.1"),
            new Plugins.KeepAlive("KeepAlive", new string[]{"Persists Connections to Keepalive", "Emits Packets per Interval"}, "0.0.1"),
            new Plugins.ObjectDisposer("ObjectDisposer", new string[]{"Deletes Relative Objects from Corresponding Lists upon Disposal Detection" }, "0.0.01"),
        };
    }
}
