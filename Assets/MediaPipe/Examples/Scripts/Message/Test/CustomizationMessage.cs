using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class CustomizationMessage
{
    /// <summary>
    /// 消息：UIMessage
    /// 功能：  LocalSearch-本地作品搜索
    ///         ShopSearch-商店作品搜索
    ///         PlayWorks-播放作品
    ///         OpenRecordMovies-打开录制视频浏览列表
    ///         Login-登陆
    ///         BarMenuSetting-菜单横条
    ///         UIAdjustSetting-UI界面微调
    /// </summary>


    /// <summary>
    /// 作品列表的分类
    /// </summary>
    string uimessageBarMenu = "mrtp://UIMessage/BarMenuSetting/实景直播=mrtp://UIMessage/LocalSearch/spaceName=测试空间&tag=风景&keyWord=测试" +
                              "|虚拟直播=mrtp://UIMessage/LocalSearch/spaceName=测试空间&tag=风景&keyWord=测试";
    /// <summary>
    /// 作品界面相关功能的开闭
    /// </summary>
    //string uimessageUIAdjust = "mrtp://UIMessage/UIAdjustSetting/视频浏览=off&home=on&播放列表=off";


    //作品界面操作
    //string uimessageLocalSearch = "mrtp://UIMessage/LocalSearch/tag=风景&keyWord=测试&fromPage=home?keyName=实景直播";
    //string uimessageShopSearch = "mrtp://UIMessage/ShopSearch/spaceName=测试空间&tag=风景&keyWord=测试";
    string uimessagePlayworks = "mrtp://UIMessage/PlayWorks/workId=4f5d4g4fgg&workName=MOBI";
    string uimessagePlayworks_1 = "mrtp://UIMessage/PlayWorks/spaceName=mBase&workName=神秘领域1";
    
    
    string uimessagePlayworks_2 = "mttp://HandMessage/Status/Gesture=&Direction=&";
    string uimessagePlayworks_3 = "mttp://HandMessage/HandLandMarkMessage/Status/X=&Y=&Z=&Index=";
    //MessageMgr.GetIns().Dispatch("HandMessage", uimessagePlayworks_3);
        
    //string uimessageOpenRecordMovies = "mrtp://UIMessage/OpenRecordMovies";

    /// <summary>
    /// Home界面状态反馈s
    /// home=on  home界面显示
    /// home=off home界面隐藏
    /// </summary>
    string uimessageStatus = "mrtp://UIMessage/Status/home=off";
    //string uimessageLogin = "mrtp://UIMessage/Login/home=on";
    //string uimessageSuccess = "mrtp://UIMessage/Success/action=close";

    

    private void Start()
    {
        MessageMgr.GetIns().RegisterMessage<string>("UIMessage", Message, new Disposable());

        MessageMgr.GetIns().Dispatch("UIMessage", uimessageStatus);
    }

    private static void Message(string message)
    {
        string headStr = "mrtp://UIMessage/";
        string content = message.Substring(headStr.Length);
        string messageFunc = content.Remove(content.IndexOf("/"));
        string messageParams = content.Substring(content.IndexOf("/") + 1);
        //switch (messageFunc)
        //{
        //    case "Status":
        //        HomeStatus(messageParams);
        //        break;
        //}
    }
}
