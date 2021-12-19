using System;
using System.Messaging;

namespace Queue
{
    public sealed class FailedQueue
    {
        private readonly MessageQueue failedQueue;

        public FailedQueue(string queuePath)
        {
            failedQueue = new MessageQueue(queuePath);
            failedQueue.Formatter = new JsonMessageFormatter();
        }

        public void SendToQueue(string message)
        {
            try
            {
                failedQueue.Send(message);
            }

            catch (Exception ex)
            {
                //todo: log message
            }
        }
    }
}
