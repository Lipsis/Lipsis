using System;
using System.Threading;
using System.Collections.Generic;


namespace Lipsis.Core {
    public unsafe class MessagePump {
        private thread[] p_Threads;
        private string p_Name;
        private bool p_Abort = true;
        private object p_SyncLock = new object();
        private int p_RunningThreads;
        private int p_ThreadCount;
        private int p_ThreadCurrentIndex = 0;

        public MessagePump(string name) {
            p_Name = name;
        }

        public void SendMessage(void* paramPtr, MessageHandler callback) { 
            //get a thread to send the message to
            thread thr;
            lock (p_SyncLock) {
                thr = p_Threads[p_ThreadCurrentIndex++];
                if (p_ThreadCurrentIndex == p_ThreadCount) {
                    p_ThreadCurrentIndex = 0;
                }
            }

            //send the message to the thread
            lock (thr.syncronizeLock) {
                thr.stackLength++;
                thr.stack.Push(new msg {
                    handler = callback,
                    paramPtr = paramPtr
                });
            }
        }

        public void Start() {
            Start(1);
        }
        public void Start(int threadCount) { 
            //already started?
            if (!p_Abort) { return; }

            //valid thread count
            if (threadCount <= 0) {
                throw new Exception("Invalid thread count " + threadCount);
            }

            //spawn the threads
            p_Abort = false;
            p_Threads = new thread[threadCount];
            p_ThreadCount = threadCount;
            p_ThreadCurrentIndex = 0;
            for (int c = 0; c < threadCount; c++) {
                thread t = new thread() {
                    pump = this,
                    stack = new Stack<msg>(),
                    syncronizeLock = new object()
                };
                t.nativeThread = new Thread(t.main) { 
                    Priority = ThreadPriority.Highest,
                    Name = p_Name + " [thread: " + c + "]"
                };
                t.nativeThread.Start();
                p_Threads[c] = t;
            }
        }

        public void Stop() { Stop(true); }
        public void Stop(bool wait) { 
            //already stopped?
            if (p_Abort) { return; }

            //set the flag to stop so all threads will abort on their
            //next cycles
            p_Abort = true;

            //wait?
            while (wait && p_RunningThreads != 0) ;
        }

        public string Name { get { return p_Name; } }
        public int RunningThreads { get { return p_RunningThreads; } }
        public int ThreadCount { get { return p_Threads.Length; } }
        public bool Running { get { return !p_Abort; } }

        public override string ToString() {
            return p_Name;
        }

        public delegate void MessageHandler(void* paramPtr);

        private class thread {
            public Thread nativeThread;
            public Stack<msg> stack;
            public MessagePump pump;
            public int stackLength;
            public object syncronizeLock;
           
            public void main() {
                pump.p_RunningThreads++;

                //keep iterating while the pump
                //is running.
                while (!pump.p_Abort) {
                    lock (syncronizeLock) { 
                        //anything to run?
                        if (stackLength == 0) {
                            Thread.Sleep(1);
                            continue;
                        }

                        //run the item at the top of the stack
                        msg message = stack.Pop();
                        message.handler(message.paramPtr);
                        stackLength--;
                    }
                }

                pump.p_RunningThreads--;
            }
        }
        private struct msg {
            public MessageHandler handler;
            public void* paramPtr;
        }
    }
}