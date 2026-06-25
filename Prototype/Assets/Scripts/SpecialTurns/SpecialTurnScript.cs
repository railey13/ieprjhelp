using UnityEngine;

public abstract class SpecialTurnScript : ScriptableObject
{
    public string specialTurnName = "Special Turn";

    [Header("Trigger")]
    public Skill triggerSkill;
    public bool requireDamage = true;
    public bool requireEnemyTarget = true;
    public bool allowSelfTrigger = false;

    [Header("Limit")]
    public SpecialTurnLimit limit = SpecialTurnLimit.OncePerAction;

    public bool CanTrigger(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner)
    {
        if (owner == null || context == null || runner == null)
            return false;

        if (owner.hp <= 0)
            return false;

        if (context.target == null || context.target.hp <= 0)
            return false;

        if (!allowSelfTrigger && owner == context.user)
            return false;

        if (requireDamage && !context.damagedTarget)
            return false;

        if (requireEnemyTarget && !(context.target is EnemyClass))
            return false;

        if (triggerSkill != null && context.skill != triggerSkill)
            return false;

        if (!runner.tracker.CanUse(owner, this, context.target, limit))
            return false;

        return ExtraCanTrigger(owner, context, runner);
    }

    protected virtual bool ExtraCanTrigger(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner)
    {
        return true;
    }

    public abstract void Activate(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner);
}