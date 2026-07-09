using System.Collections.Generic;
using UnityEngine;

public class UnitClass : MonoBehaviour
{
    [Header("UI")]
    public Sprite unitIcon;
    [Header("Basic Attack Type")]
    public DamageCategory basicAttackCategory = DamageCategory.Physical;
    public DamageSubtype basicAttackSubtype = DamageSubtype.Sharp;
    public int hitCount;
    [Header("Stats")]
    public string UnitName;
    public int hp;
    public int maxHp;
    public float atk;
    public float mag;
    public int speed;
    public int movement;
    public int range;
    [Header("Main Defence")]
    public float physDef;
    public float magDef;
    [Header ("Type Resistances")]
    public float sharpRes = 1f;
    public float pierceRes = 1f;
    public float bluntRes = 1f;
    public float fireRes = 1f;
    public float natureRes = 1f;
    public float darkRes = 1f;

    [Header("Skills")]
    public List<Skill> skills = new();
    public List<SkillState> skillStates = new();

    protected virtual void Awake()
    {
        skillStates.Clear();
        maxHp = hp;
        foreach (Skill skill in skills)
        {
            skillStates.Add(new SkillState
            {
                skill = skill
            });
        }
    }
    public virtual void TakeDamage(float damage)
    {
        hp -= Mathf.FloorToInt(damage); // always rounds down decimal damage
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
    }

    protected virtual void Die()
    {
        Debug.Log(UnitName + " has died.");
    }

    public bool IsAlive()
    {
        return hp > 0;
    }

    public float GetResistance(DamageSubtype subtype)
    {
        // PHYSICAL SUBTYPES
        if (subtype == DamageSubtype.Sharp)
            return sharpRes;
        else if (subtype == DamageSubtype.Pierce)
            return pierceRes;
        else if (subtype == DamageSubtype.Blunt)
            return bluntRes;
        // MAGICAL SUBTYPES
        else if (subtype == DamageSubtype.Fire)
            return fireRes;
        else if (subtype == DamageSubtype.Nature)
            return natureRes;
        else if (subtype == DamageSubtype.Dark)
            return darkRes;

        // typeless
        else
            return 1f;
    }
    
}