using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public delegate void EventHandleFun(EventArgs args);

    public class EventManager
    {
        private static Dictionary<HLEventId, Dictionary<int, EventHandleFun>> m_eventPool;
        private static Dictionary<HLEventId, Dictionary<int, EventHandleFun>> m_eventOncePool;
        private static bool m_bCacheAll = false;
        private static Queue<KeyValuePair<EventHandleFun, EventArgs>> m_eventCacheQueue;

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
            if(id >= 0 && id < HLEventId.MAX_EVENT)
            {
                _ret = false;
                Debug.Log(string.Format(String.Format("HLEventManager, EventID is Out of Range")));
            }
            Dictionary<int, EventHandleFun> _eventHandler;
            if (!m_eventPool.TryGetValue(id, out _eventHandler))
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
                m_eventPool[id] = new Dictionary<int, EventHandleFun>();
                m_eventPool[id].Add(objHashId, handleFun);
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
            if(id >= 0 && id < HLEventId.MAX_EVENT)
            {
                _ret = false;
                Debug.Log(string.Format(String.Format("HLEventManager, EventID {0} is Out of Range", id)));
            }
            Dictionary<int, EventHandleFun> _eventHandler;
            if(!m_eventPool.TryGetValue(id, out _eventHandler))
            {
                EventHandleFun _handleFun;
                if(!m_eventPool[id].TryGetValue(objHashId, out _handleFun))
                {
                    m_eventPool[id].Remove(objHashId);
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
            if(id >= 0 && id <= HLEventId.MAX_EVENT)
            {
                _ret = false;
                Debug.Log(string.Format(String.Format("HLEventManager, EventID {0} is Out of Range", id)));
            }
            Dictionary<int, EventHandleFun> _eventHandler;
            if (!m_eventPool.TryGetValue(id, out _eventHandler))
            {
                Dictionary<int, EventHandleFun>.Enumerator _enumerator = _eventHandler.GetEnumerator();
                while(_enumerator.MoveNext())
                {
                    if(m_bCacheAll)
                    {
                        m_eventCacheQueue.Enqueue(new KeyValuePair<EventHandleFun, EventArgs>(_enumerator.Current.Value, args));
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
            Dictionary<HLEventId, Dictionary<int, EventHandleFun>>.Enumerator _enumerator = m_eventPool.GetEnumerator();
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
            m_bCacheAll = true;
        }


        /// <summary>
        /// 停止缓存所有事件
        /// </summary>
        /// <returns></returns>
        public static void StopCacheEvent()
        {
            m_bCacheAll = false;
            foreach(KeyValuePair<EventHandleFun, EventArgs> kvp in m_eventCacheQueue)
            {
                kvp.Key(kvp.Value);
            }
            m_eventCacheQueue.Clear();
        }


    }
}

