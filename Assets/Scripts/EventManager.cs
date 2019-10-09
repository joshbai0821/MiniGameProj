using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public delegate void EventHandleFun(EventArgs args);

    public class IntEventArgs : EventArgs
    {
        public int m_args;

        public IntEventArgs(int args)
        {
            m_args = args;
        }
    }


    public class EventManager
    {
        private static Dictionary<HLEventId, Dictionary<int, EventHandleFun>> EventPool = 
            new Dictionary<HLEventId, Dictionary<int, EventHandleFun>>();
        private static bool BCacheAll = false;
        private static Queue<KeyValuePair<EventHandleFun, EventArgs>> EventCacheQueue =
            new Queue<KeyValuePair<EventHandleFun, EventArgs>>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="id">事件id枚举</param>
        /// <param name="objHashId">对象id标识</param>
        /// <param name="handleFun">对象函数</param>
        /// <returns>注册成功与否</returns>
        public static bool RegisterEvent(HLEventId id, int objHashId, EventHandleFun handleFun)
        {
            bool _ret = true;
            if(id < 0 && id >= HLEventId.MAX_EVENT)
            {
                _ret = false;
                Debug.Log(string.Format(String.Format("EventManager, EventID is Out of Range")));
                return _ret;
            }
            Dictionary<int, EventHandleFun> _eventHandler;
            if (EventPool.TryGetValue(id, out _eventHandler))
            {
                EventHandleFun _handleFun;
                if (!_eventHandler.TryGetValue(objHashId, out _handleFun))
                {
                    _eventHandler.Add(objHashId, handleFun);
                }
                else
                {
                    _ret = false;
                    Debug.Log(string.Format(String.Format("HLEventManager, Obj {0} Do Not Register Twice in Event {1}", objHashId.ToString(), id.ToString())));
                }
                
            }
            else
            {
                EventPool[id] = new Dictionary<int, EventHandleFun>();
                EventPool[id].Add(objHashId, handleFun);
            }
            return _ret;
        }

        /// <summary>
        /// 取消注册某一个事件
        /// </summary>
        /// <param name="id">事件id枚举</param>
        /// <param name="objHashId">对象id标识</param>
        /// <returns>取消注册成功与否</returns>
        public static bool UnregisterEvent(HLEventId id, int objHashId)
        {
            bool _ret = true;
            if (id == HLEventId.MAX_EVENT)
            {
                _ret = false;
                Debug.Log(String.Format("HLEventManager, EventID {0} is Out of Range", id));
            }
            Dictionary<int, EventHandleFun> _eventHandler;
            if(EventPool.TryGetValue(id, out _eventHandler))
            {
                EventHandleFun _handleFun;
                if(EventPool[id].TryGetValue(objHashId, out _handleFun))
                {
                    EventPool[id].Remove(objHashId);
                }
                else
                {
                    _ret = false;
                    Debug.Log(string.Format("HLEventManager, Obj {0} Do Not Register in Event {1}", objHashId, id));
                }
            }
            else
            {
                _ret = false;
                Debug.Log(string.Format("HLEventManager, EventPool Do Not Register Event %s Register", id));
            }
            return _ret;
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="id">事件id枚举</param>
        /// <param name="args">事件参数</param>
        /// <returns>发送成功与否</returns>
        public static bool SendEvent(HLEventId id, EventArgs args)
        {
            bool _ret = true;
            if(id < 0 || id >= HLEventId.MAX_EVENT)
            {
                _ret = false;
                Debug.Log(string.Format(String.Format("HLEventManager, EventID {0} is Out of Range", id)));
            }
            Dictionary<int, EventHandleFun> _eventHandler;
            if (EventPool.TryGetValue(id, out _eventHandler))
            {
                Dictionary<int, EventHandleFun>.Enumerator _enumerator = _eventHandler.GetEnumerator();
                while(_enumerator.MoveNext())
                {
                    if(BCacheAll)
                    {
                        EventCacheQueue.Enqueue(new KeyValuePair<EventHandleFun, EventArgs>(_enumerator.Current.Value, args));
                    }
                    else
                    {
                        EventHandleFun _handleFun = _enumerator.Current.Value;
                        _handleFun(args);
                    }
                    
                }
                _enumerator.Dispose();
            }
            else
            {
                _ret = false;
                Debug.Log(string.Format("HLEventManager, EventPool Do Not Register Event {0} Register", id));
            }
            return _ret;
        }

        /// <summary>
        /// 取消注册对象所有事件
        /// </summary>
        /// <param name="objHashId">对象id</param>
        /// <returns>取消成功与否</returns>
        public static bool UnRegisterObjectEvents(int objHashId)
        {
            bool _ret = true;
            Dictionary<HLEventId, Dictionary<int, EventHandleFun>>.Enumerator _enumerator = EventPool.GetEnumerator();
            while(_enumerator.MoveNext())
            {
                Dictionary<int, EventHandleFun> _eventHandler = _enumerator.Current.Value;
                _eventHandler.Remove(objHashId);
            }
            _enumerator.Dispose();
            return _ret;
        }

        /// <summary>
        /// 开始缓存所有事件
        /// </summary>
        /// <returns></returns>
        public static void StartCacheEvent()
        {
            BCacheAll = true;
        }


        /// <summary>
        /// 停止缓存所有事件
        /// </summary>
        /// <returns></returns>
        public static void StopCacheEvent()
        {
            BCacheAll = false;
            foreach(KeyValuePair<EventHandleFun, EventArgs> kvp in EventCacheQueue)
            {
                kvp.Key(kvp.Value);
            }
            EventCacheQueue.Clear();
        }


    }
}

