using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
public class AchievementPanelController : MonoBehaviour {


    public GameObject achievementPanel;

    public Button achievementButton;
    public GameObject achievement;
    public GameObject[] achievementItems;
    public GameObject previousButton;
    public GameObject nextButton;
    public Text pageMessage;
    public GameObject processingWindow;

    public Text goldCurrencyCount;

    private List<string> achievementKeys;
    private Dictionary<string, string> achievementData;
    private int itemsLength;
    private const int itemsPerPage = 8;
    private int currentPageNumber;
    private int maxPageNumber;

    private List<string> rewardKeys;
    private Dictionary<string, string> rewardData;

    private int requestNum;                         

    void OnEnable()
    {
        processingWindow.SetActive(false);
        
        Init(); 

        achievementButton.Select(); 
        currentPageNumber = 1;
        maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();

        ButtonControl();
        ShowAchievementItems();
    }

    void Init()
    {
        achievementData = new Dictionary<string, string>(); 
        achievementKeys = new List<string>();
        foreach (KeyValuePair<string, string> kvp in GameInfo.titleData)
        {
            if (kvp.Key.Contains("Achievement"))
            {
                achievementData.Add(kvp.Key,kvp.Value);
                achievementKeys.Add(kvp.Key);
            }
        }
        itemsLength = achievementData.Count;

        rewardData = new Dictionary<string, string>();
        rewardKeys = new List<string>();

        foreach (KeyValuePair<string, string> kvp in GameInfo.titleData)
        {
            if (kvp.Key.Contains("Reward"))
            {
                rewardData.Add(kvp.Key, kvp.Value);
                rewardKeys.Add(kvp.Key);
            }
        }
    }


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

    void ShowAchievementItems()
    {
        int start, end, i, j;
        start = (currentPageNumber - 1) * itemsPerPage;
        if (currentPageNumber == maxPageNumber)
            end = itemsLength;
        else
            end = start + itemsPerPage;
        Text[] texts;
        Button button;
        Dictionary<string, string> AchievementItemData;

        for (i = start, j = 0; i < end; i++, j++)
        {
            string achievementName = achievementKeys[i];
            texts = achievementItems[j].GetComponentsInChildren<Text>();
            button = achievementItems[j].GetComponentInChildren<Button>();
            AchievementItemData = PlayFabSimpleJson.DeserializeObject<Dictionary<string, string>>(achievementData[achievementName]);
            texts[0].text = AchievementItemData["Name"];
            texts[1].text = AchievementItemData["Description"] + "：" + AchievementItemData["Count"];
            texts[2].text = AchievementItemData["Reward"];
            int point = int.Parse(AchievementItemData["Reward"]);

            if (PlayFabUserData.userData.ContainsKey(achievementName))
            {
                texts[3].text = "Received";      
                button.interactable = false;
            }
            else if((AchievementItemData["Description"] == "TotalKill" && PlayFabUserData.totalKill >= int.Parse(AchievementItemData["Count"])) 
                || (AchievementItemData["Description"] == "TotalWin" && PlayFabUserData.totalWin >= int.Parse(AchievementItemData["Count"])))
            {
                texts[3].text = "Get";
                button.interactable = true;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate ()
                {
                    GetAchievement(achievementName,point); 
                });
            }
            else
            {
                texts[3].text = "UnCompleted";
                button.interactable = false;
            }

            achievementItems[j].SetActive(true);
        }
        for (; j < itemsPerPage; j++)
            achievementItems[j].SetActive(false);
    }
    

    void GetAchievement(string name,int point)  
    {
        processingWindow.SetActive(true);

        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest()
        {
            Body = new Dictionary<string, object>() {
                { "Achievement", name}
            },
            EventName = "player_get_achievement"
        },
        result => 
            PlayFabClientAPI.UpdateUserData(
                new UpdateUserDataRequest() { Data = new Dictionary<string, string> { { name, "true" } } },
            OnUpdateUserData,
            OnPlayFabError),
        OnPlayFabError);
    }

    void OnUpdateUserData(UpdateUserDataResult result)
    {
        GetUserDataRequest request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnGetUserData, OnPlayFabError);
    }

    void OnGetUserData(GetUserDataResult result) {
        PlayFabUserData.userData = result.Data;
        processingWindow.SetActive(false);
        ShowAchievementItems();
    }

    void onWritePlayerEvent(WriteEventResponse result)
    {
        UpdateUserDataRequest request = new UpdateUserDataRequest();
        request.Data = new Dictionary<string, string>();
        request.Data.Add(name, "true");
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {Data = new Dictionary<string, string> {name, "true" } }, OnUpdateUserData, OnPlayFabError);
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error); 
        processingWindow.SetActive(false);
    }


    public void ClickPreviousButton()
    {
        currentPageNumber--;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowAchievementItems();
    }
    public void ClickNextButton()
    {
        currentPageNumber++;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowAchievementItems();
    }
}
