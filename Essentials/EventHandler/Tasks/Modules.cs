using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Essentials.EventHandler.Tasks
{
    public class DataTask
    {
        public string Tag;
        public string ModuleName;

        private Func<Network.Data.FeedTask, int>? Module;

        public bool PauseTask;
        public bool StopTask;
        public bool VerboseOutput;

        public bool Running;

        public bool Dispose;

        // Initialized by Entities.ConnectionThread.HandleData
        public DataTask(string Tag, string ModuleName)
        {
            if (!Modules.Lookup.ContainsKey(ModuleName))
            {
                throw new Exception("ModuleLookup Failure");
            }
            if (!TaskRegistry.ContainsTag(Tag))
            {
                throw new Exception("TaskRegistry Contains Tag");
            }

            this.Tag = Tag;
            this.ModuleName = ModuleName;

            Module = null;

            PauseTask = false;
            StopTask = false;
            VerboseOutput = false;

            Running = false;
        
            Dispose = false;
        }

        public DataTask(Network.Data.InitializeTask initializer)
        {
            if (!Modules.Lookup.ContainsKey(initializer.ModuleName))
            {
                throw new Exception("ModuleLookup Failure");
            }
            if(!TaskRegistry.ContainsTag(initializer.Tag))
            {
                throw new Exception("TaskRegistry Contains Tag");
            }
            
            this.Tag = initializer.Tag;
            this.ModuleName = initializer.ModuleName;

            Module = null;

            PauseTask = false;
            StopTask = false;
            VerboseOutput = false;

            Running = false;

            Dispose = false;
        }

        public Func<Network.Data.FeedTask, int>? GetModule() => Module;
    
        public int Execute(Network.Data.FeedTask input)
        {
            if (StopTask && !Running)
            {
                Running = false;
                return 1;
            }
            else if(PauseTask && !Running)
            {
                Running = false;
                return -1;
            }
            Running = true;

            if(Module != null)
            {
                Module(input);
            }

            return 0;
        }
    }

    public static class TaskRegistry
    {
        public static List<DataTask> Tasks = new List<DataTask>();

        public static bool ContainsTag(string Tag)
        {
            foreach(DataTask task in Tasks)
            {
                if (task.Tag == Tag)
                    return true;
            }

            return false;
        }

        public static DataTask? GetTask(string Tag)
        {
            foreach(DataTask dataTask in Tasks)
            {
                if (dataTask.Tag == Tag)
                    return dataTask;
            }

            return null;
        }
    }

    public static class Modules
    {
        public static Dictionary<string, Action> Lookup = new Dictionary<string, Action>()
        {

        };

    }
}
