using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
public class LeaderboardController : MonoBehaviour {

    public GameObject leaderboardPanel;
    public GameObject leaderboardLoadingLabel;


    public Button TotalKillButton;
    public Button KillPerDeathButton;
    public Button TotalWinButton;

    public GameObject[] users;
    public GameObject localUser;


	private Dictionary<string,uint> leaderboard = new Dictionary<string, uint> ();
	private string leaderboardType="";
    private Text[] localUserTexts;

	void OnEnable () {
        localUserTexts = localUser.GetComponentsInChildren<Text>();
        localUserTexts[1].text = PlayFabUserData.username;

        TotalKillButton.Select();
		ClickTotalKillButton ();
    }


    public void ClickTotalKillButton(){
        leaderboardLoadingLabel.SetActive(true);

        leaderboardType = "TotalKill";
        Text[] texts;
        texts = localUser.GetComponentsInChildren<Text>();

        texts[0].text = "-";
        texts[2].text = "-";

        foreach (var user in users)
        {
            texts = user.GetComponentsInChildren<Text>();
            texts[0].text = "-";
            texts[1].text = "-";
            texts[2].text = "-";
        }

        var leaderboardRequest = new GetLeaderboardRequest() {
            MaxResultsCount = 3,
            StatisticName = "TotalKill",StartPosition=0,
            ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowAvatarUrl = true
                }
        };
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnGetLeaderboard, OnPlayFabError);
    }

	public void ClickKillPerDeathButton(){
        leaderboardLoadingLabel.SetActive(true);

        leaderboardType = "KillPerDeath";
        Text[] texts;
        texts = localUser.GetComponentsInChildren<Text>();

        texts[0].text = "-";
        texts[1].text = PlayFabUserData.username;
        texts[2].text = "-";

        foreach (var user in users)
        {
            texts = user.GetComponentsInChildren<Text>();
            texts[0].text = "-";
            texts[1].text = "-";
            texts[2].text = "-";
        }

        var leaderboardRequest = new GetLeaderboardRequest()
        {
            MaxResultsCount = 3,
            StatisticName = "KillPerDeath",
            StartPosition = 0,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnGetLeaderboard, OnPlayFabError);
    }

	public void ClickTotalWinButton(){
        leaderboardLoadingLabel.SetActive(true);

        leaderboardType = "TotalWin";
        Text[] texts;
        texts = localUser.GetComponentsInChildren<Text>();

        texts[0].text = "-";
        texts[1].text = PlayFabUserData.username;
        texts[2].text = "-";

        foreach (var user in users)
        {
            texts = user.GetComponentsInChildren<Text>();
            texts[0].text = "-";
            texts[1].text = "-";
            texts[2].text = "-";
        }

        var leaderboardRequest = new GetLeaderboardRequest()
        {
            MaxResultsCount = 3,
            StatisticName = "TotalWin",
            StartPosition = 0,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true            }
        };
        PlayFabClientAPI.GetLeaderboard(leaderboardRequest, OnGetLeaderboard, OnPlayFabError);
    }

	void OnGetLeaderboard(GetLeaderboardResult result){
		leaderboard.Clear ();

		foreach (PlayerLeaderboardEntry entry in result.Leaderboard) {
            if (entry.DisplayName == null)
            {
                leaderboard.Add(entry.PlayFabId, (uint)entry.StatValue);
            }
            else
            {
                leaderboard.Add(entry.DisplayName, (uint)entry.StatValue);
            }
        }
		SetLeadboard ();
	}


	void SetLeadboard(){
		leaderboardLoadingLabel.SetActive (false);
		int i = 0;
        Text[] texts;
		foreach (KeyValuePair<string,uint>kvp in leaderboard) {
			texts = users [i].GetComponentsInChildren<Text> ();
			texts [0].text = (i + 1).ToString();
			texts [1].text = kvp.Key;
			if (leaderboardType == "TotalKill" || leaderboardType == "TotalWin")
				texts [2].text = leaderboardType+"："+kvp.Value.ToString ();
			else if (leaderboardType == "KillPerDeath")
				texts [2].text = leaderboardType + "：" + (kvp.Value / 10000.0f).ToString ("0.0");
            if (kvp.Key == PlayFabUserData.username)
            {
                localUserTexts[0].text = texts[0].text;
                localUserTexts[2].text = texts[2].text;
            }
            users [i].SetActive (true);
			i++;
		}
        localUser.SetActive(true);
	}

    void OnPlayFabError(PlayFabError error){
		Debug.LogError ("Get an error:" + error.Error);
	}
}
