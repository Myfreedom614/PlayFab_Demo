using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
//PlayFab相关的游戏货玩家账号数据的本地存储
public class PlayFabUserData : MonoBehaviour {

	public static string equipedWeapon="AK47";              //玩家装备的枪支
	public static string catalogVersion = "GunWeapon";      //游戏道具的目录名

    public static string playFabId = "";        //玩家账号的ID
	public static string username ="";          //玩家账号的用户名
    public static string email = "";            //玩家账号的绑定邮箱

    public static Dictionary<string, UserDataRecord> userData;  //PlayFab GameManager中存储的玩家自定义数据Player Data

    public static int achievementPoints;        //玩家成就点数

    public static int lv = 0;               //玩家等级
    public static int exp = 0;              //玩家经验值

    //玩家天赋技能等级
    public static int expAndMoneySkillLV = 0;           //"土豪"技能
    public static int shootingRangeSkillLV = 0;         //“精通射程”技能
    public static int shootingIntervalSkillLV = 0;      //“精通射速”技能
    public static int shootingDamageSkillLV = 0;        //“精通伤害”技能

    //玩家的战斗数据
	public static int totalKill = 0;            //累计杀敌数
	public static int totalDeath = 0;           //累计死亡数
	public static float killPerDeath = 0.0f;    //杀敌死亡比

	public static int totalWin = 0;             //胜利场次
	public static int totalGame = 0;            //战斗总场次
	public static float winPercentage = 0.0f;   //胜利场次比
}
