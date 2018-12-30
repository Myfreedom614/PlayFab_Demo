using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

//玩家仓库界面控制器
public class InventoryPanelController : MonoBehaviour {

	public GameObject lobbyPanel;               //大厅面板
    public GameObject roomPanel;                //房间面板
    public GameObject inventoryPanel;           //玩家仓库面板
    public GameObject inventoryLoadingWindow;   //玩家仓库加载中提示窗口
	public Button backButton;                   //返回按钮
    public Text currentPanel;                   //当前面板文本信息

    public GameObject[] inventoryItems;         //道具条目
    public GameObject previousButton;           //上一页按钮
	public GameObject nextButton;               //下一页按钮
	public Text pageMessage;                    //页面信息
	public GameObject inventoryItemDetails;     //道具条目的详细信息

	public static int selectedItem;             //选中的道具
	public static List<ItemInstance> items;     //玩家的道具信息
	public static bool isEquiped;               //玩家是否切换装备的道具

	private int itemsLength;                    //玩家拥有的道具总数
	private const int itemsPerPage = 6;         //每页显示的道具个数
	private int currentPageNumber;              //当前页数
	private int maxPageNumber;                  //总页数

    //玩家仓库界面启用后，初始化仓库界面信息
	void OnEnable () {
        currentPanel.text = "仓 库";
		isEquiped = false;
		inventoryLoadingWindow.SetActive (true);        //提示仓库数据正在加载
		inventoryItemDetails.SetActive (false);
		foreach (GameObject go in inventoryItems) {
			go.SetActive (false);
		}

        //获取玩家仓库信息
		GetUserInventoryRequest request = new GetUserInventoryRequest ();
		PlayFabClientAPI.GetUserInventory (request, OnGetUserInventory, OnPlayFabError);

        //为返回按钮绑定响应函数
		backButton.onClick.RemoveAllListeners ();
		backButton.onClick.AddListener (delegate {
            if (PhotonNetwork.inRoom)           //如果玩家在游戏房间中，点击返回按钮后，游戏界面显示游戏房间。
                roomPanel.SetActive(true);
            else                                //如果玩家在游戏大厅中，点击返回按钮后，游戏界面显示游戏大厅。
                lobbyPanel.SetActive(true);
			inventoryPanel.SetActive (false);
		});
	}

    //玩家仓库信息获取成功后调用，在仓库界面显示玩家拥有的道具
	void OnGetUserInventory(GetUserInventoryResult result){
		items = result.Inventory;       //玩家的道具列表
		itemsLength = items.Count;
		currentPageNumber = 1;
		maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();               //翻页按钮控制
		ShowItems ();                   //显示玩家的道具列表
		inventoryLoadingWindow.SetActive (false);   //仓库信息读取完毕，禁用提示面板
	}

    //翻页按钮控制
	void ButtonControl(){
		if (currentPageNumber == 1)
			previousButton.SetActive (false);
		else
			previousButton.SetActive (true);
		if (currentPageNumber == maxPageNumber)
			nextButton.SetActive (false);
		else
			nextButton.SetActive (true);
	}

    //显示玩家的道具列表
	void ShowItems(){
		int start, end, i, j;
		start = (currentPageNumber - 1) * itemsPerPage;
		if (currentPageNumber == maxPageNumber)
			end = itemsLength;
		else
			end = start + itemsPerPage;
		for (i = start,j = 0; i < end; i++,j++) {
			int itemNum = i;

			Text itemName = inventoryItems [j].transform.Find ("Name").GetComponent<Text>();        //道具名称
			Image image = inventoryItems [j].transform.Find ("ItemImage").GetComponent<Image>();    //道具图片
			Text equip = inventoryItems [j].transform.Find ("Equip").GetComponent<Text>();          //道具是否装备
			Button button = inventoryItems [j].transform.Find ("Button").GetComponent<Button>();    //道具装备按钮

			itemName.text = items [i].DisplayName;
			image.sprite = GameInfo.guns [items [i].ItemClass];
			if (PlayFabUserData.equipedWeapon == items [i].ItemClass) {
				equip.gameObject.SetActive(true);
				button.gameObject.SetActive (false);
			} else {
				equip.gameObject.SetActive(false);
				button.gameObject.SetActive (true);
                //为“装备”按钮绑定响应事件
				button.onClick.RemoveAllListeners ();
				button.onClick.AddListener (delegate {
					selectedItem = itemNum;
					inventoryItemDetails.SetActive (true);
				});
			}
			inventoryItems [j].SetActive (true);
		}
		for (; j < itemsPerPage; j++)
			inventoryItems [j].SetActive (false);
	}

    //PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error){
		Debug.LogError (error.ErrorDetails);
	}

    //上一页按钮
    public void ClickPreviousButton(){
		currentPageNumber--;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();
		ShowItems ();
	}
    //下一页按钮
    public void ClickNextButton(){
		currentPageNumber++;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();
		ShowItems ();
	}

    //如果玩家更新了装备道具，更新玩家仓库信息的显示
	void Update(){
		if (isEquiped) {
			ShowItems ();
			isEquiped = false;
		}
	}
}
