using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
//金币购买界面控制
public class BuyGoldPageController : MonoBehaviour {
    
    public GameObject processPanel;
    public Text hintLabel;
    public GameObject backButton;
    public Text singlePaymentDiamondCost;
    public Text singlePaymentGoldGet;
    public Text multiplePaymentGoldGet;

    public Text goldCurrencyCount;
    public Text diamondCurrencyCount;

    /* 钻石兑换金币的功能（ClickSingleBuyGoldButton和ClickMultipleBuyGoldButton函数的实现）
     * 检测玩家钻石数量（diamondCurrencyCount）是否充足
     * 使用PlayFabClientAPI.ExecuteCloudScript函数，调用CloudScript的ExchangeGold函数，
     * ExchangeGold函数的调用需要两个参数DC与GC，分别表示消耗的钻石数量和获得的金币数量。
     * ExchangeGold函数调用成功，执行OnExecuteCloudScript函数，提示玩家购买成功，更新玩家的货币数量
     * ExchangeGold函数调用失败，执行OnPlayFabError函数，提示玩家购买失败，在控制台输出错误原因
     */

	//“单次购买”按钮的响应事件
	public void ClickSingleBuyGoldButton()
	{
		
	}


    //ExecuteCloudScript执行成功时，执行该函数
    void OnExecuteCloudScript(ExecuteCloudScriptResult result)
    {
        //获取ExchangeGold函数的返回值，包括玩家的钻石和金币的数量
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object diamondCurrencyResult, goldCurrencyResult;
        jsonResult.TryGetValue("diamondCurrencyResult", out diamondCurrencyResult);
        jsonResult.TryGetValue("goldCurrencyResult", out goldCurrencyResult);

        //更新玩家钻石和金币的数量
        diamondCurrencyCount.text = diamondCurrencyResult.ToString();
        goldCurrencyCount.text = goldCurrencyResult.ToString();

        //提示玩家金币购买成功
        hintLabel.text = "购买成功";
        backButton.SetActive(true);
    }

    //ExecuteCloudScript执行失败时，执行该函数
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError(error.Error);

        //提示玩家金币购买失败
        hintLabel.text = "购买失败";
		backButton.SetActive(true);
    }
}
