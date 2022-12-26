using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Essentials.EventHandler.Table
{
    public static class EventTableExtensions
    {
        public static Dictionary<string, Func<string, List<dynamic>>?> QueryTable = new Dictionary<string, Func<string, List<dynamic>>?>()
        {
            {"TaskResults", QueryTaskResults },
            {"TaskStatus", QueryTaskStatus },
            {"AllocatedTasks", QueryAllocatedTasks }
        };

        public static List<dynamic> QueryAllocatedTasks(string possessor)
        {
            List<dynamic> tasks = new List<dynamic>();
            foreach (Tasks.DataTask task in Tasks.TaskRegistry.Tasks)
            {
                tasks.Add(task.Tag);
            }

            return tasks;
        }

        /// <summary>
        /// 0: ThreadRunning
        /// 1: Idling
        /// 2: Stopped
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static List<dynamic> QueryTaskStatus(string tag)
        {
            List<dynamic> status = new List<dynamic>();
            foreach(Tasks.DataTask task in Tasks.TaskRegistry.Tasks)
            {
                if(task.Tag == tag)
                {
                    status.Add(task.Running && task.CurrentThreads.Count > 0);
                    status.Add(!task.StopTask && !task.PauseTask && task.CurrentThreads.Count == 0);
                    status.Add(task.StopTask && task.CurrentThreads.Count == 0 || task.PauseTask);

                    return status;
                }
            }
            return null;
        }

        public static List<dynamic> QueryTaskResults(string tag)
        {
            List<dynamic> results = new List<dynamic>();

            foreach (Tasks.DataTask task in Tasks.TaskRegistry.Tasks)
            {
                if (task.Tag == tag)
                {
                    results = task.Results.ToList();
                    return results;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets Paused/Stopped&0Threads Tasks
        /// </summary>
        /// <returns></returns>
        public static List<Tasks.DataTask> GetStopped()
        {
            List<Tasks.DataTask> stopped = new List<Tasks.DataTask>();
            foreach(Tasks.DataTask task in Tasks.TaskRegistry.Tasks)
            {
                if(task.StopTask && task.CurrentThreads.Count == 0 || task.PauseTask)
                {
                    stopped.Add(task);
                }

            }

            return stopped;
        }

        public static List<Tasks.DataTask>? GetIdle()
        {
            List<Tasks.DataTask> active = new List<Tasks.DataTask>();
            foreach (Tasks.DataTask task in Tasks.TaskRegistry.Tasks)
            {
                if (!task.StopTask && !task.PauseTask && task.CurrentThreads.Count == 0)
                {
                    active.Add(task);
                }

            }

            return active;
        }
        public static List<Tasks.DataTask> GetActive()
        {
            List<Tasks.DataTask> active = new List<Tasks.DataTask>();
            foreach (Tasks.DataTask task in Tasks.TaskRegistry.Tasks)
            {
                if (task.Running && task.CurrentThreads.Count > 0)
                {
                    active.Add(task);
                }

            }

            return active;
        }
    }
}
