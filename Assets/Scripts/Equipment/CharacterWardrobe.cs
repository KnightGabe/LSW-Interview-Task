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
    //for the hair
    public SlotSetter[] hairSlots;

    //we need to track hair as a different property because it's handled differently from equipment
    public List<ClothingItem> currentHair = new List<ClothingItem>();

    //strating hairsyle on character
    public Clothing startingHair;

    //events to call when visuals need to be changed
    public System.Action<List<ClothingItem>> onEquipmentChanged;
    public System.Action<List<ClothingItem>> onHairstyleChanged;

    public void Start()
    {
        //initialize the player's clothes an hair based on a pre-supplied array
        if (startingHair)
            currentHair.Add(startingHair.CreateInstance());
        for (int i = 0; i < startingClothes.Length; i++)
        {
            ClothingItem newPiece = null;
            if (startingClothes[i].tintable)
                newPiece = startingClothes[i].CreateInstance(startingTint[i]);
            else
                newPiece = startingClothes[i].CreateInstance();
            equippedClothing.Add(newPiece);
        }
        foreach (var item in CheckForOverlap())
        {
            availableClothing.Add(item);
        }
        for (int i = 0; i < slots.Length; i++)
        {
            onEquipmentChanged += slots[i].UpdateSlots;
        }
        for (int i = 0; i < hairSlots.Length; i++)
        {
            onHairstyleChanged += hairSlots[i].UpdateSlots;
        }
        onHairstyleChanged?.Invoke(currentHair);
        onEquipmentChanged?.Invoke(equippedClothing);
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
    
    //Same as CheckForOverlap, but for hair
    public List<ClothingItem> CheckForHairOverlap(ClothingItem item)
    {
        List<ClothingSlot> usedSlots = new List<ClothingSlot>();
        List<ClothingItem> overlaps = new List<ClothingItem>();

        for (int i = 0; i < item.assetReference.slots.Length; i++)
        {
            usedSlots.Add(item.assetReference.slots[i]);
        }

        for (int i = currentHair.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < currentHair[i].assetReference.slots.Length; j++)
            {
                if (usedSlots.Contains(currentHair[i].assetReference.slots[j]))
                {
                    overlaps.Add(currentHair[i]);
                    break;
                }
            }
        }
        
        return overlaps;
    }

    /// <summary>
    /// This function only updates the visuals without changing the actual equipment list
    /// </summary>
    public void PreviewClothing(List<ClothingItem> tempClothes)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateSlots(tempClothes);
        }
    }
    /// <summary>
    /// Same as PreviewClothing, but for the hair slots
    /// </summary>
    public void PreviewHairstyles(List<ClothingItem> tempStyles)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            hairSlots[i].UpdateSlots(tempStyles);
        }
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
        onEquipmentChanged?.Invoke(equippedClothing);
    }

    public void UnequipPiece(ClothingItem piece)
    {
        equippedClothing.Remove(piece);
        availableClothing.Add(piece);
        onEquipmentChanged?.Invoke(equippedClothing);
    }
    
    public void RemovePiece(ClothingItem piece)
    {
        if (equippedClothing.Contains(piece))
            equippedClothing.Remove(piece);
        if (availableClothing.Contains(piece))
            availableClothing.Remove(piece);
    }
    
    public void RemoveHair(ClothingItem piece)
    {
        if (currentHair.Contains(piece))
            currentHair.Remove(piece);
    }

    public void ChangeHair(ClothingItem newHair)
    {
        //returns overlapping hair
        foreach (var item in CheckForHairOverlap(newHair))
        {
            currentHair.Remove(item);
        }
        //set the desired style
        currentHair.Add(newHair);
    }
}
