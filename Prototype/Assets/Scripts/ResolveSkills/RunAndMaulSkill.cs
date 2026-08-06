using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Run And Maul")]
public class RunAndMaulSkill : Skill
{
    public int damage = 30;
    public bool addUserAtk = true;
    public float stopDistance = 1.5f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (!SkillUtility.IsEnemyFor(user, target))
        {
            Debug.Log(SkillName + " failed because the target is not an enemy");
            return;
        }

        Vector3 direction = user.transform.position - target.transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();

            // Enzo changes: I move Huxley near the enemy before the damage resolves
            user.transform.position = target.transform.position + direction * stopDistance;
        }

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}
