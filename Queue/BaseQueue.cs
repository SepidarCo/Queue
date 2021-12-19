using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Queue
{
    public class BaseQueue
    {
        protected string QueueAddress { get; set; } = "";

        private static ConcurrentDictionary<string, string> CurrentServerIps = new ConcurrentDictionary<string, string>();

        public BaseQueue(string queueAddress)
        {
            // QueueAddress = FillQueueName(queueAddress);
            QueueAddress = queueAddress;

            FillIps();

            if (!MessageQueue.Exists(QueueAddress))
            {
                MessageQueue.Create(QueueAddress, false);
            }
        }

        public bool PushToQueue<T>(T dto, int retryCount)
        {
            for (int i = 0; i < retryCount; i++)
            {
                if (PushToQueue(dto))
                {
                    return true;
                }
            }
            return false;
        }

        private bool PushToQueue<T>(T dto)
        {
            var result = false;

            try
            {
                MessageQueue queue = new MessageQueue(QueueAddress);
                queue.Formatter = new BinaryMessageFormatter();
                queue.Send(dto);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = false;
            }

            return result;
        }

        private string FillQueueName(string queueAddress)
        {
            var splited = queueAddress.Split(',');

            if (splited[0] == "." ||
                splited[0].Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase) ||
                CurrentServerIps.ContainsKey(splited[0]))
            {
                return @".\Private$\" + splited[1];
            }
            else
                return "FormatName:DIRECT=TCP:" + splited[0] + @"\private$\" + splited[1];

        }

        private void FillIps()
        {
            if (CurrentServerIps.Keys.Count == 0)
            {
                lock (CurrentServerIps)
                {
                    if (CurrentServerIps.Keys.Count == 0)
                    {
                        var host = Dns.GetHostEntry(Dns.GetHostName());
                        foreach (var ip in host.AddressList)
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetwork)
                            {
                                CurrentServerIps.AddOrUpdate(ip.ToString(), ip.ToString(), (key, oldvalue) => ip.ToString());
                            }
                        }
                    }
                }
            }
        }

        //private object Subscriber()
        //{
        //    var result = false;
        //    try
        //    {
        //        var queue = new MessageQueue(QueueAddress)
        //        {
        //            Formatter = new BinaryMessageFormatter()
        //        };

        //        queue.ReceiveCompleted += QueueReceiveCompleted;
        //        queue.BeginReceive();
        //        return result == true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return result;
        //    }
        //}

        //private void QueueReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        //{
        //    var msg = (string)e.Message.Body;

        //     JsonConvert.DeserializeObject<Message>(msg);
        //}
    }
}
