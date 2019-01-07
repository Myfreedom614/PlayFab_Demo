

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
  server.UpdatePlayerStatistics(request)
}



handlers.SetupPlayerData = function (args:any,context: IPlayFabContext ) {
  let request : PlayFabDataModels.SetObjectsRequest={
    Entity: server.GetUserAccountInfo({PlayFabId:context.playerProfile.PlayerId}).UserInfo.TitleInfo.TitlePlayerAccount,
    Objects:[{
      ObjectName:"",
      DataObject:""
    },{
      ObjectName:"",
      DataObject:""
    }

    ]
  }
  entity.SetObjects(request)

  server.UpdateUserReadOnlyData({PlayFabId:context.playerProfile.PlayerId,Data:{
    "Achievement1":"false",
    "Achievement2":"false"
  },Permission: "Public"})
  
}

handlers.ExchangeGold = function(args)
{
  var DCCost = args.DC;
  var GCGet = args.GC;
  
  var subtractDCResult = server.SubtractUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "DC",
    Amount : DCCost
  });
  var DCResult = subtractDCResult["Balance"];
  
  var addGCResult = server.AddUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "GC",
    Amount : GCGet
  });
  var GCResult = addGCResult["Balance"];
  
  return { diamondCurrencyResult : DCResult,
        goldCurrencyResult : GCResult};
}

handlers.GetAchievement = function(args:any,context: IPlayFabContext){

}

handlers.ValidatePurchaseCode = function (args)
{
  var codeKey = "";
  var diamondPrice = 0;
  var result = false;
  switch(args)
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

  var finalDia = -1;
  var addDia = parseInt(codeKey.substr(12)); //截取数字
  var addResult = server.AddUserVirtualCurrency({
    PlayFabId : currentPlayerId,
    VirtualCurrency : "DC" ,
    Amount : addDia
  });

  return { purchaseResult : result ,
        newDiamondAmount : finalDia };
  
}