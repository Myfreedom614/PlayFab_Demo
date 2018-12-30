using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
//商城面板控制器
public class ShopPanelController : MonoBehaviour
{

    public GameObject lobbyPanel;           //大厅面板
    public GameObject roomPanel;            //房间面板
    public GameObject shopPanel;            //商城面板
    public GameObject shopLoadingWindow;    //“商城加载中”提示窗口
    public Button backButton;               //返回按钮
    public Text currentPanel;               //当前面板文本信息

    public GameObject[] shopItemsPanel;     //商城道具面板
    public Sprite diamondCurrencySprite;    //钻石图片
    public Sprite goldCurrencySprite;       //金币图片
    public GameObject previousButton;       //上一页按钮
    public GameObject nextButton;           //下一页按钮
    public Text pageMessage;                //页面信息
    public GameObject shopItemDetails;      //商城道具详细信息面板

    public static int selectedItem;                 //选中的商城道具
    public static List<CatalogItem> shopItems;      //商城道具列表
    public static List<ItemInstance> userItems;     //玩家道具列表

    private int itemsLength;
    private const int itemsPerPage = 6;
    private int currentPageNumber;
    private int maxPageNumber;

    //商城面板激活时调用，获取商城道具信息，显示在商城面板中
    void OnEnable()
    {
        currentPanel.text = "商 城";
		shopLoadingWindow.SetActive(true);
		shopItemDetails.SetActive(false);
        foreach (GameObject go in shopItemsPanel)
        {
            go.SetActive(false);
        }

        //获取玩家仓库信息
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetUserInventory, OnPlayFabError);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate {
            if (PhotonNetwork.inRoom)
                roomPanel.SetActive(true);
            else lobbyPanel.SetActive(true);
            shopPanel.SetActive(false);
        });
    }

    //玩家仓库信息获取成功时调用
    void OnGetUserInventory(GetUserInventoryResult result)
    {
        userItems = result.Inventory;

        //获取商城道具列表
        GetCatalogItemsRequest request = new GetCatalogItemsRequest()
        {
            CatalogVersion = PlayFabUserData.catalogVersion
        };
        PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogItems, OnPlayFabError);
    }

    //商城道具列表获取成功后调用
    void OnGetCatalogItems(GetCatalogItemsResult result)
    {
        List<CatalogItem> temp = result.Catalog;
        for (int i = temp.Count - 1; i >= 0; i--)
        {
            if (temp[i].VirtualCurrencyPrices.ContainsKey("FR"))    //剔除AK47在商店中的显示（AK47是免费枪支）
                temp.RemoveAt(i);
        }
        //计算商城道具个数，计算商城面板页数
        shopItems = temp;
        itemsLength = temp.Count;
        currentPageNumber = 1;
        maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();        //翻页按钮控制
        ShowItems();            //显示商城道具
        shopLoadingWindow.SetActive(false);
    }

    //翻页按钮控制
    void ButtonControl()
    {
        if (currentPageNumber == 1)
            previousButton.SetActive(false);
        else
            previousButton.SetActive(true);
        if (currentPageNumber == maxPageNumber)
            nextButton.SetActive(false);
        else
            nextButton.SetActive(true);
    }
    //显示商城道具
    public void ShowItems()
    {
        int start, end, i, j;
        start = (currentPageNumber - 1) * itemsPerPage;
        if (currentPageNumber == maxPageNumber)
            end = itemsLength;
        else
            end = start + itemsPerPage;
        Text[] texts;
        Image[] images;
        Button button;
        for (i = start, j = 0; i < end; i++, j++)
        {
            int itemNum = i;
            texts = shopItemsPanel[j].GetComponentsInChildren<Text>();
            images = shopItemsPanel[j].GetComponentsInChildren<Image>();
            button = shopItemsPanel[j].GetComponentInChildren<Button>();
            texts[0].text = shopItems[i].DisplayName;
            images[1].sprite = GameInfo.guns[shopItems[i].ItemClass];
            //道具是金币购买还是钻石购买
            if (shopItems[i].VirtualCurrencyPrices.ContainsKey("GC"))
            {
                texts[1].text = shopItems[i].VirtualCurrencyPrices["GC"].ToString();
                images[2].sprite = goldCurrencySprite;
            }
            else if (shopItems[i].VirtualCurrencyPrices.ContainsKey("DC"))
            {
                texts[1].text = shopItems[i].VirtualCurrencyPrices["DC"].ToString();
                images[2].sprite = diamondCurrencySprite;
            }
            button.onClick.RemoveAllListeners();

            //根据道具的类型（ItemClass），判断玩家是否已经拥有该物品
            bool hasItems = false;
            foreach (ItemInstance ii in userItems)
            {
                if (ii.ItemClass == shopItems[i].ItemClass)
                {
                    hasItems = true;
                    break;
                }
            }
            if (hasItems)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "已拥有";
            }
            else
            {
                button.interactable = true;
                button.GetComponentInChildren<Text>().text = "购买";
                button.onClick.AddListener(delegate
                {
                    selectedItem = itemNum;
                    shopItemDetails.SetActive(true);
                });
            }
            shopItemsPanel[j].SetActive(true);
        }
        for (; j < itemsPerPage; j++)
            shopItemsPanel[j].SetActive(false);
    }

    //PlayFab请求出错时调用，在控制台输出错误信息
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
    }

    //上一页按钮
    public void ClickPreviousButton()
    {
        currentPageNumber--;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowItems();
    }

    //下一页按钮
    public void ClickNextButton()
    {
        currentPageNumber++;
        pageMessage.text = currentPageNumber.ToString() + "/" + maxPageNumber.ToString();
        ButtonControl();
        ShowItems();
    }
}
