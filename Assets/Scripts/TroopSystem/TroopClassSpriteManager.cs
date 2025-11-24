using UnityEngine;

[CreateAssetMenu(fileName = "TroopClassSprites", menuName = "Troop Class Sprites")]
public class TroopClassSpriteManager : ScriptableObject
{
    private static TroopClassSpriteManager _instance;

    [Header("Class Sprites")]
    public Sprite redClassSprite;
    public Sprite greenClassSprite;
    public Sprite blueClassSprite;

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
}