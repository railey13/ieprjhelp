using System.Collections.Generic;
using UnityEngine;

public class UnitClass : MonoBehaviour
{
    [Header("UI")]
    public Sprite unitIcon;

    [Header("Stats")]
    public string UnitName;

    public int hp;
    public int atk;
    public int speed;
    public int movement;
    public int range;
    public List<Skill> skills = new();
    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log(
    UnitName +
    " took " + damage +
    " damage. Current HP: " + hp
);  
        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        hp += amount;
    }

    protected virtual void Die()
    {
        Debug.Log(UnitName + " has died.");
    }

    public bool IsAlive()
    {
        return hp > 0;
    }
}