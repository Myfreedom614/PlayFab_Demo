using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

//游戏相关信息
public class GameInfo {

	public string[] gunNames;

    public static List<CatalogItem> catalogItems;


    public string[] myLevelRankNames;       //玩家等级对应的军衔名称
    public int[] myLevelExps;               //玩家升级所需经验
    public Sprite[] myLevelMedals;          //玩家等级对应的勋章
    public static string[] levelRankNames;  //同myLevelRankNames，静态字段，方便其他类调用
    public static int[] levelExps;          //同myLevelExps，静态字段，方便其他类调用
    public static Sprite[] levelMedals;     //同myLevelMedals，静态字段，方便其他类调用

    public static Dictionary<string, string> titleData;     //PlayFab GameManager中存储的游戏数据

}
