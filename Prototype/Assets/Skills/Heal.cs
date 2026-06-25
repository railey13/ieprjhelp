using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal")]
public class HealSkill : Skill
{
    public int healAmount = 30;

    public override void Use(UnitClass user, UnitClass target)
    {
        target.hp += healAmount;
        Debug.Log(user.UnitName + " healed " + target.UnitName + " for " + healAmount);
    }
}