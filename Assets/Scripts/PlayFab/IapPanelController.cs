using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

//钻石购买、钻石兑换码控制器
public class IapPanelController : MonoBehaviour {
    
    public InputField codeInputField;   //钻石兑换码的输入框
    public GameObject processPanel;     //“处理中”面板
    public Text hintLabel;              //提示信息
    public GameObject backButton;       //返回按钮

    public Text diamondCurrencyCount;   //钻石数量的显示文本

    //点击钻石购买按钮，跳转到相应的淘宝链接
    public void ClickOneGetCodeButton(CodeType type) {
        IAPManager.JumpToPurchaseApp(type);
    }

    //提示“处理中”
    void ResetProcessPanel()
    {
        processPanel.SetActive(true);
        hintLabel.text = "处 理 中...";
        backButton.SetActive(false);
    }

    public void ClickConfirmInputButton() {
        ResetProcessPanel();
        string code = codeInputField.text;
        IAPManager.AsyncExchangeCodesToDiamond(code, OnGetDiamondSuccess, OnGetDiamondFailed, OnError);
    }

    //钻石购买按钮的响应函数
    public void Click300DiaButton() {
        ClickOneGetCodeButton(CodeType.Dia300);
    }
    public void Click980DiaButton()  {
        ClickOneGetCodeButton(CodeType.Dia980);
    }
    public void Click1980DiaButton() {
        ClickOneGetCodeButton(CodeType.Dia1980);
    }
    public void Click3280DiaButton() {
        ClickOneGetCodeButton(CodeType.Dia3280);
    }

    //钻石兑换成功时调用，更新钻石数量的显示，提示获取成功
    public void OnGetDiamondSuccess(int newDiamond) {
        diamondCurrencyCount.text = newDiamond.ToString();
        hintLabel.text = "获取成功";
        backButton.SetActive(true);
    }
    //钻石兑换失败时调用，提示钻石码是无效的
    public void OnGetDiamondFailed()
    {
        hintLabel.text = "无效的钻石码";
        backButton.SetActive(true);
    }
    //PlayFab请求发生错误时调用，在控制台输出错误信息
    public void OnError(PlayFabError error) {
        Debug.LogError(error.ErrorDetails);
    }
}
