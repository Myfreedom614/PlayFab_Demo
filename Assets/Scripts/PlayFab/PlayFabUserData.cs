using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;

public class PlayFabUserData : MonoBehaviour {

	public static string equipedWeapon="47";
	public static string catalogVersion = "GunWeapon";

    public static string playFabId = "";
	public static string username ="";
    public static string email = "";

    public static Dictionary<string, UserDataRecord> userData;
    public static Dictionary<string, object> userEntityData;

    public static int Number = 0;
    public static int achievementPoints;

    public static int lv = 0;
    public static int exp = 0;

	public static int totalKill = 0;
	public static int totalDeath = 0;
	public static float killPerDeath = 0.0f;

	public static int totalWin = 0;
	public static int totalGame = 0;
	public static float winPercentage = 0.0f;
}
