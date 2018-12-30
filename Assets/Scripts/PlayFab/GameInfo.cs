using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

//游戏相关信息
public class GameInfo : MonoBehaviour {

	public string[] gunNames;       //枪械名称
	public Sprite[] gunSprites;     //枪械图片
	public AudioClip[] gunAudios;   //枪械音效

    public static Dictionary<string, Sprite> guns;                  //枪械名称与图片的映射
    public static Dictionary<string, AudioClip> gunShootingAudios;  //枪械名称与音效的映射
    public static List<CatalogItem> catalogItems;                   //游戏道具数据列表

    public string[] mapNames;           //游戏地图中文名
    public string[] mapEnglishName;     //游戏地图英文名（对应场景名称）
    public Sprite[] mapSprites;         //游戏地图缩略图

    public static Dictionary<string, string> mapNameMappings;   //游戏地图中文名和英文名的映射
    public static Dictionary<string, Sprite> maps;              //游戏地图中文名和游戏地图缩略图的映射

    public string[] myLevelRankNames;       //玩家等级对应的军衔名称
    public int[] myLevelExps;               //玩家升级所需经验
    public Sprite[] myLevelMedals;          //玩家等级对应的勋章
    public static string[] levelRankNames;  //同myLevelRankNames，静态字段，方便其他类调用
    public static int[] levelExps;          //同myLevelExps，静态字段，方便其他类调用
    public static Sprite[] levelMedals;     //同myLevelMedals，静态字段，方便其他类调用

    public static Dictionary<string, string> titleData;     //PlayFab GameManager中存储的游戏数据

	void Start () {
        //设置枪支名称和枪支图片、枪支音效的映射
		guns = new Dictionary<string, Sprite> ();
		gunShootingAudios = new Dictionary<string, AudioClip> ();
		int length = gunNames.Length;
		for (int i = 0; i < length; i++) {
			guns.Add (gunNames [i], gunSprites [i]);
			gunShootingAudios.Add (gunNames [i], gunAudios [i]);
		}

        //设置地图中文名和英文名、缩略图的映射
        mapNameMappings = new Dictionary<string,string> ();
        maps = new Dictionary<string, Sprite>();
        length = mapNames.Length;
        for (int i = 0; i < length; i++)
        {
            mapNameMappings.Add(mapNames[i], mapEnglishName[i]);
            maps.Add(mapNames[i], mapSprites[i]);
        }

        //等级数据静态变量的赋值
        levelRankNames = myLevelRankNames;
        levelExps = myLevelExps;
        levelMedals = myLevelMedals;
	}
}
