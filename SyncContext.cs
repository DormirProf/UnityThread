using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SyncContext : MonoBehaviour
{
    public static TaskScheduler unityTaskScheduler;
    public static int unityThread;
    public static SynchronizationContext unitySynchronizationContext;
    static public Queue<Action> runInUpdate = new Queue<Action>();

    public static bool isOnUnityThread => unityThread == Thread.CurrentThread.ManagedThreadId;


    private void Awake()
    {
        unitySynchronizationContext = SynchronizationContext.Current;
        unityThread = Thread.CurrentThread.ManagedThreadId;
        unityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    }
    
    private void Update()
    {
        while (runInUpdate.Count > 0)
        {
            Action action = null;
            lock (runInUpdate)
            {
                if (runInUpdate.Count > 0)
                    action = runInUpdate.Dequeue();
            }

            action?.Invoke();
        }
    }
    
    public static void RunOnUnityThread(Action action)
    {
        if (unityThread == Thread.CurrentThread.ManagedThreadId)
        {
            action();
        }
        else
        {
            lock (runInUpdate)
            {
                runInUpdate.Enqueue(action);
            }

        }
    }
}
