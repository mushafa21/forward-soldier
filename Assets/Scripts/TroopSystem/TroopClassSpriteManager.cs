using UnityEngine;

[CreateAssetMenu(fileName = "TroopClassSprites", menuName = "Troop Class Sprites")]
public class TroopClassSpriteManager : ScriptableObject
{
    private static TroopClassSpriteManager _instance;

    [Header("Class Sprites")]
    public Sprite redClassSprite;
    public Sprite greenClassSprite;
    public Sprite blueClassSprite;
    
    
    [Header("Class Names")]
    public string redClassName;
    public string greenClassName;
    public string blueClassName;
    
    
    [Header("Class Colors")]
    public Color redClassColor;
    public Color greenClassColor;
    public Color blueClassColor;


    // Singleton instance property
    public static TroopClassSpriteManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find it in Resources
                _instance = Resources.Load<TroopClassSpriteManager>("TroopClassSprites");

                // If not found, try to find any instance in the project
                if (_instance == null)
                {
                    Debug.LogWarning("TroopClassSpriteManager not found! Please create a TroopClassSpriteManager asset in Resources folder.");
                }
            }
            return _instance;
        }
    }

    // Method to get the sprite based on troop class
    public Sprite GetSpriteForClass(TroopClass troopClass)
    {
        switch (troopClass)
        {
            case TroopClass.Red:
                return redClassSprite;
            case TroopClass.Green:
                return greenClassSprite;
            case TroopClass.Blue:
                return blueClassSprite;
            default:
                return null;
        }
    }
    
    public string GetNameForClass(TroopClass troopClass)
    {
        switch (troopClass)
        {
            case TroopClass.Red:
                return redClassName;
            case TroopClass.Green:
                return greenClassName;
            case TroopClass.Blue:
                return blueClassName;
            default:
                return null;
        }
    }
    
    public Color GetColorForClass(TroopClass troopClass)
    {
        switch (troopClass)
        {
            case TroopClass.Red:
                return redClassColor;
            case TroopClass.Green:
                return greenClassColor;
            case TroopClass.Blue:
                return blueClassColor;
            default:
                return redClassColor;
        }
    }
}