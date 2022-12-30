using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using DataProcessor.Essentials.Network.Entities;

namespace DataProcessor.Essentials.Network
{
    public static class Manager
    {
        public static List<ConnectionThread> Instances = new List<ConnectionThread>();
        public static bool LockAccess = false;
        public static bool InsertInstance(ConnectionThread instance)
        {

            if (GetInstance(obj => obj.GetConnection() == instance.GetConnection()) != null)
            {
                return false;
            }
            else
            {
                Instances.Add(instance);
                return true;
            }
        }

        public static void SetLock()
        {
            if(LockAccess)
            {
                LockAccess = false;
            }
            else
            {
                LockAccess = true;
            }
        }

        public static void WaitAndSet()
        {
            while(LockAccess)
            {
                Thread.Sleep(10);
            }
            if(!LockAccess)
                SetLock();
        }

        public static bool RemoveInstance(ConnectionThread instance)
        {
            WaitAndSet();

            if(Instances.Contains(instance))
            {
                Instances.Remove(instance);
                SetLock();
                return Instances.Contains(instance) == false;
            }
            else
            {
                SetLock();
                return true;
            }
        }

        public static ConnectionThread? GetInstance(Func<ConnectionThread, bool> expression)
        {
           foreach (ConnectionThread thread in Instances)
           {
               bool condition = expression(thread);
               if(condition)
               {
                   return thread;
               }
           }

            return null;
        }

    }
}
