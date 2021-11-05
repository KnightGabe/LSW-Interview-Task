using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InventoryMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    public UnityEvent pauseEvent;
    public UnityEvent resumeEvent;

    //where to spawn equipment buttons
    public Transform buttonContentFather;

    public bool gamePaused = false;

    public TextMeshProUGUI goldCount;

    [Header("Equip Options")]
    public GameObject itemInfo;
    public TextMeshProUGUI itemValue;
    public TextMeshProUGUI itemName;

    public Button toggleEquipBttn;


    public void OpenMenu()
    {
        gameObject.SetActive(true);
        goldCount.text = "Your Gold : " + TopDownCharacterController.Instance.gold;
        gamePaused = true;
        pauseEvent.Invoke();
        SetupInventoryButtons();
        itemInfo.gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        gamePaused = false;
        resumeEvent.Invoke();
        itemInfo.gameObject.SetActive(false);
    }

    //setting up this way might be performance heavy. If I have enough time, I'll come back to create pooling logic
    public virtual void SetupInventoryButtons()
    {
        for (int i = buttonContentFather.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonContentFather.GetChild(i).gameObject);
        }
        //cycle through equipped clothes to instantiate their preview buttons
        foreach(var item in TopDownCharacterController.Instance.equipment.equippedClothing)
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

    //called when a equipment button is pressed
    public virtual void SelectEquipment(EquipmentButton button)
    {
        //set the correct text 
        if (TopDownCharacterController.Instance.equipment.equippedClothing.Contains(button.myItem))
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
        else
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";

        //pop up the menu
        itemInfo.gameObject.SetActive(true);
        itemName.text = button.myItem.name;
        itemValue.text = button.myItem.assetReference.value.ToString() + " Gold";

        //reset and update the listeners on the equip/unequip button
        toggleEquipBttn.onClick.RemoveAllListeners();
        toggleEquipBttn.onClick.AddListener(()=> button.ToggleEquip());
        toggleEquipBttn.onClick.AddListener(()=> itemInfo.gameObject.SetActive(false));
    }
}
