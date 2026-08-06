using System.Collections.Generic;
using UnityEngine;

public class UnitClass : MonoBehaviour
{
    [Header("UI")]
    public Sprite unitIcon;

    [Header("Stats")]
    public string UnitName;

    public int hp;
    public int maxHp;
    public int atk;
    public int speed;
    public int movement;
    public int range;

    public List<Skill> skills = new List<Skill>();

    protected virtual void Awake()
    {
        // Enzo changes: if I forget to set max hp it just uses the starting hp
        if (maxHp <= 0)
            maxHp = hp;
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;

        Debug.Log(UnitName + " took " + damage + " damage. Current HP: " + hp);

        HitEffect hitEffect = GetComponent<HitEffect>();
        if (hitEffect != null)
            hitEffect.PlayHitEffect();

        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        hp += amount;

        // Enzo changes: healing shouldnt go over max hp
        if (maxHp > 0)
            hp = Mathf.Min(hp, maxHp);

        Debug.Log(UnitName + " healed for " + amount + ". Current HP: " + hp);
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
