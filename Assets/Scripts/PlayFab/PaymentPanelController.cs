using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//货币兑换面板控制器
public class PaymentPanelController : MonoBehaviour {

    public GameObject paymentPanel;         //货币兑换面板
    public Button buyGoldButton;            //“购买金币”按钮
    public GameObject buyGoldPage;          //“购买金币”页面
    public GameObject buyDiamondPage;       //“购买钻石”页面
    public GameObject exchangeDiamondPage;  //“兑换钻石”页面
    public GameObject processPanel;         //“处理中”提示面板
    
    //货币兑换面板启用时调用
	void OnEnable()
    {
        //默认显示“购买金币”页面
        buyGoldButton.Select();
        ClickBuyGoldButton();
        processPanel.SetActive(false);
    }
    //点击右上角的货币界面，显示货币兑换面板
    public void ClickPaymentPanelButton()
    {
        paymentPanel.SetActive(true);
    }

    //“购买金币”按钮的响应函数
    public void ClickBuyGoldButton()
    {
        buyGoldPage.SetActive(true);
        buyDiamondPage.SetActive(false);
        exchangeDiamondPage.SetActive(false);
    }
    //“购买钻石”按钮的响应函数
    public void ClickBuyDiamondButton()
    {
        buyGoldPage.SetActive(false);
        buyDiamondPage.SetActive(true);
        exchangeDiamondPage.SetActive(false);
    }
    //“兑换钻石”按钮的响应函数
    public void ClickExchangeDiamondButton()
    {
        buyGoldPage.SetActive(false);
        buyDiamondPage.SetActive(false);
        exchangeDiamondPage.SetActive(true);
    }
    
    //退出货币兑换面板
    public void ClickExitButton()
    {
        paymentPanel.SetActive(false);
    }
    //点击返回按钮，退出“处理中”提示面板
    public void ClickBackButton()
    {
        processPanel.SetActive(false);
    }
}
