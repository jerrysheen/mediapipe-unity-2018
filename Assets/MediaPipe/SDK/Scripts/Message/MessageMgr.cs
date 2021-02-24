using System;
using System.Collections.Generic;

public class MessageData
{
    /// <summary>
    /// 唯一id
    /// </summary>
    public string Uid;

    /// <summary>
    /// 回调
    /// </summary>
    public Delegate mDelegate;

    public MessageData(string uid, Delegate d)
    {
        Uid = uid;
        mDelegate = d;
    }
}

/// <summary>
/// 消息管理器
/// </summary>
public class MessageMgr
{
    /// <summary>
    /// 消息列表字典
    /// </summary>
    private Dictionary<string, List<MessageData>> messageDic = new Dictionary<string, List<MessageData>>();

    /// <summary>
    /// 已注册字典
    /// </summary>
    private Dictionary<string, bool> registerDic = new Dictionary<string, bool>();

    /// <summary>
    /// 单例
    /// </summary>
    private static MessageMgr instance;

    public static MessageMgr GetIns()
    {
        if(null == instance)
        {
            instance = new MessageMgr();
        }
        return instance;
    }

    #region 注册消息
    public void RegisterMessage<T1, T2, T3, T4>(string msg, Action<T1, T2, T3, T4> callback, Disposable mDisposable)
    {
        RegisterMessage(msg, mDisposable.GetUid(), (Delegate)callback);
    }
    public void RegisterMessage<T1, T2, T3>(string msg, Action<T1, T2, T3> callback, Disposable mDisposable)
    {
        RegisterMessage(msg, mDisposable.GetUid(), (Delegate)callback);
    }
    public void RegisterMessage<T1, T2>(string msg, Action<T1, T2> callback, Disposable mDisposable)
    {
        RegisterMessage(msg, mDisposable.GetUid(), (Delegate)callback);
    }
    public void RegisterMessage<T>(string msg, Action<T> callback, Disposable mDisposable)
    {
        RegisterMessage(msg, mDisposable.GetUid(), (Delegate)callback);
    }
    public void RegisterMessage(string msg, Action callback, Disposable mDisposable)
    {
        RegisterMessage(msg, mDisposable.GetUid(), (Delegate)callback);
    }
    public void RegisterMessage(string msg, string uid, Delegate callback)
    {
        string tempMessageName = msg + uid;
        if(registerDic.ContainsKey(tempMessageName))
        {
            return;
        }
        registerDic[tempMessageName] = true;
        if(!messageDic.ContainsKey(msg))
        {
            messageDic[msg] = new List<MessageData>();
        }
        messageDic[msg].Add(new MessageData(uid, callback));

    }
    #endregion

    #region 移除消息

    /// <summary>
    /// 移除所有消息
    /// </summary>
    public void RemoveAllMessage()
    {
        messageDic.Clear();
        registerDic.Clear();
    }

    public void RemoveMessage(string msg, Disposable mDisposable)
    {
        RemoveMessage(msg, mDisposable.GetUid());
    }

    private void RemoveMessage(string msg,string uid)
    {
        string tempMessageName = msg + uid;
        if (registerDic.ContainsKey(tempMessageName))
        {
            registerDic.Remove(tempMessageName);
            if (messageDic.ContainsKey(msg))
            {
                List<MessageData> tempList = messageDic[msg];
                if(null != tempList)
                {
                    for (int i =tempList.Count;i-->0;)
                    {
                        if(tempList[i].Uid == uid)
                        {
                            tempList.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 派发消息
    public void Dispatch<T1, T2, T3, T4>(string msg, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        List<MessageData> messageList = GetMessageList(msg);
        if (messageList != null)
        {
            foreach (MessageData m in messageList)
            {
                try
                {
                    ((Action<T1, T2, T3, T4>)m.mDelegate)(arg1, arg2, arg3, arg4);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch<T1, T2, T3>(string msg, T1 arg1, T2 arg2, T3 arg3)
    {
        List<MessageData> messageList = GetMessageList(msg);
        if (messageList != null)
        {
            foreach (MessageData m in messageList)
            {
                try
                {
                    ((Action<T1, T2, T3>)m.mDelegate)(arg1, arg2, arg3);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch<T1, T2>(string msg, T1 arg1, T2 arg2)
    {
        List<MessageData> messageList = GetMessageList(msg);
        if (messageList != null)
        {
            foreach (MessageData m in messageList)
            {
                try
                {
                    ((Action<T1, T2>)m.mDelegate)(arg1, arg2);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch<T>(string msg, T arg)
    {
        List<MessageData> messageList = GetMessageList(msg);
        if (messageList != null)
        {
            foreach (MessageData m in messageList)
            {
                try
                {
                    ((Action<T>)m.mDelegate)(arg);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch(string msg)
    {
        List<MessageData> messageList = GetMessageList(msg);
        if (messageList != null)
        {
            for (int i = 0; i < messageList.Count; i++)
            {
                try
                {
                    ((Action)messageList[i].mDelegate)();
                }
                catch (Exception e) { LogError(e); }
            }
            //foreach (MessageData m in messageList)
            //{
            //    try
            //    {
            //        ((Action)m.mDelegate)();
            //    }
            //    catch (Exception e) { LogError(e); }
            //}
        }
    }
    #endregion

    /// <summary>
    /// 获取所有消息
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private List<MessageData> GetMessageList(string msg)
    {
        if (messageDic.ContainsKey(msg))
        {
            List<MessageData> tempList = messageDic[msg];
            if (null != tempList)
            {
                return tempList;
            }
        }
        return null;
    }

    private static void LogError(Exception e)
    {
        //UnityEngine.Debug.LogError(e);
    }
}