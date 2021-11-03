using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public GameObject pauseMenu;

    public UnityEvent pauseEvent;
    public UnityEvent resumeEvent;

    //where to spawn equipment buttons
    public Transform buttonContentFather;

    public KeyCode menuKey;

    public bool gamePaused = false;

    [Header("Equip Options")]
    public RectTransform equipOptionsMenu;
    public Button toggleEquipBttn;

    private void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            PauseGame(!gamePaused);
        }
    }

    public void PauseGame(bool value)
    {
        gamePaused = value;
        TopDownCharacterController.Instance.ToggleMovement(!value);
        if (value)
        {
            pauseEvent.Invoke();
            SetupInventoryButtons();
        }
        else
        {
            resumeEvent.Invoke();
        }
    }

    //setting up this way might be performance heavy. If I have enough time, I'll come back to create pooling logic
    public void SetupInventoryButtons()
    {
        for (int i = buttonContentFather.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(buttonContentFather.GetChild(i).gameObject);
        }
        //cycle through equipped clothes to instantiate their preview buttons
        foreach(var item in TopDownCharacterController.Instance.equipment.equippedClothing)
        {
            Button newButton = Instantiate(item.assetReference.previewPrefab, buttonContentFather).GetComponent<Button>();
            
            //change the color of the button so we know it's currently equipped
            newButton.targetGraphic.color = Color.grey;
            //when pressed, call this function
            newButton.onClick.AddListener(() => SelectEquipment(item, newButton));

            //if it can be of different colors, represent it with the correct color
            if (item.assetReference.tintable)
            {
                foreach (var childImgs in newButton.transform.GetChild(0).GetComponentsInChildren<Image>())
                {
                    childImgs.color = item.colorTint;
                }                
            }
        }
        //do the same for the unequipped clothes in the player's inventory
        foreach (var item in TopDownCharacterController.Instance.equipment.availableClothing)
        {
            Button newButton = Instantiate(item.assetReference.previewPrefab, buttonContentFather).GetComponent<Button>();

            //change the color of the button so we know it's currently unequipped
            newButton.targetGraphic.color = Color.black;
            newButton.onClick.AddListener(() => SelectEquipment(item, newButton));

            if (item.assetReference.tintable)
            {
                foreach (var childImgs in newButton.transform.GetChild(0).GetComponentsInChildren<Image>())
                {
                    childImgs.color = item.colorTint;
                }
            }
        }
    }

    //called when a equipment button is pressed
    public void SelectEquipment(ClothingItem item, Button button)
    {
        //set the correct text 
        if (TopDownCharacterController.Instance.equipment.equippedClothing.Contains(item))
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
        else
            toggleEquipBttn.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";

        //pop up the menu
        equipOptionsMenu.position = button.GetComponent<RectTransform>().position;
        equipOptionsMenu.gameObject.SetActive(true);

        //reset and update the listeners on the equip/unequip button
        toggleEquipBttn.onClick.RemoveAllListeners();
        toggleEquipBttn.onClick.AddListener(()=> ToggleEquip(item, button));
    }

    //actualize the equipment change
    public void ToggleEquip(ClothingItem piece, Button button)
    {
        if (piece != null)
        {
            //remove the piece if it's equipped and vice-versa
            if (TopDownCharacterController.Instance.equipment.equippedClothing.Contains(piece))
            {
                TopDownCharacterController.Instance.equipment.RemovePiece(piece);
                button.targetGraphic.color = Color.black;
            }
            else
            {
                TopDownCharacterController.Instance.equipment.EquipPiece(piece);
                button.targetGraphic.color = Color.grey;
            }
            equipOptionsMenu.gameObject.SetActive(false);
        }
    }
}
