using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Producer
{
    public class PersonHandler
    {
        private static ConcurrentQueue<Person> personQueue = new ConcurrentQueue<Person>();
        private static PersonHandler instance = null;
        private static readonly object instanceLock = new object();
        private Thread thread = null;
        private int sleepTime = 500;

        public static PersonHandler Instnace
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new PersonHandler();
                        }
                    }
                }
                return instance;
            }
        }

        public PersonHandler()
        {
            thread = new Thread(new ThreadStart(ThreadCalculator));
            thread.Start();
        }

        public static void Init(int sleepTime)
        {
            var instance = PersonHandler.Instnace;
            instance.sleepTime = sleepTime;
        }

        public void HandlePerson(Person receivedObject)
        {
            personQueue.Enqueue(receivedObject);
        }

        private void ThreadCalculator()
        {
            while (true)
            {
                if (personQueue.Count > 0)
                {
                    int count = Math.Min(1000, personQueue.Count);
                    var list = new List<Person>(count);
                    for (int i = 0; i < count; i++)
                    {
                        personQueue.TryDequeue(out var person);
                        list.Add(person);
                    }
                    PrintPerson(list);
                }
                Thread.Sleep(5000);
            }
        }

        private void PrintPerson(List<Person> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item.Name + " " + item.Family);
            }
        }
    }
}
