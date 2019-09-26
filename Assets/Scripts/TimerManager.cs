using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MiniProj
{
    public class TimerManager
    {
        private static List<TimerItem> TimerList = new List<TimerItem>();
        private static long ManagerTimerId = 0;
        private static long DtTickFrom = 0;

        public void Initial()
        {
            ManagerTimerId = 0;
            DateTime _dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DtTickFrom = _dtFrom.Ticks;
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
            private TimerStatus m_status;
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
                get { return m_status; }
                set { m_status = value; }
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
                m_status = TimerStatus.BORNING;
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
            long _timeId = ManagerTimerId;
            ++ManagerTimerId;
            TimerItem item = new TimerItem(interval, delay, _callTime, _timeId, loop, func, args);
            AddTimerItem(item);
            return _timeId;
        }

        public static void StopTimer(long timerId)
        {
            if(timerId < 0 || timerId >= ManagerTimerId)
            {
                Debug.Log(String.Format("TimerId {0} is Not Correct", timerId.ToString()));
            }
            else
            {
                foreach(TimerItem item in TimerList)
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
            long currentMillis = (currentTicks - DtTickFrom) / 10000;
            while(TimerList.Count > 0 && currentMillis > TimerList[0].CallTime)
            {
                TimerItem _item = RemoveTimerItem();
                if(_item.Status == TimerStatus.BORNING)
                {
                    _item.Execute();
                }
                
                if(_item.Status != TimerStatus.DEAD && _item.Loop)
                {
                    _item.ResetCallTime();
                    AddTimerItem(_item);
                }
                else
                {
                    _item.Status = TimerStatus.DEAD;
                }
            }
        }

        private static void AddTimerItem(TimerItem item)
        {
            TimerList.Add(item);
            int index = TimerList.Count - 1;
            int parentIndex = (index + 1) / 2 - 1; 
            while (parentIndex >= 0 && TimerList[parentIndex] > TimerList[index])
            {
                SwapTimerItem(index, parentIndex);
                index = parentIndex;
                parentIndex = (index + 1) / 2 - 1;
            }
        }

        private static void SwapTimerItem(int index1, int index2)
        {
            TimerItem tempItem = TimerList[index1];
            TimerList[index1] = TimerList[index2];
            TimerList[index2] = tempItem;
        }

        private static TimerItem RemoveTimerItem()
        {
            int _lastIndex = TimerList.Count - 1;
            SwapTimerItem(0, _lastIndex);
            TimerItem item = TimerList[_lastIndex];
            TimerList.RemoveAt(_lastIndex);
            int _index = 0;
            int _childIndex = _index * 2 + 1;
            while(_childIndex < TimerList.Count)
            {
                if(_childIndex + 1 < TimerList.Count && TimerList[_childIndex] > TimerList[_childIndex + 1])
                {
                    ++_childIndex;
                }
                if(TimerList[_childIndex] < TimerList[_index])
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
            long currentMillis = (currentTicks - DtTickFrom) / 10000;
            return currentMillis;
        }
    }
}


