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

    int requestNum = 4;
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

        requestNum = 4;

        GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(getUserDataRequest, OnGetUserData, OnPlayFabError);

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

    }

	void OnGetUserData(GetUserDataResult result){
		Debug.Log ("User Data Loaded");
        PlayFabUserData.userData = result.Data;

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

        if (result.Data.ContainsKey ("EquipedWeapon"))
			PlayFabUserData.equipedWeapon = result.Data["EquipedWeapon"].Value;
		else
			PlayFabUserData.equipedWeapon = "AK47";
        
        GetUserInventoryRequest getUserInventoryRequest = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(getUserInventoryRequest, OnGetUserInventory, OnPlayFabError);
    }

    void OnGetUserEntityData(GetObjectsResponse result)
    {
        Debug.Log("User Data Loaded");
        PlayFabUserData.userEntityData = result.Objects;

        if (result.Objects.ContainsKey("AchievementPoints"))
            PlayFabUserData.achievementPoints = int.Parse(result.Objects["AchievementPoints"].EscapedDataObject);
        else PlayFabUserData.achievementPoints = 0;
        if (result.Objects.ContainsKey("LV"))
            PlayFabUserData.lv = int.Parse(result.Objects["LV"].EscapedDataObject);
        else
            PlayFabUserData.lv = 1;
        lvValue.text = PlayFabUserData.lv.ToString();
        if (result.Objects.ContainsKey("Exp"))
            PlayFabUserData.exp = int.Parse(result.Objects["Exp"].EscapedDataObject);
        else PlayFabUserData.exp = 0;

        if (result.Objects.ContainsKey("TotalKill"))
            PlayFabUserData.totalKill = int.Parse(result.Objects["TotalKill"].EscapedDataObject);
        else
            PlayFabUserData.totalKill = 0;
        if (result.Objects.ContainsKey("TotalDeath"))
            PlayFabUserData.totalDeath = int.Parse(result.Objects["TotalDeath"].EscapedDataObject);
        else
            PlayFabUserData.totalDeath = 0;
        if (PlayFabUserData.totalDeath == 0)
            PlayFabUserData.killPerDeath = (float)PlayFabUserData.totalKill * 100.0f;
        else
            PlayFabUserData.killPerDeath = PlayFabUserData.totalKill * 100.0f / PlayFabUserData.totalDeath;

        if (result.Objects.ContainsKey("TotalWin"))
            PlayFabUserData.totalWin = int.Parse(result.Objects["TotalWin"].EscapedDataObject);
        else
            PlayFabUserData.totalWin = 0;
        if (result.Objects.ContainsKey("TotalGame"))
            PlayFabUserData.totalGame = int.Parse(result.Objects["TotalGame"].EscapedDataObject);
        else
            PlayFabUserData.totalGame = 0;
        if (PlayFabUserData.totalGame == 0)
            PlayFabUserData.winPercentage = 0.0f;
        else
            PlayFabUserData.winPercentage = PlayFabUserData.totalWin * 100.0f / PlayFabUserData.totalGame;

        if (result.Objects.ContainsKey("EquipedWeapon"))
            PlayFabUserData.equipedWeapon = result.Objects["EquipedWeapon"].EscapedDataObject;
        else
            PlayFabUserData.equipedWeapon = "AK47";

        GetUserInventoryRequest getUserInventoryRequest = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(getUserInventoryRequest, OnGetUserInventory, OnPlayFabError);
    }


    void OnGetUserInventory(GetUserInventoryResult result){
        Debug.Log("User Inventory Loaded");
		if (result.VirtualCurrency.Count == 0) {
			OnMessageResponse();
			return;
		}
		goldCurrencyCount.text = result.VirtualCurrency ["AU"].ToString();
        diamondCurrencyCount.text = result.VirtualCurrency["GM"].ToString();
		bool hasEquipedWeapon = false;
		foreach (ItemInstance i in result.Inventory) {
			if (i.ItemClass == PlayFabUserData.equipedWeapon) {
				hasEquipedWeapon = true;
				break;
			}
		}
        if (!hasEquipedWeapon)
        {
            PlayFabUserData.equipedWeapon = "AK47";

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
}
