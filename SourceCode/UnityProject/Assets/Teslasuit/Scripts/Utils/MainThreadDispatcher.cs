using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace TeslasuitAPI
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private Thread MainThread;

        private Queue<IEnumerator> coroutinesQueue = new Queue<IEnumerator>();
        private Queue<Action> actionsQueue = new Queue<Action>();
        private Queue<Action<object>> actionsArg1Queue = new Queue<Action<object>>();
        private Queue<object> args1Queue = new Queue<object>();

        private void Awake()
        {
            MainThread = Thread.CurrentThread;
            StartCoroutine(Loop());
        }

        private IEnumerator DoInMainThreadRoutineTimed(Action action, float time = 0.0f)
        {
            if (time > 0.0f)
                yield return new WaitForSeconds(time);
            action();
        }

        private IEnumerator Loop()
        {
            while (true)
            {
                while (coroutinesQueue.Count > 0)
                {
                    StartCoroutine(coroutinesQueue.Dequeue());
                }
                while (actionsArg1Queue.Count > 0)
                {
                    lock (lockObject)
                    {
                        actionsArg1Queue.Dequeue()(args1Queue.Dequeue());
                    }
                }
                while (actionsQueue.Count > 0)
                {
                    actionsQueue.Dequeue()();
                }
                yield return null;
            }
        }


        public static MainThreadDispatcher Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<MainThreadDispatcher>();
            }
            if (Instance == null)
            {
                Instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(Instance.gameObject);
            }
        }



        public static void Execute(Action action)
        {
            if(Thread.CurrentThread == Instance.MainThread)
            {
                action();
                return;
            }
           
            lock (lockObject)
            {
                Instance.actionsQueue.Enqueue(action);
            }
        }

        public static void Execute(Action action, float time)
        {
            lock (lockObject)
            {
                Instance.coroutinesQueue.Enqueue(Instance.DoInMainThreadRoutineTimed(action, time));
            }
        }

        public static void Execute(Action<object> action, object opaque)
        {
            if (Thread.CurrentThread == Instance.MainThread)
            {
                action(opaque);
                return;
            }
            lock (lockObject)
            {
                Instance.actionsArg1Queue.Enqueue(action);
                Instance.args1Queue.Enqueue(opaque);
            }
        }

        private static object lockObject = new object();
    } 
}