using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;



[System.Serializable]
public enum ShopItemType
{
    TowerUpgrade,
    SoulUpgrade,
    Troop
}


[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop Item Data")]
public class ShopItemSO : ScriptableObject
{
    [Header("Stats")]
    public ShopItemType type;

    public int price;
    public Sprite icon;
    [Header("References")] 
    public TroopSO troopSO; // Only used when type is Troop
    

}