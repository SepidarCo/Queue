using Newtonsoft.Json;
using Queue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;

namespace Producer
{
    public class QueueListener : BaseQueue
    {
        private readonly MessageQueue queue;
        private readonly FailedQueue failedQueue;
        public static ConcurrentQueue<Person> concurrentQueue = new ConcurrentQueue<Person>();
        CancellationTokenSource cancellationToken = new CancellationTokenSource();


        public QueueListener(string queueAddress) : base(queueAddress)
        {
            queue = new MessageQueue(queueAddress);
            queue.Formatter = new BinaryMessageFormatter();
            queue.ReceiveCompleted += Queue_ReceiveCompleted;
            failedQueue = new FailedQueue(queueAddress + "failed");
        }

        public void Start()
        {
            StartListening(Queue_ReceiveCompleted);
        }

        private void StartListening(ReceiveCompletedEventHandler Queue_ReceiveCompleted)
        {
            queue.BeginReceive();
        }

        public void StopListening()
        {
            cancellationToken.Cancel();
            queue.Dispose();
        }

        private void Queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var queue = (MessageQueue)sender;
            try
            {
                var m = queue.EndReceive(e.AsyncResult);
                var receivedObject = JsonConvert.DeserializeObject<Person>(e.Message.Body.ToString());
                PersonHandler.Instnace.HandlePerson(receivedObject);
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                    Console.WriteLine(ex.Message);

                failedQueue.SendToQueue((string)e.Message.Body);
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                queue.BeginReceive();
            }
        }

        public void Set()
        {
            for (int i = 0; i < 1000; i++)
            {
                var temp = new Person(1, "Ali", "Fallah");
                var model = JsonConvert.SerializeObject(temp);
                PushToQueue(model, 1);

                var temp2 = new Person(2, "a", "b");
                var model2 = JsonConvert.SerializeObject(temp2);
                PushToQueue(model2, 1);

                var temp3 = new Person(3, "c", "d");
                var model3 = JsonConvert.SerializeObject(temp3);
                PushToQueue(model3, 1);

            }
        }
    }
}
