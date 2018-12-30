using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
//成就系统面板控制器
public class AchievementPanelController : MonoBehaviour {

    public GameObject lobbyPanel;           //大厅面板
    public GameObject roomPanel;            //房间面板
    public GameObject achievementPanel;     //成就系统面板
    public Button backButton;               //返回按钮
    public Text currentPanel;               //当前面板文本信息

    public Button achievementButton;        //“成就”按钮
    public GameObject achievement;          //“成就”页面
    public GameObject[] achievementItems;   //“成就”页面中的成就任务信息
    public GameObject previousButton;       //“上一页”按钮
    public GameObject nextButton;           //“下一页”按钮
    public Text pageMessage;                //页面信息
    public GameObject processingWindow;     //“处理中”提示窗口

    public GameObject reward;               //“奖励”页面
    public GameObject[] rewardItems;        //“奖励”页面中的奖励条目信息
    public Text goldCurrencyCount;          //玩家的金币数量显示（领取奖励后，玩家的金币数量发生变化）

    private List<string> achievementKeys;                   //成就任务的键值
    private Dictionary<string, string> achievementData;     //成就任务的数据
    private int itemsLength;                //成就任务个数
    private const int itemsPerPage = 8;     //每页显示的成就任务数量
    private int currentPageNumber;          //当前页数
    private int maxPageNumber;              //总页数

    private List<string> rewardKeys;                //成就奖励的名称（Key）
    private Dictionary<string, string> rewardData;  //成就奖励的数据（Value）

    private int requestNum;                         

    void OnEnable()
    {
        currentPanel.text = "成 就";
        backButton.onClick.RemoveAllListeners();        
        backButton.onClick.AddListener(delegate {
            if (PhotonNetwork.inRoom)           //如果玩家在游戏房间中，点击返回按钮后，游戏界面显示游戏房间。
                roomPanel.SetActive(true);
            else                                //如果玩家在游戏大厅中，点击返回按钮后，游戏界面显示游戏大厅。
                lobbyPanel.SetActive(true);
            achievementPanel.SetActive(false);
        });
        processingWindow.SetActive(false);      //禁用“处理中”提示窗口
        
        Init(); //成就系统数据初始化

        //显示“成就”界面
        achievementButton.Select(); 
        ClickAchievementButton();
    }

    //成就系统数据初始化。
    void Init()
    {
        achievementData = new Dictionary<string, string>(); 
        achievementKeys = new List<string>();
        foreach (KeyValuePair<string, string> kvp in GameInfo.titleData)    //遍历GameInfo中游戏titleData的信息
        {
            if (kvp.Key.Contains("Achievement"))            //如果titleData数据的键值包含Achievement，表示该数据是成就任务数据
            {
                achievementData.Add(kvp.Key,kvp.Value);
                achievementKeys.Add(kvp.Key);
            }
        }
        itemsLength = achievementData.Count;

        rewardData = new Dictionary<string, string>();
        rewardKeys = new List<string>();

        foreach (KeyValuePair<string, string> kvp in GameInfo.titleData)    //遍历GameInfo中游戏titleData的信息
        {
            if (kvp.Key.Contains("Reward"))                 //如果titleData数据的键值包含Reward，表示该数据是成就奖励数据
            {
                rewardData.Add(kvp.Key, kvp.Value);
                rewardKeys.Add(kvp.Key);
            }
        }
    }

    //“成就”按钮的响应事件
    public void ClickAchievementButton()
    {
        achievement.SetActive(true);    //显示“成就”界面
        reward.SetActive(false);        //禁用“奖励”界面

        //初始化页面信息
        currentPageNumber = 1;
        maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();

        ButtonControl();                //翻页按钮控制
        ShowAchievementItems();         //显示成就条目
    }

    //翻页按钮控制
    void ButtonControl()
    {
        if (currentPageNumber == 1)
            previousButton.SetActive(false);
        else
            previousButton.SetActive(true);
        if (currentPageNumber == maxPageNumber)
            nextButton.SetActive(false);
        else
            nextButton.SetActive(true);
    }

    //显示成就任务条目
    void ShowAchievementItems()
    {
        //根据当前页数计算需要显示的成就条目
        int start, end, i, j;
        start = (currentPageNumber - 1) * itemsPerPage;
        if (currentPageNumber == maxPageNumber)
            end = itemsLength;
        else
            end = start + itemsPerPage;
        Text[] texts;
        Button button;
        Dictionary<string, string> myData;

        //在“成就”页面显示成就条目
        for (i = start, j = 0; i < end; i++, j++)
        {
            string achievementName = achievementKeys[i];
            texts = achievementItems[j].GetComponentsInChildren<Text>();
            button = achievementItems[j].GetComponentInChildren<Button>();
            myData = PlayFabSimpleJson.DeserializeObject<Dictionary<string, string>>(achievementData[achievementName]);
            texts[0].text = myData["Name"];                                         //成就名称
            texts[1].text = myData["Description"] + "：" + myData["Count"];         //成就描述            
            texts[2].text = myData["Point"];                                        //成就点数
            int point = int.Parse(myData["Point"]);

            //如果玩家已经领取该成就
            if (PlayFabUserData.userData.ContainsKey(achievementName))
            {
                texts[3].text = "已领取";      
                button.interactable = false;            //禁用按钮的交互
            }
            //如果玩家未领取该成就，但是已完成成就任务
            else if((myData["Description"] == "累计杀敌" && PlayFabUserData.totalKill >= int.Parse(myData["Count"])) 
                || (myData["Description"] == "胜利场次" && PlayFabUserData.totalWin >= int.Parse(myData["Count"])))
            {
                texts[3].text = "领取";
                button.interactable = true;             //启用按钮的交互
                button.onClick.RemoveAllListeners();
                //为按钮绑定响应事件
                button.onClick.AddListener(delegate ()
                {
                    GetAchievement(achievementName,point); 
                });
            }
            //如果玩家未完成该成就
            else
            {
                texts[3].text = "未完成";
                button.interactable = false;            //禁用按钮的交互
            }

            achievementItems[j].SetActive(true);        //显示成就系统条目
        }
        for (; j < itemsPerPage; j++)
            achievementItems[j].SetActive(false);
    }
    
