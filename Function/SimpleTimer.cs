using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace YPCommon.Function
{
    public class SimpleTimer
    {
        /// <summary>
        /// 定时委托
        /// </summary>
        private delegate void TimerDel();


        /// <summary>
        /// 工作定时器
        /// </summary>
        private static Timer WorkTimer { get; set; }

        /// <summary>
        /// 定时事件集合
        /// </summary>
        private static Dictionary<TimerType, List<ElapsedEventHandler>> TimerEventList { get; set; }

        /// <summary>
        /// 定时类型
        /// </summary>
        public enum TimerType
        {
            /// <summary>
            /// 秒
            /// </summary>
            Second,
            /// <summary>
            /// 分钟
            /// </summary>
            Minute,
            /// <summary>
            /// 小时
            /// </summary>
            Hour,
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static SimpleTimer()
        {
            WorkTimer = new Timer(1000)
            {
                Enabled = true,
                AutoReset = true,
            };
            WorkTimer.Elapsed += TimerElapsed;
            TimerEventList = new Dictionary<TimerType, List<ElapsedEventHandler>>();
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (TimerEventList == null || TimerEventList.Count <= 0) return;

            var del = new TimerDel(delegate
        {
            try
            {
                bool isMinute = DateTime.Now.Second == 0, isHour = DateTime.Now.Minute == 0;
                Dictionary<TimerType, bool> listTimerType = new Dictionary<TimerType, bool>() {
                { TimerType.Second,true },
                {TimerType.Minute,isMinute },
                {TimerType.Hour,isHour }};

                lock (TimerEventList)
                {
                    foreach (var tType in listTimerType)
                    {
                        if (HaveTimerEvent(tType.Key, TimerEventList) && tType.Value)
                        {
                            TimerElapsed(TimerEventList[tType.Key], sender, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        });

            del.BeginInvoke(null, null);

        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventList"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TimerElapsed(List<ElapsedEventHandler> eventList, object sender, ElapsedEventArgs e)
        {
            if (eventList == null || eventList.Count <= 0) return;
            foreach (var s in eventList)
            {
                var sEvent = new ElapsedEventHandler(s);
                sEvent.BeginInvoke(sender, e, null, null);
            }
        }

        /// <summary>
        /// 是否包含指定类型事件
        /// </summary>
        /// <param name="timerType"></param>
        /// <param name="timerEventList"></param>
        /// <returns></returns>
        private static bool HaveTimerEvent(TimerType timerType, Dictionary<TimerType, List<ElapsedEventHandler>> timerEventList)
        {
            return timerEventList != null && timerEventList.ContainsKey(timerType) && timerEventList[timerType].Count > 0;
        }


        /// <summary>
        /// 添加定时事件
        /// </summary>
        /// <param name="tType">定时类型</param>
        /// <param name="timerEvent">定时事件</param>
        public static void AddTimeEvent(TimerType tType, ElapsedEventHandler timerEvent)
        {
            if (TimerEventList == null) TimerEventList = new Dictionary<TimerType, List<ElapsedEventHandler>>();
            lock (TimerEventList)
            {
                if (TimerEventList.ContainsKey(tType))
                {
                    if (TimerEventList[tType] == null) TimerEventList[tType] = new List<ElapsedEventHandler>();
                    TimerEventList[tType].Add(timerEvent);
                }
                else
                {
                    TimerEventList.Add(tType, new List<ElapsedEventHandler>() { timerEvent });
                }
            }
        }

        /// <summary>
        /// 添加定时事件
        /// </summary>
        /// <param name="tType">定时类型</param>
        /// <param name="timerEvent">定时事件</param>
        public static void RemoveTimeEvent(TimerType tType, ElapsedEventHandler timerEvent)
        {
            if (TimerEventList == null) return;
            lock (TimerEventList)
            {
                if (TimerEventList.ContainsKey(tType) && TimerEventList[tType].Contains(timerEvent))
                {
                    TimerEventList[tType].Remove(timerEvent);
                }
            }
        }


    }
}
