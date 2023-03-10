using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

namespace DataProcessor.Essentials.EventHandler.Tasks
{
    public class DataTask
    {
        public string Tag;
        public string ModuleName;

        private Func<DataTask, Network.Data.Receiving.FeedTask, int>? Module;

        public bool PauseTask;
        public bool StopTask;
        public bool VerboseOutput;

        public bool Running;

        public bool Dispose;

        public List<Thread> CurrentThreads;
        public List<dynamic> Results;

        // Initialized by Entities.ConnectionThread.HandleData
        public DataTask(string Tag, string ModuleName)
        {
            if (!Modules.Lookup.ContainsKey(ModuleName))
            {
                throw new Exception("ModuleLookup Failure");
            }
            if (TaskRegistry.ContainsTag(Tag))
            {
                throw new Exception("TaskRegistry Contains Tag");
            }

            this.Tag = Tag;
            this.ModuleName = ModuleName;

            if(!Modules.Lookup.TryGetValue(ModuleName, out Module))
                Module = null;

            PauseTask = false;
            StopTask = false;
            VerboseOutput = false;

            Running = false;
        
            Dispose = false;

            CurrentThreads = new List<Thread>();
            Results = new List<dynamic>();
        }

        public DataTask(Network.Data.Receiving.InitializeTask initializer)
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

            if (!Modules.Lookup.TryGetValue(ModuleName, out Module))
                Module = null;

            PauseTask = false;
            StopTask = false;
            VerboseOutput = false;

            Running = false;

            Dispose = false;

            CurrentThreads = new List<Thread>();
            Results = new List<dynamic>();
        }

        public Func<DataTask, Network.Data.Receiving.FeedTask, int>? GetModule() => Module;
        public bool IsWorking() => CurrentThreads.Count > 0;
    
        public int Execute(Network.Data.Receiving.FeedTask input)
        {
            Logging.Logging.Info("DataTask.Executer", String.Format($"Executing Module {ModuleName}"));
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
                Module(this, input);
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
        public static Dictionary<string, Func<DataTask, Network.Data.Receiving.FeedTask, int>> Lookup = new Dictionary<string, Func<DataTask, Network.Data.Receiving.FeedTask, int>>()
        {
            {"ImageRatioCompare",  ImageRatioCompare },

        };

        /// <summary>
        /// Internal Function for Module, Returns a New Resized Nested List
        /// </summary>
        /// <param name="area"></param>
        /// <param name="matching"></param>
        /// <returns></returns>
        static internal List<List<int>> Resize3DArray(List<List<int>> area, List<List<int>> matching)
        {
            List<List<int>> varea = new List<List<int>>();
            for (int i = 0; i < matching.Count; i++)
            {
                varea.Add(area[i].GetRange(0, matching[i].Count));
            }

            return varea;
        }

        /// <summary>
        /// Internal Function for Modules, Returns Smallest Nested List, If Multiple of the Same Smallest Size, Last is Returned
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        static internal List<List<int>> Smallest3DArray(List<List<List<int>>> areas)
        {
            List<List<int>> smallest = null;
            for (int i = 0; i < areas.Count; i++)
            {   // List<List<int>> area
                if(smallest == null)
                {
                    smallest = areas[i];
                }

                if(areas[i].Count <= smallest.Count && areas[i][0].Count <= smallest[0].Count)
                {
                    smallest = areas[i];
                }
            }

            return smallest;
        }

        /// <summary>
        /// Returns Decimal Ratio of Comparison between image A and image B
        /// </summary>
        /// <param name="dataTask"></param>
        /// <param name="feedTask"></param>
        /// <returns></returns>
        public static int ImageRatioCompare(DataTask dataTask, Network.Data.Receiving.FeedTask feedTask)
        {
            Thread current = Thread.CurrentThread;
            if (!dataTask.CurrentThreads.Contains(current))
            {
                Logging.Logging.Info("ModuleOutput.ImageRatioCompare", "Thread does not belong to passed DataTask");
            }

            List<List<int>> image_a = feedTask.Arguments["A"].ToObject<List<List<int>>>();
            List<List<int>> image_b = feedTask.Arguments["B"].ToObject<List<List<int>>>();

            if (image_a == null || image_b == null)
            {
                Logging.Logging.Info("ModuleOutput.ImageRatioCompare", "An image was null");
            }
            if (image_a.Count != image_b.Count || image_a[0].Count != image_b[0].Count)
            {

                if (image_a.Count > image_b.Count || image_a[0].Count > image_b[0].Count)
                    image_a = Resize3DArray(image_a, image_b);
                if (image_b.Count > image_a.Count || image_b[0].Count > image_a[0].Count)
                    image_b = Resize3DArray(image_b, image_a);

            }

            float Size = image_a.Count * image_b[0].Count;
            float Matched = 0;

            for (int y = 0; y < image_a.Count; y++)
            {
                for (int x = 0; x < image_a[0].Count; x++)
                {
                    if (image_a[y][x] == image_b[y][x])
                    {
                        Matched++;
                    }
                }
            }

            float ComparisonRate = (Matched / Size) * 100f;
            Logging.Logging.Info("ModuleOutput.ImageRatioCompare", String.Format($"Compared Results -> {ComparisonRate}%"));

            dataTask.CurrentThreads.Remove(current);
            dataTask.Results.Add(ComparisonRate);
            return 0;
        }

    }
}
