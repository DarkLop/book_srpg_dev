#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MessageCenter.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 15:55:19 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    /// <summary>
    /// 消息中心。
    /// 消息中心的方式比较多，你可以挑选一种或几种。
    /// 这里把所有东西都放在了MessageCenter中，
    /// </summary>
    public static class MessageCenter
    {
        #region Delegate
        /// <summary>
        /// Delegate Listener
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="messageArgs"></param>
        /// <param name="messageParams"></param>
        public delegate void MessageListener(string message, object sender, MessageArgs messageArgs, params object[] messageParams);
        #endregion

        #region Field
        /// <summary>
        /// 注册的Listener.
        /// 你也可以不使用List，直接使用MessageListener.
        /// 注册的时候用 listener += method.
        /// 注销的时候用 listener -= method.
        /// </summary>
        private static readonly Dictionary<string, List<MessageListener>> s_ListenerDict = new Dictionary<string, List<MessageListener>>();


        /// <summary>
        /// 注册的Handler.
        /// 你也可以使用Dictionary＜string, List＜IMesageHandler＞＞来分类保存。
        /// </summary>
        private static readonly List<IMessageHandler> s_Handlers = new List<IMessageHandler>();

        /// <summary>
        /// 注册的Controller.
        /// 这里使用普通的类，并使用System.Activator.CreateInstance(System.Type type)创建，并执行.
        /// 你也可以不用它，而使用Asset的ScriptableObject，见方法ExecuteMessage中的注释， 
        /// 如果是动态创建的ScriptableObject，且你需要销毁它，则要使用Destroy，不要不管或赋值null。
        /// </summary>
        private static readonly Dictionary<string, Type> s_ControllerDict = new Dictionary<string, Type>();
        #endregion

        #region Add/Remove Listener
        /// <summary>
        /// 注册Listener
        /// </summary>
        /// <param name="message"></param>
        /// <param name="listener"></param>
        public static void AddListener(string message, MessageListener listener)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[MessageCenter] Argument named 'message' is null or empty.");
                return;
            }

            if (listener == null)
            {
                Debug.LogError("[MessageCenter] Argument named 'listener' is null.");
                return;
            }

            List<MessageListener> listeners;
            if (!s_ListenerDict.TryGetValue(message, out listeners))
            {
                listeners = new List<MessageListener>();
                s_ListenerDict.Add(message, listeners);
            }

            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        /// <summary>
        /// 注销Listener
        /// 如果Listener是MonoBehaviour, 你可以在OnDestroy或其它销毁Method中，加入注销.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="listener"></param>
        public static void RemoveListener(string message, MessageListener listener)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[MessageCenter] Argument named 'message' is null or empty.");
                return;
            }

            if (listener == null)
            {
                Debug.LogError("[MessageCenter] Argument named 'listener' is null.");
                return;
            }

            List<MessageListener> listeners;
            if (!s_ListenerDict.TryGetValue(message, out listeners))
            {
                return;
            }

            if (listeners.Remove(listener) && listeners.Count == 0)
            {
                s_ListenerDict.Remove(message);
            }
        }

        /// <summary>
        /// 注销Listeners
        /// </summary>
        /// <param name="message"></param>
        public static void RemoveListeners(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[MessageCenter] Argument named 'message' is null or empty.");
                return;
            }

            s_ListenerDict.Remove(message);
        }

        /// <summary>
        /// 注销所有Listeners
        /// </summary>
        public static void RemoveListenersAll()
        {
            s_ListenerDict.Clear();
        }
        #endregion

        #region (Un)Register Handler
        /// <summary>
        /// 注册Handler
        /// </summary>
        /// <param name="handler"></param>
        public static void RegisterHandler(IMessageHandler handler)
        {
            if (handler == null)
            {
                Debug.LogError("[MessageCenter] Argument named 'handler' is null.");
                return;
            }

            if (!s_Handlers.Contains(handler))
            {
                s_Handlers.Add(handler);
            }
        }

        /// <summary>
        /// 注销Handler.
        /// 如果Handler是MonoBehaviour, 你可以在OnDestroy或其它销毁Method中，加入注销.
        /// </summary>
        /// <param name="handler"></param>
        public static void UnregisterHandler(IMessageHandler handler)
        {
            if (handler == null)
            {
                Debug.LogError("[MessageCenter] Argument named 'handler' is null.");
                return;
            }

            s_Handlers.Remove(handler);
        }

        /// <summary>
        /// 注销所有Handlers
        /// </summary>
        public static void UnregisterHandlersAll()
        {
            s_Handlers.Clear();
        }
        #endregion

        #region (Un)Register Controller
        /// <summary>
        /// 注册Controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public static void RegisterController<T>(string message) where T : MessageControllerBase
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[MessageCenter] Argument named 'message' is null or empty.");
                return;
            }

            Type type = typeof(T);
            if (type.IsAbstract)
            {
                Debug.LogError("[MessageCenter] Type of controller is abstract.");
                return;
            }

            if (s_ControllerDict.ContainsKey(message))
            {
                Debug.LogError("[MessageCenter] Type of controller is exist.");
                return;
            }

            s_ControllerDict.Add(message, type);
        }

        /// <summary>
        /// 注销Controller
        /// </summary>
        /// <param name="message"></param>
        public static void UnregisterController(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[MessageCenter] Argument named 'message' is null or empty.");
                return;
            }

            s_ControllerDict.Remove(message);
        }

        /// <summary>
        /// 注销所有Controllers
        /// </summary>
        public static void UnregisterControllerAll()
        {
            s_ControllerDict.Clear();
        }
        #endregion

        #region Unregister All
        /// <summary>
        /// 注销所有（Controller, Handler, Listener）
        /// </summary>
        public static void UnregisterAll()
        {
            RemoveListenersAll();
            UnregisterHandlersAll();
            UnregisterControllerAll();
        }
        #endregion

        #region Send Method
        /// <summary>
        /// 分发Message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="messageParams"></param>
        public static void Send(string message, object sender, params object[] messageParams)
        {
            ExecuteMessage(message, sender, null, messageParams);
        }

        /// <summary>
        /// 分发Message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="messageParams"></param>
        public static void Send(string message, object sender, MessageArgs args, params object[] messageParams)
        {
            ExecuteMessage(message, sender, args, messageParams);
        }

        /// <summary>
        /// 分发Message的实现
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="messageParams"></param>
        private static void ExecuteMessage(string message, object sender, MessageArgs args, params object[] messageParams)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[MessageCenter] Argument named 'message' is null or empty.");
                return;
            }

            if (sender == null)
            {
                Debug.LogError("[MessageCenter] Argument named 'sender' is null");
                return;
            }

            // 分发给Controller
            Type type;
            if (s_ControllerDict.TryGetValue(message, out type))
            {
                MessageControllerBase controller = Activator.CreateInstance(type) as MessageControllerBase;
                if (controller != null)
                {
                    controller.ExecuteMessage(message, sender, args, messageParams);
                }
            }

            /*
            ///// 使用已经创建好的ScriptableObject，Dictionary<string, MessageControllerBase> s_ControllerDict;
            // MessageControllerBase controller;
            // if (s_ControllerDict.TryGetValue(message, out controller))
            // {
            //     controller.ExecuteMessage(message, sender, args, messageParams);
            // }
            */

            /*
            ///// 使用动态创建的ScriptableObject
            // Type assetType;
            // if (s_ControllerDict.TryGetValue(message, out assetType))
            // {
            //     MessageControllerBase controller = ScriptableObject.CreateInstance(assetType) as MessageControllerBase;
            //     controller.ExecuteMessage(message, sender, args, messageParams);
            //     ScriptableObject.Destroy(controller);
            // }
            */

            // 分发给Handler
            if (s_Handlers.Count > 0)
            {
                for (int i = 0; i < s_Handlers.Count; i++)
                {
                    IMessageHandler handler = s_Handlers[i];
                    if (handler == null)
                    {
                        s_Handlers.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        handler.ExecuteMessage(message, sender, args, messageParams);
                    }
                }
            }

            // 分发给Listener
            List<MessageListener> listeners;
            if (s_ListenerDict.TryGetValue(message, out listeners))
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    MessageListener listener = listeners[i];
                    if (listener == null)
                    {
                        listeners.RemoveAt(i);
                        i--;
                        if (listeners.Count == 0)
                        {
                            s_ListenerDict.Remove(message);
                            break;
                        }
                    }
                    else
                    {
                        listener(message, sender, args, messageParams);
                    }
                }
            }
        }
        #endregion

        #region Type Extention
        /// <summary>
        /// 让MonoBehaviour可以发送消息
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="message"></param>
        /// <param name="messageParams"></param>
        public static void SendByMessageCenter(this MonoBehaviour mono, string message, params object[] messageParams)
        {
            if (mono == null)
            {
                Debug.LogError("[MessageCenter] Type extention. Argument named 'mono' is null.");
                return;
            }

            ExecuteMessage(message, mono, null, messageParams);
        }

        /// <summary>
        /// 让MonoBehaviour可以发送消息
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <param name="messageParams"></param>
        public static void SendByMessageCenter(this MonoBehaviour mono, string message, MessageArgs args, params object[] messageParams)
        {
            if (mono == null)
            {
                Debug.LogError("[MessageCenter] Type extention. Argument named 'mono' is null.");
                return;
            }

            ExecuteMessage(message, mono, args, messageParams);
        }
        #endregion
    }
}