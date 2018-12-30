using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
//主面板控制器
public class MainPanelController {

	public GameObject loginPanel;			//游戏登录面板
	public GameObject mainPanel;			//游戏主面板

	public GameObject lobbyPanel;           //游戏大厅面板
	public GameObject shopPanel;            //游戏商城面板
	public GameObject inventoryPanel;       //玩家仓库面板
    public GameObject talentPanel;          //天赋系统面板
	public GameObject leaderboardPanel;     //玩家排行榜面板
    public GameObject achievementPanel;     //成就系统面板
    public GameObject roomPanel;            //游戏房间面板
    public GameObject userMessagePanel;     //玩家信息面板
    public GameObject paymentPanel;         //货币兑换面板

	public GameObject dataLoadingWindow;    //“数据加载中”提示窗口
	public GameObject userMessage;          //玩家简要信息
    public Text lvValue;                    //玩家简要信息 - 玩家等级
    public Text usernameText;               //玩家简要信息 - 玩家昵称
	public GameObject panelMessage;         //当前显示面板的信息提示
	public GameObject currency;             //玩家游戏货币面板
    public Text goldCurrencyCount;          //玩家金币数量文本
    public Text diamondCurrencyCount;       //玩家钻石数量文本
    public GameObject pageSwitchButton;     //页面切换按钮集合

    int requestNum = 4;
	//当游戏大厅面板启用时调用，初始化信息
	void OnEnable(){
        //显示鼠标（玩家退出战斗后，重新加载游戏场景）
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dataLoadingWindow.SetActive (true);
		userMessage.SetActive (false);
        panelMessage.SetActive(false);
        currency.SetActive(false);
        pageSwitchButton.SetActive (false);

        //禁用所有面板
		if (lobbyPanel != null)
			lobbyPanel.SetActive (false);
		if (shopPanel != null)
			shopPanel.SetActive (false);
		if (inventoryPanel != null)
			inventoryPanel.SetActive (false);
        if (talentPanel != null)
            talentPanel.SetActive(false);
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        if (achievementPanel != null)
            achievementPanel.SetActive(false);
        if (roomPanel != null)
            roomPanel.SetActive(false);
        if (userMessagePanel != null)
            userMessagePanel.SetActive(false);
        if (paymentPanel != null)
            paymentPanel.SetActive(false);

        //玩家登录后，需要同时向PlayFab发起4个请求
        requestNum = 4;

        //获取玩家数据Player Data
		GetUserDataRequest getUserDataRequest = new GetUserDataRequest ();
		PlayFabClientAPI.GetUserData (getUserDataRequest, OnGetUserData, OnPlayFabError);

        //获取游戏道具的信息
        GetCatalogItemsRequest getCatalogItemsrequest = new GetCatalogItemsRequest()
        {
            CatalogVersion = PlayFabUserData.catalogVersion //武器道具：GunWeapon
        };
        PlayFabClientAPI.GetCatalogItems(getCatalogItemsrequest, OnGetCatalogItems, OnPlayFabError);

        //获取玩家账户信息
        GetAccountInfoRequest getAccountInfoRequest = new GetAccountInfoRequest()
        {
            PlayFabId = PlayFabUserData.playFabId
        };
        PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest, OnGetAccountInfo, OnPlayFabError);

