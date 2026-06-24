using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Fireball")]
public class FireballSkill : Skill
{
    public int Damage;

    public override void Use(UnitClass user, UnitClass target)
    {
        target.TakeDamage(Damage);
    }
}