using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Control Damage")]
public class ResolveControlDamageSkill : Skill
{
    public int damage = 10;
    public bool addUserAtk = false;
    public bool enemiesOnly = true;

    [Header("Resolve Debuff")]
    public int movementMinus = 0;
    public int rangeMinus = 0;
    public int minimumMovement = 0;
    public int minimumRange = 1;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (enemiesOnly && !SkillUtility.IsEnemyFor(user, target))
        {
            Debug.Log(SkillName + " failed because the target is not an enemy");
            return;
        }

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName);

        // Enzo changes: this is the simple version of cc for now
        if (movementMinus > 0)
            target.movement = Mathf.Max(minimumMovement, target.movement - movementMinus);

        if (rangeMinus > 0)
            target.range = Mathf.Max(minimumRange, target.range - rangeMinus);

        Debug.Log(target.UnitName + " movement is now " + target.movement + " and range is now " + target.range);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}
