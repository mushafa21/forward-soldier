using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;



[CreateAssetMenu(fileName = "Troop", menuName = "Troop Data")]
public class TroopSO : ScriptableObject
{
    [Header("Stats")]
    public float health;
    public float damage;
    public float attackRange;
    public float movementSpeed;
    public string speedCategory;
    public float attackCooldown;
    public float spawnCooldown;
    public float soulCost;
    public float soulGainedWhenDefeated;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;


    [Header("Presentation")] 
    public string name;
    public Sprite potrait;
    public AudioClip walkSound;
    public AudioClip hitSound;
    public AudioClip allyDeathSound;
    public AudioClip enemyDeathSound;
    public AnimatorOverrideController animatorController;

    // Method to calculate stats for a specific level
    public float GetHealthAtLevel(int level)
    {
        return health * Mathf.Pow(1.1f, level - 1);
    }

    public float GetDamageAtLevel(int level)
    {
        return damage * Mathf.Pow(1.1f, level - 1);
    }
}