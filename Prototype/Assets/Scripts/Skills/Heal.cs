using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal")]
public class HealSkill : Skill
{
    public float healMultiplier = 1.5f;

    public override void Use(UnitClass user, UnitClass target)
    {
        target.hp += Mathf.FloorToInt(user.mag * healMultiplier);
        Debug.Log(user.UnitName + " healed " + target.UnitName + " for " + Mathf.FloorToInt(user.mag * healMultiplier));
    }
}