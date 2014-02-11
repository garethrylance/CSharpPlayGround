using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Scratch
{
    public class OnDemandQueueProcessor
    {
        /*
         * http://twistedoakstudios.com/blog/Post2061_emulating-actors-in-c-with-asyncawait
      * The above code manages running the consume method via the _pendingCount field. 
      * The _pendingCount field is atomically incremented after enqueuing actions, and atomically decremented
      * after dequeuing (and invoking) actions. Initially, exactly one producer will be the one to increment 
      * _pendingCount from 0 to 1. They’re in charge of triggering the start of consumption. 
      * The consumer can only decrement from 1 to 0 if no more actions have finished being enqueued. 
      * It’s possible for an action to be in the queue when the decrement-to-0 happens, 
      * but only if there’s a producer about to increment from 0 to 1 and re-trigger consumption! 
      * The consumer can just stop when it looks like the queue is empty, even if it’s not!
        */



        private readonly ConcurrentQueue<Object> _inbox = new ConcurrentQueue<object>();
        private int _pendingCount;
        private int _isActive = 1;
        private readonly Action<Object> _onMessageAction;

        public OnDemandQueueProcessor(Action<Object> onMessageAction)
        {
            _onMessageAction = onMessageAction;
        }


        public void Post(Object message)
        {
            _inbox.Enqueue(message);
            if (Interlocked.Increment(ref _pendingCount) == 1 && _isActive == 1)
            {
                StartProcessing();
            }
        }


        public void StopProcessing()
        {
            Interlocked.Decrement(ref _isActive);
        }


        private void StartProcessing()
        {
            Task.Factory.StartNew((x) =>
            {
                int i = 0;
                do
                {
                    Object message;
                    if (_inbox.TryDequeue(out message))
                    {
                        _onMessageAction(message);
                    }
                    else
                    {
                        throw new Exception("Should not get here");
                    }
                    i++;
                } while (Interlocked.Decrement(ref _pendingCount) > 0 && _isActive == 1);

                return i;

            }, null);
        }
    }
}