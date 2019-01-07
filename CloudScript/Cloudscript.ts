

handlers.SetupPlayerLeaderBoard = function (args:any,context: IPlayFabContext ) {
  let request: PlayFabServerModels.UpdatePlayerStatisticsRequest = {
    PlayFabId: context.playerProfile.PlayerId,
    Statistics: [{
      StatisticName: "TotalWin",
      Value: Math.floor(Math.random()*100)+1
  },{
    StatisticName: "TotalKill",
    Value: Math.floor(Math.random()*1000)+1
  },{
    StatisticName: "KillPerDeath",
    Value: Math.floor(Math.random()*10)+1
  }
]
  }

  server.UpdatePlayerStatistics(request, )
}

handlers.ExchangeGold = function(args)
{
  //函数的两个参数，DC表示消耗的钻石数，GC表示获得的金币数
  var DCCost = args.DC;
  var GCGet = args.GC;
  
  //调用Server的SubtractUserVirtualCurrency函数，降低玩家的钻石数
  var subtractDCResult = server.SubtractUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "DC",
    Amount : DCCost
  });
  var DCResult = subtractDCResult["Balance"];
  
  //调用Server的AddUserVirtualCurrency函数，增加玩家的金币数
  var addGCResult = server.AddUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "GC",
    Amount : GCGet
  });
  var GCResult = addGCResult["Balance"];
  
  //金币购买完成，返回玩家当前的钻石数和金币数
  return { diamondCurrencyResult : DCResult,
        goldCurrencyResult : GCResult};
}

handlers.validatePurchaseCode = function (args)
{
  var oneCode = args.codes;//用户输入
  var codeKey = "";//钻石兑换码类型
  var diamondPrice = 0;//价格
  var result = false;//兑换结果
  switch(oneCode.substring(0,1))
  {
    case '0':
      codeKey = "PurchaseCode300";
      diamondPrice = 30;
      break;
   	case '1':
      codeKey = "PurchaseCode980";
      diamondPrice = 90;
      break;
   	case '2':
      codeKey = "PurchaseCode1980";
      diamondPrice = 180;
      break;
   	case '3':
      codeKey = "PurchaseCode3280";
      diamondPrice = 300;
      break;
  }
  var titledata = server.GetTitleInternalData({
  		keys:[codeKey]
  }); 
  var codeStr = titledata.Data[codeKey];//获取该类型钻石兑换码
  if(codeStr == null) codeStr = "";
  var index = codeStr.search(oneCode);
  var finalDia = -1;
  if(index>=0){        // a valid code,该钻石兑换码已经使用，删除该钻石兑换码
  		if(index +8 > codeStr.length){ //搜索得到的钻石兑换码是最后一个
          if(index == 0)//钻石兑换码是最后一个也是第一个
            codeStr = "";
          else
            codeStr = codeStr.substr(0,index-1);
      }
      else{
      		if(index == 0){//钻石兑换码是第一个
            codeStr = codeStr.substr(index+8);
          }
          else{//钻石兑换码在中间的情况
            codeStr = codeStr.substr(0,index)+codeStr.substr(index+8);     
          }
      }
    result = true;
        
    var addDia = parseInt(codeKey.substr(12)); //截取“PurchaseCode300”后面的数字“300”，表示兑换获得的钻石数量
    //调用PlayFab Server的AddUserVirtualCurrency函数，增加玩家的钻石数量
    var addResult = server.AddUserVirtualCurrency({
      PlayFabId : currentPlayerId,
      VirtualCurrency : "DC" ,
      Amount : addDia
    });
    finalDia = addResult["Balance"];
    //调用PlayFab Server的SetTitleInternalData函数，设置游戏的TitleInternalData
    server.SetTitleInternalData({
    	key : codeKey,
    	value : codeStr
  	 });
    //调用CloudScript的函数generateExchangeDiamondEvent，在PlayStream生成钻石兑换的游戏事件
    handlers.generateExchangeDiamondEvent({
    	 diamond : addDia,
       paymentType : "TaoBao",
       price : diamondPrice,
       quantity : 1
    })
  }

  return { purchaseResult : result ,  //购买结果 true or false
        newDiamondAmount : finalDia }; //最终钻石数量 若失败返回-1
  
}