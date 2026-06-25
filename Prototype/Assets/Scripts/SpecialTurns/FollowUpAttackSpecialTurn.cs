using UnityEngine;

[CreateAssetMenu(menuName = "Special Turns/Follow Up Attack")]
public class FollowUpAttackSpecialTurn : SpecialTurnScript
{
    [Header("Range")]
    public bool useOwnerRange = true;
    public float customRange = 3f;
    public float rangeBuffer = 0.5f;

    [Header("Damage")]
    public bool useOwnerAttackStat = true;
    public int customDamage = 10;

    protected override bool ExtraCanTrigger(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner)
    {
        float attackRange = useOwnerRange ? owner.range : customRange;
        float distance = Vector3.Distance(owner.transform.position, context.target.transform.position);

        return distance <= attackRange + rangeBuffer;
    }

    public override void Activate(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner)
    {
        runner.tracker.MarkUsed(owner, this, context.target, limit);

        int damage = useOwnerAttackStat ? owner.atk : customDamage;

        context.target.TakeDamage(damage);

        Debug.Log(owner.UnitName + " triggered " + specialTurnName + " on " + context.target.UnitName);
    }
}