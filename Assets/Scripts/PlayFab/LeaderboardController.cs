using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
//玩家排行榜界面控制器
public class LeaderboardController : MonoBehaviour {

	public GameObject lobbyPanel;               //大厅面板
    public GameObject roomPanel;                //房间面板
    public GameObject leaderboardPanel;         //玩家排行榜面板
    public GameObject leaderboardLoadingLabel;  //玩家排行版加载提示


    public Button TotalKillButton;              //“累计杀敌数”排行榜按钮

    public GameObject[] users;                  //前三玩家信息
    public GameObject localUser;                //本地玩家信息

    public Sprite metalCourage;                 //鼓励奖牌

	private Dictionary<string,uint> leaderboard = new Dictionary<string, uint> ();  //用于存储PlayFab获取的排行榜
	private string leaderboardType="";          //排行榜类型（累计杀敌数/杀敌死亡比/胜利场次/）
    private Text[] localUserTexts;              //本地玩家的数据显示
    private Image localUserImage;               //本地玩家的奖牌

    //玩家排行榜界面启用时调用，初始化排行榜界面
	void OnEnable () {
        localUserTexts = localUser.GetComponentsInChildren<Text>();
        localUserTexts[1].text = PlayFabUserData.username;
        localUserImage = localUser.GetComponentsInChildren<Image>()[1];

        //默认显示玩家的累计杀敌数排行榜
        TotalKillButton.Select();
		ClickTotalKillButton ();

    }
    /* 学生作业：实现排行榜按钮的响应函数
     * 作业提示：
     * 首先，启用leaderboardLoadingLabel，提示排行榜正在加载；
     * 其次，禁用排行榜中的前三名和本地玩家信息面板，在排行榜加载过程中不应该显示这些信息；
     * 接着，设置leaderboardType字段，标志显示的是什么数据的排行榜，在之后的SetLeadboard设置排行榜界面信息时会用到该参数；
     * 然后，初始化本地玩家信息面板，包括玩家的排名（初始化为‘-’，表示未进入排行榜）、该项统计数据的显示以及玩家的奖牌（初始化为metalCourage）；
     * 最后，使用GetLeaderboardRequest声明账号登录请求，再使用PlayFabClientAPI.GetLeaderboard，获取玩家排行榜信息；
     * （GetLeaderboardRequest需要设置两个参数，MaxResultsCount表示获取排行榜前MaxResultsCount名玩家的信息，StatisticName表示玩家排行榜的统计数据）
     * 获取成功，调用OnGetLeaderboard函数，将排行榜信息显示玩家排行榜界面。
     * 获取失败，根据错误类型提示玩家排行榜获取失败原因，在控制台输出错误信息
     */

    //“累计杀敌数”按钮的响应函数
    public void ClickTotalKillButton(){
		
	}

    //“杀敌死亡比”按钮的响应函数
	public void ClickKillPerDeathButton(){
		
	}

    //“胜利场次”按钮的响应函数
	public void ClickTotalWinButton(){
		
	}

    //排行榜数据获取成功后调用此函数，在界面显示排行榜信息
	void OnGetLeaderboard(GetLeaderboardResult result){
		leaderboard.Clear ();   //清空排行榜数据
        //填入排行榜数据
		foreach (PlayerLeaderboardEntry entry in result.Leaderboard) {
			leaderboard.Add (entry.DisplayName, (uint)entry.StatValue);
		}
		SetLeadboard ();        //设置排行榜界面
	}

    //设置排行榜界面
	void SetLeadboard(){
		leaderboardLoadingLabel.SetActive (false);
		int i = 0;
        Text[] texts;
        //遍历排行榜信息
		foreach (KeyValuePair<string,uint>kvp in leaderboard) {
			texts = users [i].GetComponentsInChildren<Text> ();
            //填入排行榜的玩家信息
			texts [0].text = (i + 1).ToString();
			texts [1].text = kvp.Key;
			if (leaderboardType == "累计杀敌数" || leaderboardType == "胜利场数")
				texts [2].text = leaderboardType+"："+kvp.Value.ToString ();
			else if (leaderboardType == "杀敌死亡比")        //注：PlayFab的统计数据Statistics只能存储整数数据，在存储时放大了10000倍，这里需要将获取的值除以10000
				texts [2].text = leaderboardType + "：" + (kvp.Value / 10000.0f).ToString ("0.0");
            //如果玩家进入了前三名
            if (kvp.Key == PlayFabUserData.username)
            {
                localUserTexts[0].text = texts[0].text;
                localUserTexts[2].text = texts[2].text;
                localUserImage.sprite = users[i].GetComponentsInChildren<Image>()[1].sprite;    //将玩家的奖牌更新成前三名的奖牌
            }
            users [i].SetActive (true);
			i++;
		}
        localUser.SetActive(true);
	}

    //PlayFab请求出错时调用，在控制台输出错误信息
    void OnPlayFabError(PlayFabError error){
		Debug.LogError ("Get an error:" + error.Error);
	}
}