        //获取游戏数据Title Data
        GetTitleDataRequest getTitleDataRequest = new GetTitleDataRequest();
        PlayFabClientAPI.GetTitleData(getTitleDataRequest, OnGetTitleData, OnPlayFabError);
    }

    //玩家数据Player Data获取成功时调用
	void OnGetUserData(GetUserDataResult result){
		Debug.Log ("User Data Loaded");
        //在本地保存玩家数据
        PlayFabUserData.userData = result.Data;

        //成就系统相关数据保存
        if (result.Data.ContainsKey("AchievementPoints"))
            PlayFabUserData.achievementPoints = int.Parse(result.Data["AchievementPoints"].Value);
        else PlayFabUserData.achievementPoints = 0;
        if (result.Data.ContainsKey("LV"))
            PlayFabUserData.lv = int.Parse(result.Data["LV"].Value);
        else
            PlayFabUserData.lv = 1;
        lvValue.text = PlayFabUserData.lv.ToString();
        if (result.Data.ContainsKey("Exp"))
            PlayFabUserData.exp = int.Parse(result.Data["Exp"].Value);
        else PlayFabUserData.exp = 0;

        //天赋系统相关数据保存
        if (result.Data.ContainsKey("ExpAndMoneySkillLV"))
            PlayFabUserData.expAndMoneySkillLV = int.Parse(result.Data["ExpAndMoneySkillLV"].Value);
        else PlayFabUserData.expAndMoneySkillLV = 0;
        if (result.Data.ContainsKey("ShootingRangeSkillLV"))
            PlayFabUserData.shootingRangeSkillLV = int.Parse(result.Data["ShootingRangeSkillLV"].Value);
        else PlayFabUserData.shootingRangeSkillLV = 0;
        if (result.Data.ContainsKey("ShootingIntervalSkillLV"))
            PlayFabUserData.shootingIntervalSkillLV = int.Parse(result.Data["ShootingIntervalSkillLV"].Value);
        else PlayFabUserData.shootingIntervalSkillLV = 0;
        if (result.Data.ContainsKey("ShootingDamageSkillLV"))
            PlayFabUserData.shootingDamageSkillLV = int.Parse(result.Data["ShootingDamageSkillLV"].Value);
        else PlayFabUserData.shootingDamageSkillLV = 0;

        //玩家战斗数据保存
		if (result.Data.ContainsKey ("TotalKill"))
			PlayFabUserData.totalKill = int.Parse (result.Data ["TotalKill"].Value);
		else
			PlayFabUserData.totalKill = 0;
		if (result.Data.ContainsKey ("TotalDeath"))
			PlayFabUserData.totalDeath = int.Parse (result.Data ["TotalDeath"].Value);
		else
			PlayFabUserData.totalDeath = 0;
		if (PlayFabUserData.totalDeath == 0)
			PlayFabUserData.killPerDeath = (float)PlayFabUserData.totalKill*100.0f;
		else
			PlayFabUserData.killPerDeath = PlayFabUserData.totalKill * 100.0f / PlayFabUserData.totalDeath;

		if (result.Data.ContainsKey ("TotalWin"))
			PlayFabUserData.totalWin = int.Parse (result.Data ["TotalWin"].Value);
		else
			PlayFabUserData.totalWin = 0;
		if (result.Data.ContainsKey ("TotalGame"))
			PlayFabUserData.totalGame = int.Parse (result.Data ["TotalGame"].Value);
		else
			PlayFabUserData.totalGame = 0;
		if (PlayFabUserData.totalGame == 0)
			PlayFabUserData.winPercentage = 0.0f;
		else
			PlayFabUserData.winPercentage = PlayFabUserData.totalWin * 100.0f / PlayFabUserData.totalGame;

        //玩家装备的道具
        if (result.Data.ContainsKey ("EquipedWeapon"))
			PlayFabUserData.equipedWeapon = result.Data["EquipedWeapon"].Value;
		else
			PlayFabUserData.equipedWeapon = "AK47";
        
        //获取玩家的仓库数据
        GetUserInventoryRequest getUserInventoryRequest = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(getUserInventoryRequest, OnGetUserInventory, OnPlayFabError);
    }

    //玩家仓库数据获取成功时调用
	void OnGetUserInventory(GetUserInventoryResult result){
        Debug.Log("User Inventory Loaded");
		if (result.VirtualCurrency.Count == 0) {
			OnMessageResponse();
			return;
		}
        //显示玩家的金币、钻石数量
		goldCurrencyCount.text = result.VirtualCurrency ["GC"].ToString();
        diamondCurrencyCount.text = result.VirtualCurrency["DC"].ToString();
        //检测玩家是否拥有装备道具
		bool hasEquipedWeapon = false;
		foreach (ItemInstance i in result.Inventory) {
			if (i.ItemClass == PlayFabUserData.equipedWeapon) {
				hasEquipedWeapon = true;
				break;
			}
		}
        //如果玩家未拥有装备的道具（超出使用期限）
        if (!hasEquipedWeapon)
        {
            PlayFabUserData.equipedWeapon = "AK47";

            //更新玩家属性Player Data“EquipedWeapon”
            UpdateUserDataRequest request = new UpdateUserDataRequest();
            request.Data = new Dictionary<string, string>();
            request.Data.Add("EquipedWeapon", PlayFabUserData.equipedWeapon);
            PlayFabClientAPI.UpdateUserData(request, OnUpdateUserData, OnPlayFabError);
        }
        else
        {
            OnMessageResponse();
            Debug.Log("User Data Saved");
        }

        OnMessageResponse();    //PlayFab的数据是否接收完毕

    }

    //玩家属性更新成功后调用
    void OnUpdateUserData(UpdateUserDataResult result)
    {
        Debug.Log("User Data Saved");
    }

    //游戏道具数据接收成功后调用
	void OnGetCatalogItems(GetCatalogItemsResult result){
        //在GameInfo中保存游戏道具信息
        GameInfo.catalogItems = result.Catalog;
        OnMessageResponse();    //PlayFab的数据是否接收完毕
    }

    //玩家账号信息接收成功后调用
    void OnGetAccountInfo(GetAccountInfoResult result)
    {
        //在PlayFabUserData中保存玩家邮箱信息
        PlayFabUserData.email = result.AccountInfo.PrivateInfo.Email;
        OnMessageResponse();    //PlayFab的数据是否接收完毕
    }

    //游戏数据接收成功后调用
    void OnGetTitleData(GetTitleDataResult result)
    {
        //在GameInfo中保存游戏数据
        GameInfo.titleData = result.Data;
        OnMessageResponse();    //PlayFab的数据是否接收完毕
    }

    //PlayFab请求出错时调用，在控制台输出错误信息
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error);
    }

    //PlayFab的数据是否接收完毕
    void OnMessageResponse()
    {
        requestNum--;
        if (requestNum == 0)        //PlayFab的数据已接收完毕，在游戏主面板显示游戏大厅以及其他信息
        {
            dataLoadingWindow.SetActive(false);
            userMessage.SetActive(true);
            panelMessage.SetActive(true);
            currency.SetActive(true);
            pageSwitchButton.SetActive(true);
            lobbyPanel.SetActive(true);
        }
    }

    //禁用所有面板
    public void disableAllPanel()
    {
        lobbyPanel.SetActive(false);
        shopPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        talentPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        achievementPanel.SetActive(false);
        roomPanel.SetActive(false);
        userMessagePanel.SetActive(false);
    }
    
    //“大厅”按钮的响应函数
	public void ClickLobbyButton(){
        disableAllPanel();
        lobbyPanel.SetActive(true);
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
	}
    //“商城”按钮的响应函数
    public void ClickShopButton(){
        disableAllPanel();
		shopPanel.SetActive (true);
    }
    //“仓库”按钮的响应函数
    public void ClickInventoryButton()
    {
        disableAllPanel();
        inventoryPanel.SetActive(true);
    }
    //“天赋”按钮的响应函数
    public void ClickTalentButton()
    {
        disableAllPanel();
        talentPanel.SetActive(true);
    }
    //“排行”按钮的响应函数
    public void ClickLeaderBoardButton()
    {
        disableAllPanel();
        leaderboardPanel.SetActive(true);
    }
    //“成就”按钮的响应函数
    public void ClickAchievementButton()
    {
        disableAllPanel();
        achievementPanel.SetActive(true);
    }
    //点击左下角的玩家信息，显示玩家信息面板
    public void ClickUserMessage()
    {
        disableAllPanel();
        userMessagePanel.SetActive(true);
    }
}
