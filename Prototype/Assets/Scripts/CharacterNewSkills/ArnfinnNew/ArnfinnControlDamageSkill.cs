using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Arnfinn New/Control Damage Skill")]
public class ArnfinnControlDamageSkill : Skill
{
    [Header("Damage")]
    public int damage = 10;
    public bool addUserAtk = false;

    [Header("Control")]
    public int movementMinus = 999;
    public int minimumMovement = 0;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (!SkillUtility.IsEnemyFor(user, target))
            return;

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName);

        target.movement = Mathf.Max(minimumMovement, target.movement - movementMinus);

        Debug.Log(target.UnitName + " movement is now " + target.movement);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}
