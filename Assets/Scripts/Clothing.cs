using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClothingSlot { Left_Hand, Left_Lower_Arm, Left_Upper_Arm, Left_Foot, Left_Lower_Leg, Left_Upper_Leg,
 Right_Hand, Right_Lower_Arm, Right_Upper_Arm, Right_Foot, Right_Lower_Leg, Right_Upper_Leg, Hip, Belt, Body, Chest, Headwear, Left_Shoulder, Right_Shoulder}

//This class is used only for reference to Slot occupation and Sprites
[CreateAssetMenu(menuName ="Clothing")]
public class Clothing : ScriptableObject
{
    //name to be shown in-game
    public string displayName;

    //array of slots that the piece of clothing covers
    public ClothingSlot[] slots;

    //sprites equivalent to each slot that the piece of clothing uses up
    public Sprite[] downSprites;
    public Sprite[] upSprites;
    public Sprite[] sideSprites;

    //value to be used to calculate buying and selling prices
    public int value;

    //if piece can have multiple colors
    public bool tintable = false;

    //object to be instantiated to preview the item in menus
    public EquipmentButton previewPrefab;

    public ClothingItem CreateInstance()
    {
        ClothingItem newPiece = new GameObject(displayName).AddComponent<ClothingItem>();
        newPiece.assetReference = this;
        if (tintable)
            newPiece.colorTint = Color.white;
        return newPiece;
    }

    public ClothingItem CreateInstance(Color color)
    {
        ClothingItem newPiece = new GameObject(displayName).AddComponent<ClothingItem>();
        newPiece.assetReference = this;
        if (tintable)
            newPiece.colorTint = color;
        return newPiece;
    }
}
