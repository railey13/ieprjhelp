using UnityEngine;

[CreateAssetMenu(menuName = "Skills/EnemyFireball")]
public class EnemyFireballSkill : Skill
{
    public int Multiplier = 2; // Damage multiplier for the skill
    //public int CD;
    public override void Use(UnitClass user, UnitClass target)
    {
        DamageInfo info = new DamageInfo
        {
            category = category,
            subtype = subtype
        };

        float baseDamage = DamageCalculator.CalculateDamage(user, target, info);
        float finalDamage = baseDamage * Multiplier; 
        Debug.Log("Fireball deals " + finalDamage + " to " + target.UnitName);
    }
}