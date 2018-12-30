using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
//仓库道具详细信息面板控制器
public class InventoryItemDetailsController : MonoBehaviour {

	public Image itemImage;         //道具图片
	public Text itemName;           //道具名字
	public Text itemDescription;    //道具描述
	public Button equipButton;      //装备按钮

	private ItemInstance item;      //道具的实例
	Dictionary<string,string>customData;

    //仓库道具详细信息面板启用时调用
	void OnEnable(){
        //获得玩家选中的条目
		item = InventoryPanelController.items [InventoryPanelController.selectedItem];
		itemImage.sprite = GameInfo.guns [item.ItemClass];  //显示道具的图片
		itemName.text = item.DisplayName;                   //显示道具的名字
		itemDescription.text = "";
        //根据PlayFab GameManager中设置的自定义道具属性，显示道具的描述
		foreach (CatalogItem i in GameInfo.catalogItems) {
			if (i.ItemId == item.ItemId) {
				customData = PlayFabSimpleJson.DeserializeObject<Dictionary<string,string>> (i.CustomData);
				foreach(KeyValuePair<string,string>kvp in customData)
					itemDescription.text += kvp.Key + ":" + kvp.Value + "\n";
				break;
			}
		}
		
        //为装备按钮绑定响应函数
		equipButton.onClick.RemoveAllListeners ();
		equipButton.onClick.AddListener (delegate {
			PlayFabUserData.equipedWeapon = item.ItemClass;         //更新PlayFabUserData保存的装备道具信息
            //更新PlayFab GameManager的Player Data：玩家装备的道具
			UpdateUserDataRequest request = new UpdateUserDataRequest (){ };
			request.Data = new Dictionary<string,string> ();
			request.Data.Add ("EquipedWeapon", item.ItemClass);
			PlayFabClientAPI.UpdateUserData (request, OnUpdateUserData, OnPlayFabError);
			InventoryPanelController.isEquiped = true;
			gameObject.SetActive (false);       //道具装备后,关闭道具详细信息面板
		});
	}

    //PlayFab GameManager的Player Data更新成功时调用（这里不作任何处理）
    void OnUpdateUserData(UpdateUserDataResult result){
		//Debug.Log ("Player Data Saved");
	}

    //PlayFab请求出错时调用，在控制台输出错误信息
	void OnPlayFabError(PlayFabError error){
		Debug.LogError (error.ErrorDetails);
	}

    //取消道具的装备
	public void ClickCancelButton(){
		gameObject.SetActive (false);
	}
}
