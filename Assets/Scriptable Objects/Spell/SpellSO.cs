using UnityEngine;


[System.Serializable]
public enum SpellType
{
    Fireball,
    Shield,
    AttackBoost,
}

[CreateAssetMenu(fileName = "Spell", menuName = "Spell Data")]
public class SpellSO : ScriptableObject
{
    [Header("Stats")]
    public float value;

    public float cooldown;
    public SpellType type;

    [Header("Presentation")] 
    public Sprite potrait;
  


}