    /// <summary>
    /// 完成成就任务，领取成就点
    /// name：成就任务名字
    /// point：成就任务点数
    /// </summary>
    void GetAchievement(string name,int point)  
    {
        processingWindow.SetActive(true);               //启用“处理中”窗口
        PlayFabUserData.achievementPoints += point;     //计算成就点数

        //更新玩家数据Player Data：成就点数、成就任务已领取的标记
        UpdateUserDataRequest request = new UpdateUserDataRequest();    
        request.Data = new Dictionary<string, string>();
        request.Data.Add(name, "true");
        request.Data.Add("AchievementPoints", PlayFabUserData.achievementPoints.ToString());
        PlayFabClientAPI.UpdateUserData(request, OnUpdateUserData, OnPlayFabError);
    }

    //玩家数据Player Data更新成功后调用
    void OnUpdateUserData(UpdateUserDataResult result)
    {
        //重新获取玩家数据Player Data
        GetUserDataRequest request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnGetUserData, OnPlayFabError);
    }

    //玩家数据Player Data获取成功后调用
    void OnGetUserData(GetUserDataResult result) {
        PlayFabUserData.userData = result.Data;     //在PlayFabUserData中保存玩家数据Player Data
        processingWindow.SetActive(false);          //禁用“处理中”窗口
        ShowAchievementItems();                     //玩家领取了成就，更新成就任务条目的显示
    }

    //PlayFab的请求失败时调用
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error);  //在控制台显示失败原因
        processingWindow.SetActive(false);              //禁用“处理中”窗口
    }

    //点击“奖励”按钮，显示成就的奖励界面
    public void ClickRewardButton()
    {
        achievement.SetActive(false);
        reward.SetActive(true);
        ShowRewardItems();
    }

    //显示成就奖励条目
    void ShowRewardItems()
    {
        int length = rewardItems.Length,i;
        Dictionary<string, string> myData;
        Text[] texts;
        Button button;

        //显示成就奖励条目
        for (i = 0; i < rewardKeys.Count; i++)
        {
            string rewardName = rewardKeys[i];
            texts = rewardItems[i].GetComponentsInChildren<Text>();
            button = rewardItems[i].GetComponentInChildren<Button>();
            myData = PlayFabSimpleJson.DeserializeObject<Dictionary<string, string>>(rewardData[rewardName]);
            int rewardValue = int.Parse(myData["RewardValue"]);
            //成就奖励
            texts[1].text = myData["RewardValue"];
            //累计获得成就点和奖励目标成就点
            texts[2].text = "累计获得成就点" + PlayFabUserData.achievementPoints + "/" + myData["TargetPoints"];
            //如果成就奖励已领取
            if (PlayFabUserData.userData.ContainsKey(rewardName))
            {
                button.interactable = false;
                texts[3].text = "已领取";
            }
            //如果成就奖励未领取，且成就奖励目标已达成
            else if (PlayFabUserData.achievementPoints >= int.Parse(myData["TargetPoints"]))
            {
                button.interactable = true;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate ()
                {
                    GetReward(rewardName, rewardValue);
                });
                texts[3].text = "领取";
            }
            //如果成就奖励未完成
            else
            {
                button.interactable = false;
                texts[3].text = "未完成";
            }
            rewardItems[i].SetActive(true);
        }
        for (; i < length; i++)
        {
            rewardItems[i].SetActive(false);
        }
    }

    /* 学生作业：实现成就奖励领取按钮的响应函数GetReward以及相关PlayFab请求成功或者失败的回调函数
     * GetReward函数的两个参数解释：name表示成就奖励名称，value表示成就奖励的金币数。
     * 作业提示：
     * 首先，启用processingWindow窗口，提示正在处理玩家领取成就奖励的请求；
     * 其次，使用UpdateUserDataRequest函数，更新玩家的自定义属性Player Data,包括玩家已领取相关成就奖励的数据；
     * UpdateUserDataRequest函数调用成功后，使用GetUserDataRequest函数，重新获取玩家的自定义属性，保存在PlayFabUserData类的userData中；
     * 接着，使用AddUserVirtualCurrencyRequest函数，增加玩家的金币数量（该请求需要包含两个参数：VirtualCurrency表示增加的虚拟货币种类，Amount表示增加的虚拟货币数量）；
     * AddUserVirtualCurrencyRequest函数调用成功后，更新玩家金币数量的显示
     * 最后，若两个请求都调用成功，禁用processingWindow窗口，并更新成就奖励条目的显示。
     */

	//领取成就奖励
	void GetReward(string name,int value)
	{
		
	}

	//更新玩家数据Player Data（成就奖励已领取）成功后调用
	void OnUpdateUserRewardData(UpdateUserDataResult result)
	{
		
	}
	//玩家数据Player Data获取成功后（成就奖励信息的获取）调用
	void OnGetUserData2(GetUserDataResult result)
	{
		
	}
	//玩家虚拟货币增加成功时调用
	void OnAddUserVirtualCurrencyResult(ModifyUserVirtualCurrencyResult result)
	{
		
	}

    //上一页按钮
    public void ClickPreviousButton()
    {
        currentPageNumber--;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowAchievementItems();
    }
    //下一页按钮
    public void ClickNextButton()
    {
        currentPageNumber++;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowAchievementItems();
    }
}
