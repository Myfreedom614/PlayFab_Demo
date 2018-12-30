using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
//商城道具详细信息面板控制器
public class ShopItemDetailsController : MonoBehaviour
{
    public GameObject shopPanel;            //商城面板
    public GameObject purchasePanel;        //道具购买面板
    public Text purchaseLabel;              //道具购买提示信息
    public GameObject BackButton;           //返回按钮

    public Image itemImage;                 //道具图片
    public Text itemName;                   //道具名字
    public Text itemDescription;            //道具描述
    public Image currencyImage;             //货币图片
    public Text price;                      //道具价格
    public Button confirmButton;            //确认购买按钮
    public Text goldCurrencyCount;          //玩家金币数量显示
    public Text diamondCurrencyCount;       //玩家钻石数量显示

    public Sprite goldCurrencySprite;       //金币图片
    public Sprite diamondCurrencySprite;    //钻石图片

    private CatalogItem item;
    Dictionary<string, string> customData;

    //商城道具详细信息面板启用时调用，显示选中道具的详细信息
    void OnEnable()
    {
        purchasePanel.SetActive(false);
        BackButton.SetActive(false);
        //显示选中道具的详细信息
        item = ShopPanelController.shopItems[ShopPanelController.selectedItem];
        itemImage.sprite = GameInfo.guns[item.ItemClass];
        itemName.text = item.DisplayName;
        //根据道具购买所使用的的货币，更新货币图片和数量的显示
        if (item.VirtualCurrencyPrices.ContainsKey("GC"))
        {
            price.text = item.VirtualCurrencyPrices["GC"].ToString();
            currencyImage.sprite = goldCurrencySprite;
        }
        else if (item.VirtualCurrencyPrices.ContainsKey("DC"))
        {
            price.text = item.VirtualCurrencyPrices["DC"].ToString();
            currencyImage.sprite = diamondCurrencySprite;
        }
        else price.text = "";

        //显示道具的详细信息（PlayFab GameManager存储的道具自定义属性）
        customData = PlayFabSimpleJson.DeserializeObject<Dictionary<string, string>>(item.CustomData);
        itemDescription.text = "";
        foreach (KeyValuePair<string, string> kvp in customData)
            itemDescription.text += kvp.Key + ":" + kvp.Value + "\n";

        //为购买按钮绑定购买道具的事件
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(delegate {
            if (item.VirtualCurrencyPrices.ContainsKey("GC"))
            {
                PurchaseItemRequest request = new PurchaseItemRequest()
                {
                    CatalogVersion = PlayFabUserData.catalogVersion,
                    VirtualCurrency = "GC",
                    Price = (int)item.VirtualCurrencyPrices["GC"],
                    ItemId = item.ItemId
                };
                PlayFabClientAPI.PurchaseItem(request, OnPurchaseItem, OnPlayFabPurchaseError);
                purchasePanel.SetActive(true);
                purchaseLabel.text = "商品购买中 ...";
            }
            else if (item.VirtualCurrencyPrices.ContainsKey("DC"))
            {
				//...
                PurchaseItemRequest request = new PurchaseItemRequest()
                {
                    CatalogVersion = PlayFabUserData.catalogVersion,
                    VirtualCurrency = "DC",
                    Price = (int)item.VirtualCurrencyPrices["DC"],
                    ItemId = item.ItemId
                };
                PlayFabClientAPI.PurchaseItem(request, OnPurchaseItem, OnPlayFabPurchaseError);
                purchasePanel.SetActive(true);
                purchaseLabel.text = "商品购买中 ...";
            }
        });
    }

    //道具购买成功后调用，重新获取玩家仓库信息
    void OnPurchaseItem(PurchaseItemResult result)
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetUserInventory, OnPlayFabError);
    }

    //玩家仓库信息获取成功时调用，玩家道具购买完成
    void OnGetUserInventory(GetUserInventoryResult result)
    {
        //更新货币数量的显示
        goldCurrencyCount.text = result.VirtualCurrency["GC"].ToString();
        diamondCurrencyCount.text = result.VirtualCurrency["DC"].ToString();
        //提示玩家道具购买成功
        purchaseLabel.text = "购买成功";
        BackButton.SetActive(true);
        //重新获取玩家仓库
        ShopPanelController.userItems = result.Inventory;
        //重新显示商店列表
        shopPanel.GetComponent<ShopPanelController>().ShowItems();
    }

    //道具购买失败后调用，在控制台输出错误原因，告知玩家购买失败
    void OnPlayFabPurchaseError(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
        purchaseLabel.text = "购买失败";
        BackButton.SetActive(true);
    }

    //PlayFab请求出错时调用，在控制台输出错误信息
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
    }

    //“取消”按钮，取消道具的购买，关闭道具详细信息面板
    public void ClickCancelButton()
    {
        gameObject.SetActive(false);
    }
}
