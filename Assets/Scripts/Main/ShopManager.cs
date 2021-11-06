using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : InventoryMenu
{
    //for the clothes that can be of multiple colors
    public RectTransform colorPicker;

    public Color[] customizableColors;

    protected ClothingItem selectedPiece;

    //true while item is being previewed
    protected bool viewingItem;
    //true while waiting for confirmation
    protected bool awaitingPurchase;
    //used to switch between selling and buying
    protected bool sellMode = false;

    //object to visually represent the change in transaction mode
    public Transform sellTab;

    //if needed, manipulates price change when selling/buying
    public float sellMultiplier = 1;
    public float buyMultiplier = 1;

    //clothes available for purchase
    public List<Clothing> storeInventory = new List<Clothing>();

    //used for confirming transactions
    public DialogueCaller purchaseDialogue;



    public override void CloseMenu()
    {
        base.CloseMenu();
        colorPicker.gameObject.SetActive(false);
        TopDownCharacterController.Instance.equipment.onEquipmentChanged?.Invoke(TopDownCharacterController.Instance.equipment.equippedClothing);
    }

    public override void SetupInventoryButtons()
    {
        for (int i = buttonContentFather.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonContentFather.GetChild(i).gameObject);
        }
        colorPicker.gameObject.SetActive(false);

        //same as inherited. this is to show clothes that the player can sell
        if (sellMode)
        {
            //cycle through equipped clothes to instantiate their preview buttons
            foreach (var item in TopDownCharacterController.Instance.equipment.equippedClothing)
            {
                EquipmentButton newButton = Instantiate(item.assetReference.previewPrefab, buttonContentFather);

                newButton.UpdateItem(item);
                //when pressed, call this function
                newButton.onClick.AddListener(() => SelectEquipment(newButton));
            }
            //do the same for the unequipped clothes in the player's inventory
            foreach (var item in TopDownCharacterController.Instance.equipment.availableClothing)
            {
                EquipmentButton newButton = Instantiate(item.assetReference.previewPrefab, buttonContentFather);

                newButton.UpdateItem(item);
                newButton.onClick.AddListener(() => SelectEquipment(newButton));
            }
        }
        //this is in buying mode. it shows clothes available for purchase
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

    /// <summary>
    /// Alternate between buy/sell mode
    /// </summary>
    public void SwitchMode(bool turnSellModeOn)
    {
        colorPicker.gameObject.SetActive(false);
        if (sellMode != turnSellModeOn)
        {
            sellMode = turnSellModeOn;
            if (!sellMode)
                sellTab.SetAsFirstSibling();
            else
                sellTab.SetAsLastSibling();
            SetupInventoryButtons();
        }
    }

    public override void SelectEquipment(EquipmentButton button)
    {

        //if clothing has multiple color options, allow them to be chosen
        if (button.myItem.assetReference.tintable && !sellMode)
        {
            SelectEquipmentColor(button);
        }
        else
        {
            PreviewClothing(button.myItem);
        }
    }

    //Displays name and price, along with option to buy
    public virtual void ShowInfo(ClothingItem item)
    {
        itemInfo.gameObject.SetActive(true);
        itemName.text = item.name;
        //if is in buy mode
        if (!sellMode)
        {
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            itemValue.text = (item.assetReference.value * buyMultiplier).ToString() + " Gold";
            //if player doensnt have enough money, it greys out the purchase button
            if (TopDownCharacterController.Instance.gold < item.assetReference.value * buyMultiplier)
            {
                itemValue.color = Color.red;
                toggleEquipBttn.interactable = false;
            }
            else
            {
                itemValue.color = Color.white;
                toggleEquipBttn.interactable = true;
            }
        }
        else
        {
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Sell";
            itemValue.color = Color.white;
            itemValue.text = (item.assetReference.value * sellMultiplier).ToString() + " Gold";
            toggleEquipBttn.interactable = true;
        }
        toggleEquipBttn.onClick.RemoveAllListeners();
        toggleEquipBttn.onClick.AddListener(() => StartTransaction());
        toggleEquipBttn.onClick.AddListener(() => itemInfo.gameObject.SetActive(false));
    }

    //Spawns multiple choices of color for tintable clothes
    public void SelectEquipmentColor(EquipmentButton button)
    {
        for (int i = colorPicker.childCount - 1; i >= 0; i--)
        {
            Destroy(colorPicker.GetChild(i).gameObject);
        }
        colorPicker.position = button.transform.position;
        colorPicker.gameObject.SetActive(true);
        for (int i = 0; i < customizableColors.Length; i++)
        {
            EquipmentButton newBttn = Instantiate(button, colorPicker);
            newBttn.UpdateItem(button.myItem.assetReference.CreateInstance(customizableColors[i]));
            newBttn.onClick.AddListener(() => PreviewClothing(newBttn.myItem));
        }
    }

    //puts clothes temporarily on player
    public virtual void PreviewClothing(ClothingItem itemToPreview)
    {
        colorPicker.gameObject.SetActive(false);
        ShowInfo(itemToPreview);
        viewingItem = true;
        List<ClothingItem> overlaps = TopDownCharacterController.Instance.equipment.CheckForOverlap(itemToPreview);
        List<ClothingItem> previewList = new List<ClothingItem>(TopDownCharacterController.Instance.equipment.equippedClothing);
        foreach (var item in overlaps)
        {
            if (previewList.Contains(item))
                previewList.Remove(item);
        }
        previewList.Add(itemToPreview);
        TopDownCharacterController.Instance.equipment.PreviewClothing(previewList);
        selectedPiece = itemToPreview;
    }

    //this async function and the other ones it calls track the whole process of transaction
    public virtual async void StartTransaction()
    {
        awaitingPurchase = true;

        //ask player if he really wants to buy
        await AskForConfirmation();
        if (viewingItem && !sellMode)
        {
            await AskForEquip();
        }
        SetupInventoryButtons();
        TopDownCharacterController.Instance.equipment.onEquipmentChanged?.Invoke(TopDownCharacterController.Instance.equipment.equippedClothing);
    }

    //call the dialogue asking for confirmation and waits for it. this is done using the dialoguecaller class' events in the scene
    public async Task AskForConfirmation()
    {
        purchaseDialogue.StartDialogue(0);
        while (awaitingPurchase)
        {
            await Task.Yield();
        }
    }

    //class to be called by the dialogue events fired from the dialoguecaller. it resumes the transaction
    public virtual void ConfirmTransaction(bool value)
    {
        //if is buying
        if (!sellMode)
        {
            if (value)
            {
                //calculate new player wealth. players should only get here if they have enough money, so checking here isn't necessary
                TopDownCharacterController.Instance.gold -= Mathf.FloorToInt(selectedPiece.assetReference.value * buyMultiplier);
                TopDownCharacterController.Instance.equipment.availableClothing.Add(selectedPiece);
            }
            else
            {
                viewingItem = false;
            }
        }
        else
        {
            //if player is selling and confirms it, add the gold value and add to store inventory if it doenst already have it
            if (value)
            {
                TopDownCharacterController.Instance.gold += Mathf.FloorToInt(selectedPiece.assetReference.value * sellMultiplier);
                TopDownCharacterController.Instance.equipment.RemovePiece(selectedPiece);
                if (!storeInventory.Contains(selectedPiece.assetReference))
                    storeInventory.Add(selectedPiece.assetReference);
            }
            viewingItem = false;
        }
        goldCount.text = TopDownCharacterController.Instance.gold.ToString() + " Gold";
        awaitingPurchase = false;
    }

    //ask the player if they want to equip recently bought products
    public async Task AskForEquip()
    {
        purchaseDialogue.StartDialogue(1);
        while (viewingItem)
        {
            await Task.Yield();
        }
    }

    public void ConfirmEquip(bool value)
    {
        if (value)
        {
            TopDownCharacterController.Instance.equipment.EquipPiece(selectedPiece);
        }
        viewingItem = false;
    }
}
