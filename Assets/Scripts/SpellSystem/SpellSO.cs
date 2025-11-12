using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace SpellSystem
{
    public enum SpellType
    {
        Fireball,
        Shield,
        AttackBoost
    }

    [CreateAssetMenu(fileName = "Spell", menuName = "Spell Data")]
    public class SpellSO : ScriptableObject
    {
        [Header("Spell Info")]
        public string spellName;
        public Sprite icon;
        public SpellType spellType;
        public float spellCost;
        public float cooldown;
        
        [Header("Spell Effects")]
        public float damage;          // For fireball
        public float radius;          // For fireball AOE
        public float duration;        // For shield and attack boost
        public float bonusPercent;    // For attack boost percentage
    }
}