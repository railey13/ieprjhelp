using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Fireball")]
public class FireballSkill : Skill
{
    public float Multiplier = 2; // Damage multiplier for the skill

    public override void Use(UnitClass user, UnitClass target)
    {
        DamageInfo info = new DamageInfo
        {
            category = category,
            subtype = subtype
        };

        float baseDamage = DamageCalculator.CalculateDamage(user, target, info);
        float finalDamage = baseDamage * Multiplier; // Apply the skill's damage multiplier
        target.TakeDamage(finalDamage);
    }
}