using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
//玩家账号详细信息面板控制器
public class UserMessagePanelController : MonoBehaviour {

    public GameObject loginPanel;           //登录面板
    public GameObject mainPanel;            //游戏主面板
    public GameObject lobbyPanel;           //大厅面板
    public GameObject roomPanel;            //房间面板
    public GameObject userMessagePanel;     //玩家账号详细信息面板

    public Button backButton;       //返回按钮
    public Text currentPanel;       //当前面板文本信息

    public Text userId;             //PlayFab账号ID
    public Text currentRank;        //账号当前等级（军衔）
    public Text nextRank;           //账号下一等级（军衔）
    public Image currentMedal;      //账号当前等级对应的勋章
    public Image nextMedal;         //账号下一等级对应的勋章
    public Slider expSlider;        //账号经验条
    public Text expText;            //账号经验值

    public Text totalWin;                   //玩家胜利场次
    public Text totalKill;                  //玩家累计杀敌数
    public Text killPerDeath;               //玩家杀敌死亡比

    public Text modifyPasswordLabel;        //“修改密码”按钮点击后的反馈信息
    public GameObject processingWindow;     //“处理中”窗口

    //面板启用时调用，显示玩家账号信息
    void OnEnable () {
        modifyPasswordLabel.text = "";
        processingWindow.SetActive(false);
        currentPanel.text = "玩家";
        //为返回按钮绑定响应函数
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate ()
        {
            if (PhotonNetwork.inRoom)
                roomPanel.SetActive(true);
            else
                lobbyPanel.SetActive(true);                 //如果玩家在游戏房间中，点击返回按钮后，游戏界面显示游戏房间。
            userMessagePanel.SetActive(false);              //如果玩家在游戏大厅中，点击返回按钮后，游戏界面显示游戏大厅。
        });

        userId.text = "玩家ID："+PlayFabUserData.playFabId;    

        //根据玩家当前等级，显示军衔和勋章
        currentRank.text = GameInfo.levelRankNames[PlayFabUserData.lv - 1];
        if (PlayFabUserData.lv >= GameInfo.levelRankNames.Length)
            nextRank.text = "";
        else nextRank.text = GameInfo.levelRankNames[PlayFabUserData.lv];
        currentMedal.sprite = GameInfo.levelMedals[PlayFabUserData.lv - 1];
        if (PlayFabUserData.lv >= GameInfo.levelRankNames.Length)
        {
            nextMedal.gameObject.SetActive(false);
        }
        else
        {
            nextMedal.sprite = GameInfo.levelMedals[PlayFabUserData.lv];
            nextMedal.gameObject.SetActive(true);
        }
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
            expText.text = PlayFabUserData.exp.ToString()+"(已满级)";
        }

        //显示玩家的战斗数据
        totalWin.text = "胜利场次：" + PlayFabUserData.totalWin.ToString();
        totalKill.text = "累计杀敌数：" + PlayFabUserData.totalKill.ToString();
        killPerDeath.text = "杀敌死亡比：" + (PlayFabUserData.killPerDeath / 100).ToString("0.0");
    }

    //“修改密码”按钮的响应事件
    public void ClickModifyPasswordButton()
    {
        //判断账号是否绑定邮箱
        if (PlayFabUserData.email == null)
        {
            modifyPasswordLabel.text = "该账号未绑定邮箱，无法修改密码";
            return;
        }
        processingWindow.SetActive(true);

        //发送密码修改邮件至账号绑定邮箱
        SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
        {
            TitleId = PlayFabUserData.titleId,
            Email = PlayFabUserData.email
        };
        Debug.Log(PlayFabUserData.email);
        PlayFabClientAPI.SendAccountRecoveryEmail(request,OnSendAccountRecoveryEmail,OnPlayFabError);
    }

    //密码修改邮件发送成功后执行该函数，提示玩家密码修改邮件已发送至账号绑定邮箱
    void OnSendAccountRecoveryEmail(SendAccountRecoveryEmailResult result)
    {
        processingWindow.SetActive(false);
        modifyPasswordLabel.text = "密码修改邮件已发至账号绑定邮箱";
    }

    //PlayFab请求发生错误时调用，在控制台输出错误原因
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Get an error:" + error.Error);
    }

    //“注销”按钮的响应函数
    public void ClickLogoutButton()
    {
        PhotonNetwork.Disconnect();
        mainPanel.SetActive(false);
        loginPanel.SetActive(true);
    }
}
