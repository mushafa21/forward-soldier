using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Troop", menuName = "Troop Data")]
public class TroopSO : ScriptableObject
{
    [Header("Stats")]
    public float health;
    public float damage;
    public float attackRange;
    public float movementSpeed;
    public float attackCooldown;
    
    [Header("Presentation")]
    public AudioClip walkSound;
    public AudioClip hitSound;
    public AudioClip allyDeathSound;
    public AudioClip enemyDeathSound;


}