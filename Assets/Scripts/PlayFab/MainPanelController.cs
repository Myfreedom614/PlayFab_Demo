using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using PlayFab.AuthenticationModels;

public class MainPanelController : MonoBehaviour{

	public GameObject loginPanel;
	public GameObject mainPanel;

	public GameObject lobbyPanel;
	public GameObject shopPanel;
	public GameObject inventoryPanel;
	public GameObject leaderboardPanel;
    public GameObject achievementPanel;
    public GameObject roomPanel;
    public GameObject userMessagePanel;
    public GameObject paymentPanel;


	//public GameObject dataLoadingWindow;
	public GameObject userMessage;
    public Text lvValue;
    public Text usernameText;
	public GameObject currency;
    public Text goldCurrencyCount;
    public Text diamondCurrencyCount;
    //public GameObject pageSwitchButton;

    int requestNum = 7;
	void OnEnable(){

        //dataLoadingWindow.SetActive (true);
		userMessage.SetActive (false);
        currency.SetActive(false);
        //pageSwitchButton.SetActive(false);

        if (lobbyPanel != null)
			lobbyPanel.SetActive (true);
		if (shopPanel != null)
			shopPanel.SetActive (false);
		if (inventoryPanel != null)
			inventoryPanel.SetActive (false);

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

        GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
        //PlayFabClientAPI.GetUserData(getUserDataRequest, OnGetUserData, OnPlayFabError);

        var getUserEntityRequest = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = PlayFabAuthService.entityId, Type = PlayFabAuthService.entityType } };
        PlayFabDataAPI.GetObjects(getUserEntityRequest, OnGetUserEntityData, OnPlayFabError);

        GetCatalogItemsRequest getCatalogItemsrequest = new GetCatalogItemsRequest()
        {
            CatalogVersion = PlayFabUserData.catalogVersion
        };
        PlayFabClientAPI.GetCatalogItems(getCatalogItemsrequest, OnGetCatalogItems, OnPlayFabError);

        GetAccountInfoRequest getAccountInfoRequest = new GetAccountInfoRequest()
        {
            PlayFabId = PlayFabUserData.playFabId
        };
        PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest, OnGetAccountInfo, OnPlayFabError);

        GetTitleDataRequest getTitleDataRequest = new GetTitleDataRequest();

        PlayFabClientAPI.GetTitleData(getTitleDataRequest, OnGetTitleData, OnPlayFabError);
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest (){ StatisticNames= new List<string>(){ "TotalWin", "TotalKill", "KillPerDeath" } },OnGetPlayerStatistics, OnPlayFabError);
        PlayFabClientAPI.GetUserReadOnlyData(getUserDataRequest, OnGetUserReadOnlyData, OnPlayFabError);
    }


    void OnGetUserEntityData(GetObjectsResponse result)
    {
        Debug.Log("User Data Loaded");
        var dataObject = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(result.Objects["PlayerData"].DataObject.ToString());
        PlayFabUserData.userEntityData = dataObject;

        if (dataObject.ContainsKey("LV")) { 
            PlayFabUserData.lv = int.Parse(dataObject["LV"].ToString());
            lvValue.text = dataObject["LV"].ToString();
        }

        else
            PlayFabUserData.lv = 1;
        lvValue.text = PlayFabUserData.lv.ToString();

        if (dataObject.ContainsKey("Exp"))
            PlayFabUserData.exp = int.Parse(dataObject["Exp"].ToString());
        else PlayFabUserData.exp = 0;


        if (dataObject.ContainsKey("EquipedWeapon"))
            PlayFabUserData.equipedWeapon = dataObject["EquipedWeapon"].ToString();
        else
            PlayFabUserData.equipedWeapon = "weapon20";

        GetUserInventoryRequest getUserInventoryRequest = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(getUserInventoryRequest, OnGetUserInventory, OnPlayFabError);
        OnMessageResponse();

    }

    void OnGetUserReadOnlyData(GetUserDataResult result)
    {
        if (result.Data.ContainsKey("Achievement"))
        {
            PlayFabUserData.userReadonlyData = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<Dictionary<string, bool>>(result.Data["Achievement"].Value);
        }
        OnMessageResponse();

    }

    void OnGetPlayerStatistics(GetPlayerStatisticsResult result)
    {
        foreach (var Statistic in result.Statistics)
        {
            if (Statistic.StatisticName == "TotalWin")
            {
                PlayFabUserData.totalWin = Statistic.Value;
            }
            else if(Statistic.StatisticName == "TotalKill")
            {
                PlayFabUserData.totalKill = Statistic.Value;
            }
            else
            {
                PlayFabUserData.killPerDeath = Statistic.Value;
            }
        }
        OnMessageResponse();

    }
    void OnGetUserInventory(GetUserInventoryResult result){
        Debug.Log("User Inventory Loaded");
		if (result.VirtualCurrency.Count == 0) {
			OnMessageResponse();
			return;
		}
        PlayFabUserData.diamondCurrencyCount = result.VirtualCurrency["GM"];
        PlayFabUserData.goldCurrencyCount = result.VirtualCurrency["AU"];


        goldCurrencyCount.text = result.VirtualCurrency ["AU"].ToString();
        diamondCurrencyCount.text = result.VirtualCurrency["GM"].ToString();

        OnMessageResponse();

    }

    void OnUpdateUserData(UpdateUserDataResult result)
    {
        Debug.Log("User Data Saved");
    }

	void OnGetCatalogItems(GetCatalogItemsResult result){
        GameInfo.catalogItems = result.Catalog;
        OnMessageResponse();
    }

    void OnGetAccountInfo(GetAccountInfoResult result)
    {
        PlayFabUserData.email = result.AccountInfo.PrivateInfo.Email;
        PlayFabUserData.username = result.AccountInfo.TitleInfo.DisplayName;
        usernameText.text = result.AccountInfo.TitleInfo.DisplayName;
        OnMessageResponse();
    }

    void OnGetTitleData(GetTitleDataResult result)
    {
        GameInfo.titleData = result.Data;

        OnMessageResponse();
    }
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error);
    }

    void OnMessageResponse()
    {
        requestNum--;
        if (requestNum == 0)
        {
            //dataLoadingWindow.SetActive(false);
            userMessage.SetActive(true);
            currency.SetActive(true);
            //pageSwitchButton.SetActive(true);
            lobbyPanel.SetActive(true);
        }
    }

    public void disableAllPanel()
    {
        lobbyPanel.SetActive(false);
        shopPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        achievementPanel.SetActive(false);
        roomPanel.SetActive(false);
        userMessagePanel.SetActive(false);
        paymentPanel.SetActive(false);
    }
    
	public void ClickLobbyButton(){
        disableAllPanel();
        lobbyPanel.SetActive(true);
	}
    public void ClickShopButton(){
        disableAllPanel();
		shopPanel.SetActive (true);
    }
    public void ClickInventoryButton()
    {
        disableAllPanel();
        inventoryPanel.SetActive(true);
    }

    public void ClickLeaderBoardButton()
    {
        disableAllPanel();
        leaderboardPanel.SetActive(true);
    }
    public void ClickAchievementButton()
    {
        disableAllPanel();
        achievementPanel.SetActive(true);
    }
    public void ClickUserMessage()
    {
        disableAllPanel();
        userMessagePanel.SetActive(true);
    }
    public void ClickPaymentPanelButton()
    {
        disableAllPanel();
        paymentPanel.SetActive(true);
    }
}
