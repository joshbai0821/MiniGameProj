using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MiniProj
{
    public class TimerManager
    {
        private static List<TimerItem> m_timerList;
        private static long managerTimerId = 0;
        private static long m_dtTickFrom = 0;

        public void Initial()
        {
            managerTimerId = 0;
            DateTime _dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            m_dtTickFrom = _dtFrom.Ticks;
        }

        enum TimerStatus
        {
            BORNING = 0,
            RUNNING,
            DEAD,
        }

        class TimerItem: ICloneable
        {
            private long m_interval;
            private long m_delay;
            private long m_callTime;
            private long m_timerId;
            private bool m_bLoop;
            private TimerStatus status;
            private EventHandleFun m_func;
            private EventArgs m_args;

            public long CallTime
            {
                get { return m_callTime; }
            }

            public long TimerID
            {
                get { return m_timerId; }
            }

            public bool Loop
            {
                get { return m_bLoop; }
            }

            public TimerStatus Status
            {
                get { return status; }
                set { status = value; }
            }


            public TimerItem(long interval, long delay, long callTime, long timerId, bool loop, EventHandleFun fun, EventArgs args)
            {
                m_interval = interval;
                m_delay = delay;
                m_callTime = callTime;
                m_timerId = timerId;
                m_bLoop = loop;
                m_func = fun;
                m_args = args;
                status = TimerStatus.BORNING;
            }

            public static bool operator > (TimerItem a, TimerItem b)
            {
                return a.m_callTime > b.m_callTime;
            }

            public static bool operator < (TimerItem a, TimerItem b)
            {
                return a.m_callTime < b.m_callTime;
            }

            public object Clone()
            {
                TimerItem _newItem = new TimerItem(m_interval, m_delay, m_callTime, m_timerId, m_bLoop, m_func, m_args);
                return _newItem;
            }

            public void Execute()
            {
                m_func(m_args);
            }

            public void ResetCallTime()
            {
                m_callTime = GetCurrentMilliSecond() + m_interval + m_delay;
            }
        }

        public static long StartTimer(long interval, bool loop, EventArgs args, EventHandleFun func, long delay)
        {

            long _callTime = GetCurrentMilliSecond() + interval + delay;
            long _timeId = managerTimerId;
            ++managerTimerId;
            TimerItem item = new TimerItem(interval, delay, _callTime, _timeId, loop, func, args);
            AddTimerItem(item);
            return _timeId;
        }

        public static void StopTimer(long timerId)
        {
            if(timerId < 0 || timerId >= managerTimerId)
            {
                Debug.Log(String.Format("TimerId {0} is Not Correct", timerId.ToString()));
            }
            else
            {
                foreach(TimerItem item in m_timerList)
                {
                    if(timerId == item.TimerID)
                    {
                        item.Status = TimerStatus.DEAD;
                        break;
                    } 
                }
            }
        }

        public static void UpdateTimerList()
        {
            long currentTicks = DateTime.Now.Ticks;
            long currentMillis = (currentTicks - m_dtTickFrom) / 10000;
            while(m_timerList.Count > 0 && currentMillis > m_timerList[0].CallTime)
            {
                TimerItem _item = RemoveTimerItem();
                if(_item.Status == TimerStatus.BORNING)
                {
                    _item.Execute();
                }
                
                if(_item.Status != TimerStatus.DEAD && _item.Loop)
                {
                    _item.ResetCallTime();
                }
                else
                {
                    _item.Status = TimerStatus.DEAD;
                }
            }
        }

        private static void AddTimerItem(TimerItem item)
        {
            m_timerList.Add(item);
            int index = m_timerList.Count - 1;
            int parentIndex = (index + 1) / 2 - 1; 
            while (parentIndex >= 0 && m_timerList[parentIndex] > m_timerList[index])
            {
                SwapTimerItem(index, parentIndex);
                index = parentIndex;
                parentIndex = (index + 1) / 2 - 1;
            }
        }

        private static void SwapTimerItem(int index1, int index2)
        {
            TimerItem tempItem = m_timerList[index1];
            m_timerList[index1] = m_timerList[index2];
            m_timerList[index2] = tempItem;
        }

        private static TimerItem RemoveTimerItem()
        {
            int _lastIndex = m_timerList.Count - 1;
            SwapTimerItem(0, _lastIndex);
            TimerItem item = m_timerList[_lastIndex];
            m_timerList.RemoveAt(_lastIndex);
            int _index = 0;
            int _childIndex = _index * 2 + 1;
            while(_childIndex < m_timerList.Count)
            {
                if(_childIndex + 1 < m_timerList.Count && m_timerList[_childIndex] > m_timerList[_childIndex + 1])
                {
                    ++_childIndex;
                }
                if(m_timerList[_childIndex] < m_timerList[_index])
                {
                    SwapTimerItem(_index, _childIndex);
                    _index = _childIndex;
                    _childIndex = _index * 2 + 1;
                }
                else
                {
                    break;
                }
            }
            return item;

        }

        private static long GetCurrentMilliSecond()
        {
            //获取当前Ticks
            long currentTicks = DateTime.Now.Ticks;
            long currentMillis = (currentTicks - m_dtTickFrom) / 10000;
            return currentMillis;
        }
    }
}


