using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSetter : MonoBehaviour
{
    [Tooltip("0 Left_Hand, 1 Left_Lower_Arm, 2 Left_Upper_Arm, 3 Left_Foot, 4 Left_Lower_Leg, 5 Left_Upper_Leg, 6 Right_Hand, 7 Right_Lower_Arm, 8 Right_Upper_Arm, 9 Right_Foot, 10 Right_Lower_Leg, 11 Right_Upper_Leg, 12 Hip, 13 Belt, 14 Body, 15 Chest, 16 Headwear, 17 Left Shoulder, 18 Right Shoulder")]
    public SpriteRenderer[] slotRenderers;

    //need to specify the direction to distinguish between sprite types
    public FacingDirection direction;

    public void UpdateSlots(List<ClothingItem> currentClothes)
    {
        //first, visually deactivate all slots
        foreach (var item in slotRenderers)
        {
            item.enabled = false;
            item.color = Color.white;
        }

        //check the input array for every slot occupied and then update the sprite renderers
        for (int i = 0; i < currentClothes.Count; i++)
        {
            for (int j = 0; j < currentClothes[i].assetReference.slots.Length; j++)
            {
                switch (direction)
                {
                    case FacingDirection.Down:
                        slotRenderers[(int)currentClothes[i].assetReference.slots[j]].sprite = currentClothes[i].assetReference.downSprites[j];
                        if(currentClothes[i].assetReference.tintable)
                            slotRenderers[(int)currentClothes[i].assetReference.slots[j]].color = currentClothes[i].colorTint;
                        break;
                    case FacingDirection.Up:
                        slotRenderers[(int)currentClothes[i].assetReference.slots[j]].sprite = currentClothes[i].assetReference.upSprites[j];
                        if (currentClothes[i].assetReference.tintable) 
                            slotRenderers[(int)currentClothes[i].assetReference.slots[j]].color = currentClothes[i].colorTint;
                        break;                    
                    default:
                        //Left or Right
                        slotRenderers[(int)currentClothes[i].assetReference.slots[j]].sprite = currentClothes[i].assetReference.sideSprites[j];
                        if (currentClothes[i].assetReference.tintable) 
                            slotRenderers[(int)currentClothes[i].assetReference.slots[j]].color = currentClothes[i].colorTint;
                        break;
                }
                slotRenderers[(int)currentClothes[i].assetReference.slots[j]].enabled = true;
            }
        }
    }
}
