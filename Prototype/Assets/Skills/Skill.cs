using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string SkillName;
    public int MPCost;
    public int range;
    public abstract void Use(UnitClass user, UnitClass target);
    public int cooldown;
    
    [Header("Damage Type")]
    public DamageCategory category = DamageCategory.Physical;
    public DamageSubtype subtype = DamageSubtype.Sharp;
    public DamageMode mode = DamageMode.SingleTarget;
    public int hitCount;
}