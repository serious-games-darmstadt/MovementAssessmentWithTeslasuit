using System;
using TeslasuitAPI.Utils;

namespace TeslasuitAPI
{
    public class BufferedQueueDispatcher<T1>
    {
        private BufferedQueue<T1> t1Queue;

        public int BufferSize { get; private set; }

        public BufferedQueueDispatcher(int bufferSize)
        {
            this.BufferSize = bufferSize;
            t1Queue = new BufferedQueue<T1>(4096, this.BufferSize);
        }

        public void Add(T1 a)
        {
            t1Queue.Enqueue(a);
        }

        public void Dispatch(Action<T1[], int> task)
        {
            if (t1Queue.Count == 0) return;

            var buffer = t1Queue.DequeueBuffer();

            task(buffer.Key, buffer.Value);
        }
    }

    public class BufferedQueueDispatcher<T1, T2>
    {
        private BufferedQueue<T1> t1Queue;
        private BufferedQueue<T2> t2Queue;

        public int BufferSize { get; private set; }

        public BufferedQueueDispatcher(int bufferSize)
        {
            this.BufferSize = bufferSize;
            t1Queue = new BufferedQueue<T1>(4096, this.BufferSize);
            t2Queue = new BufferedQueue<T2>(4096, this.BufferSize);
        }

        public void Add(T1 a, T2 b)
        {
            t1Queue.Enqueue(a);
            t2Queue.Enqueue(b);
        }

        public void Dispatch(Action<T1[], int, object> task)
        {
            if (t1Queue.Count == 0) return;

            var buffer = t1Queue.DequeueBuffer();
            var opaques = t2Queue.DequeueBuffer();

            task(buffer.Key, buffer.Value, opaques);
        }
    }


}

