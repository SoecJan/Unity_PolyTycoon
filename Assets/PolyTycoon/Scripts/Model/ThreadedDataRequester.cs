using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadedDataRequester {

    static ThreadedDataRequester _instance;
    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();
    private static List<Thread> _runningThreads;

    public ThreadedDataRequester()
    {
        _instance = this;
        _runningThreads = new List<Thread>();
    }

    public static void RequestData(Func<object> generateData, params Action<object>[] callback) {
        ThreadStart threadStart = delegate {
            _instance.DataThread(generateData, callback);
        };
        
        Thread thread = new Thread(threadStart);
        thread.Start();
        _runningThreads.Add(thread);
    }

    void DataThread(Func<object> generateData, params Action<object>[] callback) {
        object data = generateData();
        lock (dataQueue) {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    public void Update()
    {
        if (dataQueue.Count == 0) return;
        for (int i = 0; i < dataQueue.Count; i++) {
            ThreadInfo threadInfo = dataQueue.Dequeue();
            if (threadInfo.callback == null) continue;
            foreach (Action<object> callback in threadInfo.callback)
            {
                callback(threadInfo.parameter);
            }
        }
    }

    public void OnDestroy()
    {
        foreach (Thread thread in _runningThreads)
        {
            thread.Abort();
        }
    }

    struct ThreadInfo {
        public readonly Action<object>[] callback;
        public readonly object parameter;

        public ThreadInfo(Action<object>[] callback, object parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}