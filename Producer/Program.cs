using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static List<QueueListener> listListener = new List<QueueListener>();
        public static void SetInQueue()
        {
            var queueAdderess = ConfigurationManager.AppSettings["PeopleQueueAddress"];
            var listener = new QueueListener(queueAdderess);
            listener.Set();
        }

        public static void ListenFromQueue(int count = 1)
        {
            var queueAdderess = ConfigurationManager.AppSettings["PeopleQueueAddress"];
            PersonHandler.Init(1000);

            for (int i = 0; i < count; i++)
            {
                var listener = new QueueListener(queueAdderess);
                listListener.Add(listener);
                listener.Start();
            }
        }

        static void Main(string[] args)
        {
            SetInQueue();
            ListenFromQueue();
        }
    }
}
