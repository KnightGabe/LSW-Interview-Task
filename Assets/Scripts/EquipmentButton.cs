using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentButton : Button
{
    public ClothingItem myItem;

    public Image[] itemPreview;

    protected override void Start()
    {
        itemPreview = gameObject.transform.Find("Sprites").GetComponentsInChildren<Image>();
    }

    public void UpdateItem(ClothingItem newItem)
    {
        myItem = newItem;
        for (int i = 0; i < itemPreview.Length; i++)
        {
            if (myItem.assetReference.tintable)
                itemPreview[i].color = myItem.colorTint;
            else
                itemPreview[i].color = Color.white;
        }
        if (TopDownCharacterController.Instance.equipment.equippedClothing.Contains(myItem))
            targetGraphic.color = Color.grey;
        else
            targetGraphic.color = Color.black;
        TopDownCharacterController.Instance.equipment.onEquipmentChanged += CheckForUpdates;
    }

    public void CheckForUpdates(List<ClothingItem> equips)
    {
        if(myItem != null)
        {
            if (equips.Contains(myItem))
                targetGraphic.color = Color.grey;
            else
                targetGraphic.color = Color.black;
        }
    }

    //actualize the equipment change
    public void ToggleEquip()
    {
        if (myItem != null)
        {
            //remove the myItem if it's equipped and vice-versa
            if (TopDownCharacterController.Instance.equipment.equippedClothing.Contains(myItem))
            {
                TopDownCharacterController.Instance.equipment.UnequipPiece(myItem);
            }
            else
            {
                TopDownCharacterController.Instance.equipment.EquipPiece(myItem);
            }
        }
    }

    protected override void OnDestroy()
    {
        TopDownCharacterController.Instance.equipment.onEquipmentChanged -= CheckForUpdates;
    }
}
