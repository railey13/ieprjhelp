using System.Collections.Generic;
using UnityEngine;

public class UnitClass : MonoBehaviour
{
    private Animator animator;

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
    [Header("Type Resistances")]
    public float sharpRes = 1f;
    public float pierceRes = 1f;
    public float bluntRes = 1f;
    public float fireRes = 1f;
    public float natureRes = 1f;
    public float darkRes = 1f;

    [Header("Skills")]
    public List<Skill> skills = new();
    public List<SkillState> skillStates = new();

    [Header("TurnSystem")]
    public float currentCT = 0;
    public float maxCT = 100;
    public bool IsReady = false;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        skillStates.Clear();
        maxHp = hp;
        foreach (Skill skill in skills)
        {
            skillStates.Add(new SkillState { skill = skill });
        }
    }

    public virtual void TakeDamage(float damage, DamageSubtype subtype = DamageSubtype.Sharp)
    {
        PlayHurtAnimation();

        hp -= Mathf.FloorToInt(damage);
        Debug.Log(UnitName + " took " + damage + " damage. Current HP: " + hp);

        HitEffect hitEffect = GetComponent<HitEffect>();
        if (hitEffect != null)
            hitEffect.PlayHitEffect(subtype);

        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    public virtual void PlayHurtAnimation()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetTrigger("Hurt");
    }

    public virtual void PlayAttackAnimation()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetTrigger("Attack");
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
        if (subtype == DamageSubtype.Sharp) return sharpRes;
        else if (subtype == DamageSubtype.Pierce) return pierceRes;
        else if (subtype == DamageSubtype.Blunt) return bluntRes;
        else if (subtype == DamageSubtype.Fire) return fireRes;
        else if (subtype == DamageSubtype.Nature) return natureRes;
        else if (subtype == DamageSubtype.Dark) return darkRes;
        else return 1f;
    }
}