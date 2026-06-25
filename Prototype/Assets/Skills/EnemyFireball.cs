using UnityEngine;

[CreateAssetMenu(menuName = "Skills/EnemyFireball")]
public class EnemyFireballSkill : Skill
{
    public int Damage;
    //public int CD;
    public override void Use(UnitClass user, UnitClass target)
    {
        target.TakeDamage(Damage);
        Debug.Log("Fireball deals " + Damage + " to " + target.UnitName);
    }
}