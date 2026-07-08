using UnityEngine;
public enum DamageCategory { Physical, Magical }
public enum DamageSubtype { Sharp, Pierce, Blunt, Fire, Nature, Dark, None }
public enum DamageMode {SingleTarget,AreaofEffect,Cone}//just suggestions for AOE/Cone
public struct DamageInfo
{
    public float amount;
    public int hitCount;
    public DamageCategory category;
    public DamageSubtype subtype;
    public DamageMode mode;
}
public static class DamageCalculator
{
    private const float MitigationConstant = 50f;

    public static int CalculateDamage(UnitClass attacker, UnitClass defender, DamageInfo info)
    {
        float baseStat;

        if (info.category == DamageCategory.Physical)
            baseStat = attacker.atk;
        else
            baseStat = attacker.mag;

        float generalDef;

        if (info.category == DamageCategory.Physical)
            generalDef = defender.physDef;
        else
            generalDef = defender.magDef;

        float mitigation = MitigationConstant / (MitigationConstant + generalDef);
        float subtypeMult = defender.GetResistance(info.subtype);

        float rawDamage = 0;
        for (int i = 0; i < info.hitCount; i++)
        {
            rawDamage += (baseStat * mitigation * subtypeMult);
            Debug.Log("HitCount: "+i+": "+rawDamage);
        }

        int finalDamage = Mathf.FloorToInt(rawDamage);

        return finalDamage;
    }
}