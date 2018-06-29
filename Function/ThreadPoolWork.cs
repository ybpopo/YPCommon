using System;
using System.Collections.Generic;
using System.Threading;

namespace YPCommon.Function
{
    public class ThreadPoolWork : IDisposable
    {
        static ThreadPoolWork()
        {
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.SetMinThreads(500, 500);
        }

        public ThreadPoolWork()
        {
            this.Finish = false;
        }

        public bool Finish { get; set; }

        public Exception Error { get; set; }


        public static void WaitFinish(List<ThreadPoolWork> list)
        {
            if (list == null || list.Count <= 0) return;
            bool wait = true;
            int totalCount = 0;
            lock (list)
            {
                totalCount = list.Count;
            }

            while (wait)
            {
                lock (list)
                {
                    list.RemoveAll(s => s.Finish);
                    if (totalCount != list.Count) break;
                }
                Thread.Sleep(500);
            }
        }

        public static void WaitAll(List<ThreadPoolWork> list)
        {
            if (list == null || list.Count <= 0) return;
            bool wait = true;
            while (wait)
            {
                wait = false;
                lock (list)
                {
                    foreach (var item in list)
                    {
                        if (!item.Finish)
                        {
                            wait = true;
                            Thread.Sleep(500);
                            break;
                        }
                    }
                }

            }
        }

        public ThreadPoolWork Work(ThreadStart start)
        {
            ThreadPool.QueueUserWorkItem((object obj) =>
            {
                try
                {
                    start();
                }
                catch (Exception ex)
                {
                    Error = ex;
                }
                finally
                {
                    Finish = true;
                }
            }, null);
            return this;
        }

        public ThreadPoolWork Work(WaitCallback callBack, object state)
        {
            ThreadPool.QueueUserWorkItem((object obj) =>
            {
                try
                {
                    callBack(obj);
                }
                catch (Exception ex)
                {
                    Error = ex;
                }
                finally
                {
                    Finish = true;
                }
            }, state);
            return this;
        }


        public void Dispose()
        {
            Dispose(true);//这样会释放所有的资源
            GC.SuppressFinalize(this);//不需要再调用本对象的Finalize方法
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //清理托管资源
            }
            //清理非托管资源

        }

    }
}
