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


}