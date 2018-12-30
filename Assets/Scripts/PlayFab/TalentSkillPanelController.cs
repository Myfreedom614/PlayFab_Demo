using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

/* 学生作业：
 * 天赋系统面板控制器TalenetSkillPanelController脚本实现
 * 作业提示：请同学们参照每个函数的注释完成作业
 */
public class TalentSkillPanelController : MonoBehaviour {

    public GameObject lobbyPanel;           //大厅面板
    public GameObject roomPanel;            //房间面板
    public GameObject talentSkillPanel;     //天赋系统面板
    public Button backButton;               //返回按钮
    public Text currentPanel;               //当前面板文本信息

    public GameObject[] maskImages;     //天赋技能遮挡面板，用于遮挡未激活的天赋技能
    public Sprite[] skillSprites;       //天赋技能图片

    public Image skillImage;            //选中的天赋技能图片
    public Text skillName;              //选中的天赋技能名称
    public Text skillLevel;             //选中的天赋技能等级
    public Text skillDescription;       //选中的天赋技能描述
    public Text currentLevel;           //当前等级描述
    public Text nextLevel;              //下一等级描述
    public Text goldCurrencyCost;       //提升技能消耗的金币数
    public Button levelUpButton;        //天赋技能提升按钮

    public Button expAndMoneySkillButton;       //“土豪”天赋技能按钮
    public GameObject levelUpPanel;             //天赋技能升级中面板
    public GameObject levelUpPanelBackButton;   //天赋技能升级中面板的返回按钮
    public Text goldCurrencyCount;              //玩家金币数量的显示

    string currentSkillName;        
    string currentGoldCurrency;     

    //天赋技能面板启用时调用
    void OnEnable () {
		/* OnEnable函数实现步骤
		 * 1. 在currentPanel提示当前面板为“天 赋”面板
		 * 2. 为返回按钮button绑定事件：点击后切换游戏房间
		 * 		如果玩家在游戏房间中，点击返回按钮后，游戏界面显示游戏房间。
		 * 		如果玩家在游戏大厅中，点击返回按钮后，游戏界面显示游戏大厅。
		 * 3. 根据玩家的等级，启用天赋技能遮挡面板，提示天赋技能未激活
		 * 4. 显示“土豪”天赋技能
		 */
    }

    //天赋技能按钮的响应函数
    public void ClickExpAndMoneySkillButton()
    {
        currentSkillName = "ExpAndMoneySkill";
        UpdateTalentSkill(skillSprites[0]);
    }
    public void ClickShootingRangeSkillButton()
    {
        currentSkillName = "ShootingRangeSkill";
        UpdateTalentSkill(skillSprites[1]);
    }
    public void ClickShootingIntervalSkillButton()
    {
        currentSkillName = "ShootingIntervalSkill";
        UpdateTalentSkill(skillSprites[2]);
    }
    public void ClickShootingDamageSkillButton()
    {
        currentSkillName = "ShootingDamageSkill";
        UpdateTalentSkill(skillSprites[3]);
    }
    
    //更新选中的天赋技能描述
    void UpdateTalentSkill(Sprite skillSprite)
    {
		/* UpdateTalentSkill函数实现步骤：(天赋技能数据已经由MainControllerPanel脚本获取，保存在GameInfo.titleData中)
		 * 1. 在游戏界面右侧的天赋技能详细信息面板显示天赋技能图标和名字
		 * 2. 从GameInfo.titleData中获取天赋技能的详细信息（使用PlayFabSimpeJson.DeserializeObject解析Json数据）
		 * 3. 根据选中的天赋技能，显示该天赋技能的描述
		 * 4. 根据玩家天赋技能的等级（PlayFabUserData类保存了玩家的天赋技能等级），显示玩家天赋技能等级和对应的加成属性
		 * 5. 根据玩家天赋技能的等级，显示天赋技能下一等级数据，以及天赋技能升级需要的金币（注意处理天赋技能满级的情况）
		 * 6. 根据玩家天赋技能等级，为天赋技能提升按钮LevelUpButton绑定事件函数，以及按钮是否可以交互
		 * 		如果玩家天赋技鞥已满级：禁用按钮交互，按钮显示“已满级”
		 * 		如果玩家天赋技能未解锁，禁用按钮交互，按钮显示“未解锁”
		 * 		如果玩家可以提升天赋技能，启用按钮交互，按钮显示“提升”，为按钮动态绑定天赋技能提升相关的实现代码：
		 * 			第一步，启用levelUpPanel面板，用于提示玩家天赋技能的提升状况
         *          第二部，判断玩家金币数量是否足够：
         *          若金币不足，在levelUpPanel面板提示玩家“金币不足，提升失败”，结束函数执行。
         *          若金币足够，在levelUpPanel面板提示玩家"天赋提升中..."，并使用ExecuteCloudScript函数，调用CloudScript的UpgradeTalentSkill函数，完成玩家天赋技能的提升。
         *         （UpgradeTalentSkill函数包含三个参数，skillName表示提升的天赋技能名称，skillLevel表示玩家当前天赋技能等级，upgradeCost表示玩家天赋技能提升消耗的金币数）。
         *          ExecuteCloudScript函数执行成功，调用OnExecuteCloudScript函数，获取函数执行的结果，更新玩家的天赋技能等级以及玩家当前金币数，最后使用GetUserData函数，重新获取玩家的自定义属性Player Data。
         *          GetUserData函数执行成功，调用OnGetUserData函数，将玩家自定义属性Player Data保存在PlayFabUserData类的userData字段中，在levelUpPanel面板提示玩家“天赋技能提升成功”，最后根据currentSkillName字段更新天赋技能面板的显示。
         *          ExecuteCloudScript函数或GetUserData函数执行失败，调用OnPlayFabError函数，在levelUpPanel面板提示玩家“天赋技能提升失败”，并将失败原因输出到控制台。
         */
	}

	//CloudScript函数执行成功后调用
	void OnExecuteCloudScript(ExecuteCloudScriptResult result)
	{
		
	}

	//玩家数据获取成功后调用
	void OnGetUserData(GetUserDataResult result)
	{
		
	}

    //PlayFab请求失败时调用，在控制台输出错误原因，告知玩家天赋技能提升失败
    void OnPlayFabError(PlayFabError error)
    {
		
    }

    //关闭天赋技能提升中面板
    public void ClickLevelUpPanelBackButton()
    {
        levelUpPanel.SetActive(false);
    }
}
