using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarberMenu : ShopManager
{

    public override void CloseMenu()
    {
        base.CloseMenu();
        colorPicker.gameObject.SetActive(false);
        TopDownCharacterController.Instance.equipment.onHairstyleChanged?.Invoke(TopDownCharacterController.Instance.equipment.currentHair);
    }

    public override void PreviewClothing(ClothingItem itemToPreview)
    {
        colorPicker.gameObject.SetActive(false);
        ShowInfo(itemToPreview);
        viewingItem = true;
        List<ClothingItem> overlaps = TopDownCharacterController.Instance.equipment.CheckForHairOverlap(itemToPreview);
        List<ClothingItem> previewList = new List<ClothingItem>(TopDownCharacterController.Instance.equipment.currentHair);
        foreach (var item in overlaps)
        {
            if (previewList.Contains(item))
                previewList.Remove(item);
        }
        previewList.Add(itemToPreview);
        TopDownCharacterController.Instance.equipment.PreviewHairstyles(previewList);
        selectedPiece = itemToPreview;
    }

    public override void SetupInventoryButtons()
    {
        for (int i = buttonContentFather.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonContentFather.GetChild(i).gameObject);
        }
        colorPicker.gameObject.SetActive(false);

        //sell mode means shaving in this class
        if (sellMode)
        {
            //shows if character has hair he can shave off
            foreach (var item in TopDownCharacterController.Instance.equipment.currentHair)
            {
                EquipmentButton newButton = Instantiate(item.assetReference.previewPrefab, buttonContentFather);

                newButton.UpdateItem(item);
                //when pressed, call this function
                newButton.onClick.AddListener(() => SelectEquipment(newButton));
            }
        }
        //show hairstyles available
        else
        {
            foreach (var item in storeInventory)
            {
                EquipmentButton newButton = Instantiate(item.previewPrefab, buttonContentFather);

                newButton.UpdateItem(item.CreateInstance());
                //when pressed, call this function
                newButton.onClick.AddListener(() => SelectEquipment(newButton));
            }
        }
    }

    //different from the normal shop, we don't sell, we shave, so we still need a money check in "sellmode"
    public override void ShowInfo(ClothingItem item)
    {

        float currentMultiplier = 1;

        if (!sellMode)
        {
            currentMultiplier = buyMultiplier;
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Style";
        }
        else
        {
            currentMultiplier = sellMultiplier;
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Cut";
        }
        itemInfo.gameObject.SetActive(true);
        itemName.text = item.name; itemValue.text = (item.assetReference.value * currentMultiplier).ToString() + " Gold";

        //if player doensnt have enough money, it greys out the purchase button
        if (TopDownCharacterController.Instance.gold < item.assetReference.value * currentMultiplier)
        {
            itemValue.color = Color.red;
            toggleEquipBttn.interactable = false;
        }
        else
        {
            itemValue.color = Color.white;
            toggleEquipBttn.interactable = true;
        }


        toggleEquipBttn.onClick.RemoveAllListeners();
        toggleEquipBttn.onClick.AddListener(() => StartTransaction());
        toggleEquipBttn.onClick.AddListener(() => itemInfo.gameObject.SetActive(false));
    }

    //we won't do the equip prompt here. buying means you already style the hair
    public override async void StartTransaction()
    {
        awaitingPurchase = true;
        await AskForConfirmation();
    }

    public override void ConfirmTransaction(bool value)
    {
        //selling in this case means shaving
        if (!sellMode)
        {
            if (value)
            {
                //calculate new player wealth. players should only get here if they have enough money, so checking here isn't necessary
                TopDownCharacterController.Instance.gold -= Mathf.FloorToInt(selectedPiece.assetReference.value * buyMultiplier);
                TopDownCharacterController.Instance.equipment.ChangeHair(selectedPiece);
            }
            else
            {
                viewingItem = false;
            }
        }
        else
        {
            if (value)
            {
                TopDownCharacterController.Instance.gold -= Mathf.FloorToInt(selectedPiece.assetReference.value * sellMultiplier);
                TopDownCharacterController.Instance.equipment.RemoveHair(selectedPiece);
            }
            viewingItem = false;
        }
        goldCount.text = TopDownCharacterController.Instance.gold.ToString() + " Gold";
        awaitingPurchase = false;
        TopDownCharacterController.Instance.equipment.onHairstyleChanged?.Invoke(TopDownCharacterController.Instance.equipment.currentHair);
    }
}
