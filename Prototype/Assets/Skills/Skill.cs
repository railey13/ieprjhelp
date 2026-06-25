using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string SkillName;
    public int MPCost;
    public int range;
    public abstract void Use(UnitClass user, UnitClass target);
}