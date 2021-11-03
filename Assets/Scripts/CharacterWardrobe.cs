using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWardrobe : MonoBehaviour
{
    //use this to initialize standard clothes
    public Clothing[] startingClothes;

    //use this to choose the colors of the starting equipment
    public Color[] startingTint;

    //the list of clothes a character has equipped at any given time
    public List<ClothingItem> equippedClothing = new List<ClothingItem>();

    //clothes in inventory
    public List<ClothingItem> availableClothing = new List<ClothingItem>();

    //this stores the setters for each facing direction
    public SlotSetter[] slots;

    public void Start()
    {
        //initialize the player's clothes based on a pre-supplied array
        for (int i = 0; i < startingClothes.Length; i++)
        {
            ClothingItem newPiece = new GameObject(startingClothes[i].name).AddComponent<ClothingItem>();
            newPiece.assetReference = startingClothes[i];
            if (startingClothes[i].tintable)
                newPiece.colorTint = startingTint[i];
            else
                newPiece.colorTint = Color.white;
            equippedClothing.Add(newPiece);
        }
        foreach (var item in CheckForOverlap())
        {
            availableClothing.Add(item);
        }
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateSlots(equippedClothing.ToArray());
        }
    }

    /// <summary>
    /// Check if equipped clothes overlap. Use only for initiation
    /// </summary>
    /// <returns>returns a list o clothes that overlap and will be unequipped.</returns>
    public List<ClothingItem> CheckForOverlap()
    {
        List<ClothingSlot> usedSlots = new List<ClothingSlot>();
        List<ClothingItem> overlaps = new List<ClothingItem>();

        for (int i = equippedClothing.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < equippedClothing[i].assetReference.slots.Length; j++)
            {
                if (!usedSlots.Contains(equippedClothing[i].assetReference.slots[j]))
                {
                    usedSlots.Add(equippedClothing[i].assetReference.slots[j]);
                }
                else
                {
                    overlaps.Add(equippedClothing[i]);
                    equippedClothing.Remove(equippedClothing[i]);
                    break;
                }
            }
        }
        return overlaps;
    }
    
    /// <summary>
    /// Check if equipped clothes overlap with a especific item
    /// </summary>
    /// <returns>returns a list o clothes that overlap.</returns>
    public List<ClothingItem> CheckForOverlap(ClothingItem item)
    {
        List<ClothingSlot> usedSlots = new List<ClothingSlot>();
        List<ClothingItem> overlaps = new List<ClothingItem>();

        for (int i = 0; i < item.assetReference.slots.Length; i++)
        {
            usedSlots.Add(item.assetReference.slots[i]);
        }

        for (int i = equippedClothing.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < equippedClothing[i].assetReference.slots.Length; j++)
            {
                if (usedSlots.Contains(equippedClothing[i].assetReference.slots[j]))
                {
                    overlaps.Add(equippedClothing[i]);
                    break;
                }
            }
        }
        
        return overlaps;
    }

    public void EquipPiece(ClothingItem piece)
    {
        //Check for and unequip overlapping clothes
        foreach (var item in CheckForOverlap(piece))
        {
            availableClothing.Add(item);
            equippedClothing.Remove(item);
        }
        //add the desired item
        equippedClothing.Add(piece);
        if (availableClothing.Contains(piece))
            availableClothing.Remove(piece);
        //update the visual representation
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateSlots(equippedClothing.ToArray());
        }
    }

    public void RemovePiece(ClothingItem piece)
    {
        equippedClothing.Remove(piece);
        availableClothing.Add(piece);
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateSlots(equippedClothing.ToArray());
        }
    }
}
