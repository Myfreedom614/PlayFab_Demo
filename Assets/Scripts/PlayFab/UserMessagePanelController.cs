using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class UserMessagePanelController : MonoBehaviour {

    public GameObject userMessagePanel;

    public Text userId;
    public Text currentRank;
    public Text nextRank;

    public Slider expSlider;
    public Text expText;

    public Text totalWin;
    public Text totalKill;
    public Text killPerDeath;

    public Text modifyPasswordLabel;
    public GameObject processingWindow;

    void OnEnable () {
        modifyPasswordLabel.text = "";
        processingWindow.SetActive(false);

        userId.text = "Player ID："+PlayFabUserData.playFabId;    

        currentRank.text = GameInfo.levelRankNames[PlayFabUserData.lv - 1];
        if (PlayFabUserData.lv >= GameInfo.levelRankNames.Length)
            nextRank.text = "";
        else nextRank.text = GameInfo.levelRankNames[PlayFabUserData.lv];

        if (PlayFabUserData.lv < GameInfo.levelExps.Length)
        {
            expSlider.minValue = 0;
            expSlider.maxValue = GameInfo.levelExps[PlayFabUserData.lv - 1];
            expSlider.value = PlayFabUserData.exp;
            expText.text = PlayFabUserData.exp.ToString() + "/" + GameInfo.levelExps[PlayFabUserData.lv - 1].ToString();
        }
        else
        {
            expSlider.minValue = 0;
            expSlider.maxValue = 1;
            expSlider.value = 1;
            expText.text = PlayFabUserData.exp.ToString()+"(Full Level)";
        }

        totalWin.text = "Total Win：" + PlayFabUserData.totalWin.ToString();
        totalKill.text = "Total Kill：" + PlayFabUserData.totalKill.ToString();
        killPerDeath.text = "Kill Per Death：" + (PlayFabUserData.killPerDeath / 100).ToString("0.0");
    }

    public void ClickModifyPasswordButton()
    {
        if (PlayFabUserData.email == null)
        {
            modifyPasswordLabel.text = "Password can't be changed if the account isn't bound to the mailbox,";
            return;
        }
        processingWindow.SetActive(true);

        SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Email = PlayFabUserData.email
        };
        Debug.Log(PlayFabUserData.email);
        PlayFabClientAPI.SendAccountRecoveryEmail(request,OnSendAccountRecoveryEmail,OnPlayFabError);
    }

    void OnSendAccountRecoveryEmail(SendAccountRecoveryEmailResult result)
    {
        processingWindow.SetActive(false);
        modifyPasswordLabel.text = "The Email used to change the password has been sent to the account mailbox.";
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error);
    }

    public void ClickLogoutButton()
    {
        Photon.Pun.PhotonNetwork.Disconnect();
        PlayFabAuthenticationAPI.ForgetAllCredentials();
    }
}
