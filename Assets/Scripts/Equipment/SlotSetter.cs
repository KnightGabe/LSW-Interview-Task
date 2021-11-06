using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotSetter : MonoBehaviour
{

    public SlotPairs[] slotPairs;

    //need to specify the direction to distinguish between sprite types
    public FacingDirection direction;

    public void UpdateSlots(List<ClothingItem> currentClothes)
    {
        //first, visually deactivate all slots
        for (int i = 0; i < slotPairs.Length; i++)
        {
            slotPairs[i].renderer.enabled = false;
            slotPairs[i].renderer.color = Color.white;
        }

        //check the input array for every slot occupied and then update the sprite renderers
        for (int i = 0; i < currentClothes.Count; i++)
        {
            for (int j = 0; j < currentClothes[i].assetReference.slots.Length; j++)
            {
                switch (direction)
                {
                    case FacingDirection.Down:
                        for (int k = 0; k < slotPairs.Length; k++)
                        {
                            if (slotPairs[k].slot == currentClothes[i].assetReference.slots[j])
                            {
                                slotPairs[k].renderer.sprite = currentClothes[i].assetReference.downSprites[j];
                                if (currentClothes[i].assetReference.tintable)
                                    slotPairs[k].renderer.color = currentClothes[i].colorTint;
                                slotPairs[k].renderer.enabled = true;
                                break;
                            }
                        }
                        break;
                    case FacingDirection.Up:
                        for (int k = 0; k < slotPairs.Length; k++)
                        {
                            if (slotPairs[k].slot == currentClothes[i].assetReference.slots[j])
                            {
                                slotPairs[k].renderer.sprite = currentClothes[i].assetReference.upSprites[j];
                                if (currentClothes[i].assetReference.tintable)
                                    slotPairs[k].renderer.color = currentClothes[i].colorTint;
                                slotPairs[k].renderer.enabled = true;
                                break;
                            }
                        }
                        break;
                    default:
                        //Left or Right
                        for (int k = 0; k < slotPairs.Length; k++)
                        {
                            if (slotPairs[k].slot == currentClothes[i].assetReference.slots[j])
                            {
                                slotPairs[k].renderer.sprite = currentClothes[i].assetReference.sideSprites[j];
                                if (currentClothes[i].assetReference.tintable)
                                    slotPairs[k].renderer.color = currentClothes[i].colorTint;
                                slotPairs[k].renderer.enabled = true;
                                break;
                            }
                        }
                        break;
                }
            }

        }
    }
}

[System.Serializable]
public class SlotPairs
{
    public ClothingSlot slot;
    public SpriteRenderer renderer;
}
