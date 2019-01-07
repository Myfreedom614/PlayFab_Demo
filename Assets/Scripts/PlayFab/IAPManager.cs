using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Text.RegularExpressions;
public enum CodeType
{
    Dia300, Dia680, Dia1280, Dia3280
}
public class IAPManager : MonoBehaviour {

    public Text diamondCurrencyCount;

    public static int GetPurchaseNum(CodeType type)
    {
        int num = 0;
        switch (type)
        {
            case CodeType.Dia300:
                num = 300;
                break;
            case CodeType.Dia680:
                num = 680;
                break;
            case CodeType.Dia1280:
                num = 1280;
                break;
            case CodeType.Dia3280:
                num = 3280;
                break;
            default:
                num = 300;
                break;
        }
        return num;
    }




    public void OnGetDiamondFailed()
    {
    }

    public void OnError(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
    }


    public static void PurchaseDiamond(CodeType type,Action<int>successCallBack,Action failCallBack,Action<PlayFabError>errorCallBack = null)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "PurchaseDiamond", 
            FunctionParameter = new { num = type },
            GeneratePlayStreamEvent = true, 
        };
        PlayFabClientAPI.ExecuteCloudScript(
			request,       
			(result) =>{
            	JsonObject jsonResult = (JsonObject)result.FunctionResult;
				object bResult; object sNewDm;
            	jsonResult.TryGetValue("purchaseResult", out bResult);
            	jsonResult.TryGetValue("newDiamondAmount", out sNewDm);
            	if ((bool)bResult)
                {
                    successCallBack?.Invoke((int)(ulong)sNewDm);
                }
                else
                {
                	if (failCallBack != null)
                    	failCallBack();
            	}
			}, 
			(error) =>{
            	if (errorCallBack != null)
                	errorCallBack(error);
        	}
		);
        return ;
    }

    public void OnGetDiamondSuccess(int newDiamond)
    {
        diamondCurrencyCount.text = newDiamond.ToString();
        PlayFabUserData.diamondCurrencyCount = newDiamond;
    }

    public void Click300DiaButton()
    {
        PurchaseDiamond(CodeType.Dia300, OnGetDiamondSuccess, OnGetDiamondFailed, OnError);

    }
    public void Click680DiaButton()
    {
        PurchaseDiamond(CodeType.Dia680, OnGetDiamondSuccess, OnGetDiamondFailed, OnError);

    }
    public void Click1280DiaButton()
    {
        PurchaseDiamond(CodeType.Dia1280, OnGetDiamondSuccess, OnGetDiamondFailed, OnError);

    }
    public void Click3280DiaButton()
    {
        PurchaseDiamond(CodeType.Dia3280, OnGetDiamondSuccess, OnGetDiamondFailed, OnError);

    }
}
