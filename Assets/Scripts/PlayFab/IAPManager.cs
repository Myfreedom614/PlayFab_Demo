using UnityEngine;
using System.Collections;
using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Text.RegularExpressions;
//钻石个数枚举
public enum CodeType
{
    Dia300, Dia980, Dia1980, Dia3280
}
//钻石购买、钻石兑换码验证的实现
public class IAPManager : MonoBehaviour {

    //根据购买类型获取对应购买url链接
    //param1 购买类型
    //return 购买的url链接
    public static string GetPurchaseUrl(CodeType type)
    {
        string url = "";
        switch (type)
        {
            case CodeType.Dia300:
                url = "https://item.taobao.com/item.htm?spm=686.1000925.0.0.OX3wJs&id=538456436251";
                break;
            case CodeType.Dia980:
                url = "https://item.taobao.com/item.htm?spm=686.1000925.0.0.OX3wJs&id=538564769192";
                break;
            case CodeType.Dia1980:
                url = "https://item.taobao.com/item.htm?spm=686.1000925.0.0.OX3wJs&id=538517818959";
                break;
            case CodeType.Dia3280:
                url = "https://item.taobao.com/item.htm?spm=686.1000925.0.0.OX3wJs&id=538599832203";
                break;
            default:
                url = "https://item.taobao.com/item.htm?spm=686.1000925.0.0.OX3wJs&id=538599832203";
                break;
        }
        return url;
    }

    //跳转到淘宝App界面
    public static void JumpToPurchaseApp(CodeType type)
    {
#if (UNITY_ANDROID)
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("jumpToPurchaseApp", GetPurchaseUrl(type));
#endif
    }

    //客户端购买码校验
    //目前验证方式：购买码必须为7位并且只有数字
    //param1 code 购买码
    public static bool clientPurchaseCheck(string code)
    {
        if (code == null) return false;
        if (code.Length == 7 && Regex.Matches(code, "[^0-9]").Count == 0)
            return true;
        else
            return false;

    }
    //将输入的购买码兑换为钻石
    //param1 code：购买码  param2 successCallBack兑换成功回调
    //param3 failCallBack 兑换失败回调
    //param4 errorCallBack 服务器出错回调
    public static void AsyncExchangeCodesToDiamond(string code,Action<int>successCallBack,Action failCallBack,Action<PlayFabError>errorCallBack = null)
    {
        if (code == null) code = "";
        if (!clientPurchaseCheck(code))
        {
            if (failCallBack != null)
                failCallBack();
            return ;
        }
        //发起调用CloudScript函数的请求：validatePurchaseCode函数，用于验证钻石兑换码
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "validatePurchaseCode", 
            FunctionParameter = new { codes = code },
            GeneratePlayStreamEvent = true, 
        };
        PlayFabClientAPI.ExecuteCloudScript(
			request,       
            //执行成功后，获得钻石兑换的结果
			(result) =>{
            	JsonObject jsonResult = (JsonObject)result.FunctionResult;
				object bResult; object sNewDm;
            	jsonResult.TryGetValue("purchaseResult", out bResult);
            	jsonResult.TryGetValue("newDiamondAmount", out sNewDm);
            	if ((bool)bResult){
                	if (successCallBack != null)
                    	successCallBack((int)((System.UInt64)sNewDm));
            	}
            	else{
                	if (failCallBack != null)
                    	failCallBack();
            	}
			}, 
            //执行失败，提示错误
			(error) =>{
            	if (errorCallBack != null)
                	errorCallBack(error);
        	}
		);
        return ;
    }
}
