﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.DataModels;


public class InventoryPanelController : MonoBehaviour {

	public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject inventoryPanel; 
    public GameObject inventoryLoadingWindow;


    public GameObject[] inventoryItems;
    public GameObject previousButton;
	public GameObject nextButton;
	public Text pageMessage;

	public int selectedItem;
	public List<ItemInstance> items;

	private int itemsLength;
	private const int itemsPerPage = 6;
	private int currentPageNumber;
	private int maxPageNumber;

	void OnEnable () {
		inventoryLoadingWindow.SetActive (true);
		foreach (GameObject go in inventoryItems) {
			go.SetActive (false);
		}

		GetUserInventoryRequest request = new GetUserInventoryRequest ();
		PlayFabClientAPI.GetUserInventory (request, OnGetUserInventory, OnPlayFabError);
	}

	void OnGetUserInventory(GetUserInventoryResult result){
		items = result.Inventory;
		itemsLength = items.Count;
		currentPageNumber = 1;
		maxPageNumber = (itemsLength - 1) / itemsPerPage + 1;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();
		ShowItems ();
		inventoryLoadingWindow.SetActive (false);
	}

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

	void ShowItems(){
		int start, end, i, j;
		start = (currentPageNumber - 1) * itemsPerPage;
		if (currentPageNumber == maxPageNumber)
			end = itemsLength;
		else
			end = start + itemsPerPage;
		for (i = start,j = 0; i < end; i++,j++) {
			int itemNum = i;

			Text itemName = inventoryItems [j].transform.Find ("Name").GetComponent<Text>();
			Text equip = inventoryItems [j].transform.Find ("Equip").GetComponent<Text>();
			Button button = inventoryItems [j].transform.Find ("Button").GetComponent<Button>();

			itemName.text = items [i].DisplayName;
			if (PlayFabUserData.equipedWeapon == items [i].ItemClass) {
				equip.gameObject.SetActive(true);
				button.gameObject.SetActive (false);
			} else {
				equip.gameObject.SetActive(false);
				button.gameObject.SetActive (true);
				button.onClick.RemoveAllListeners ();
				button.onClick.AddListener (delegate {
                    var item = items[itemNum];
                    PlayFabUserData.equipedWeapon = item.ItemClass;
                    UpdateUserDataRequest request = new UpdateUserDataRequest() { };
                    request.Data = new Dictionary<string, string>();
                    request.Data.Add("EquipedWeapon", item.ItemClass);
                    PlayFabClientAPI.UpdateUserData(request, OnUpdateUserData, OnPlayFabError);

                    SetObjectsRequest requestObject = new SetObjectsRequest();
                    requestObject.Entity.Id = PlayFabAuthService.entityId;
                    requestObject.Entity.Type = PlayFabAuthService.entityType;
                    var dataObject = new Dictionary<string, object>()
                        {
                            {"EquipedWeapon", item.ItemClass}
                        };
                    var dataList = new List<SetObject>()
                        {
                            new SetObject()
                            {
                                ObjectName = "PlayerData",
                                DataObject = dataObject
                            },
                        };
                    requestObject.Objects = dataList;
                    PlayFabDataAPI.SetObjects(requestObject, OnUpdateUserData, OnPlayFabError);

                });
			}
			inventoryItems [j].SetActive (true);
		}
		for (; j < itemsPerPage; j++)
			inventoryItems [j].SetActive (false);
	}
    void OnUpdateUserData(UpdateUserDataResult result)
    {
        ShowItems();
        Debug.Log("User Data Saved");
    }
    void OnUpdateUserData(SetObjectsResponse result)
    {
        ShowItems();
        Debug.Log("User Data Saved");
    }
    void OnPlayFabError(PlayFabError error){
		Debug.LogError (error.ErrorDetails);
	}

    public void ClickPreviousButton(){
		currentPageNumber--;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();
		ShowItems ();
	}
    public void ClickNextButton(){
		currentPageNumber++;
		pageMessage.text = currentPageNumber.ToString () + "/" + maxPageNumber.ToString ();
		ButtonControl ();
		ShowItems ();
	}
}
