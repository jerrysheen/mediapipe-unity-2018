using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    public GameObject uiPanel;

    private Disposable disposable;

    // Start is called before the first frame update
    void Start()
    {
        if (disposable == null)
        {
            disposable = new Disposable();
            MessageMgr.GetIns().RegisterMessage<string>("UIMessage", Execute, disposable);
        }
        string uimessageBarMenu = "mrtp://UIMessage/BarMenuSetting/实景直播=mrtp://UIMessage/LocalSearch/spaceName=测试空间&tag=风景&keyWord=测试|虚拟直播=mrtp://UIMessage/LocalSearch/spaceName=测试空间&tag=风景&keyWord=测试";
        string uimessageUIAdjust = "mrtp://UIMessage/UIAdjustSetting/视频浏览=off&home=on&播放列表=off";
        MessageMgr.GetIns().Dispatch("UIMessage", uimessageBarMenu);
        MessageMgr.GetIns().Dispatch("UIMessage", uimessageUIAdjust);
    }

    /// <summary>
    /// 指令解析
    /// </summary>
    /// <param name="msg"></param>
    private void Execute(string msg)
    {
        
        string headStr = "mrtp://UIMessage/";
        string content = msg.Substring(headStr.Length);
        string messageFunc = content.Remove(content.IndexOf("/"));
        string messageParams = content.Substring(content.IndexOf("/") + 1);
        switch (messageFunc)
        {
            case "Status":
                HomeStatus(messageParams);
                break;
        }
    }

    /// <summary>
    /// Home显示状态设置
    /// </summary>
    /// <param name="msg"></param>
    private void HomeStatus(string msg)
    {
        if (!msg.Contains("="))
        {
            return;
        }
        string[] statusKeyValues;
        if (msg.Contains("&"))
        {
            statusKeyValues = msg.Split('&');
        }
        else
        {
            statusKeyValues = new string[] { msg };
        }

        for (int i = 0; i < statusKeyValues.Length; i++)
        {
            string[] statusValue = statusKeyValues[i].Split('=');
            if (statusValue[0] == "home")
            {
                if (statusValue[1] == "off")
                {
                    uiPanel.SetActive(false);
                }
                else
                {
                    uiPanel.SetActive(true);
                }
            }
        }
    }
}
