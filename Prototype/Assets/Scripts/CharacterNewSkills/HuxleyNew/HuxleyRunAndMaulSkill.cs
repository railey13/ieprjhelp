using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Huxley New/Run And Maul Skill")]
public class HuxleyRunAndMaulSkill : Skill
{
    [Header("Run And Maul")]
    public int damage = 30;
    public bool addUserAtk = true;
    public float stopDistance = 1.5f;

    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null || target == null)
            return;

        if (!SkillUtility.IsEnemyFor(user, target))
            return;

        Vector3 direction = user.transform.position - target.transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();

            // Enzo changes: I move Huxley near the target before damage resolves
            user.transform.position = target.transform.position + direction * stopDistance;
        }

        int finalDamage = SkillUtility.BuildDamage(user, damage, addUserAtk);

        SkillUtility.DealDamage(user, target, finalDamage, SkillName);

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}
