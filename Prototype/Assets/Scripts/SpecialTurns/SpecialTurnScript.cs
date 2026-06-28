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
        if (owner == null)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because owner is null");
            return false;
        }

        if (context == null)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because context is null");
            return false;
        }

        if (runner == null)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because runner is null");
            return false;
        }

        if (owner.hp <= 0)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because " + owner.UnitName + " is dead");
            return false;
        }

        if (context.target == null)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because target is null");
            return false;
        }

        if (context.target.hp <= 0)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because target is dead");
            return false;
        }

        if (!allowSelfTrigger && owner == context.user)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because self trigger is not allowed");
            return false;
        }

        if (requireDamage && !context.damagedTarget)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because target was not damaged");
            return false;
        }

        if (requireEnemyTarget && !(context.target is EnemyClass))
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because target is not an EnemyClass");
            return false;
        }

        if (triggerSkill != null && context.skill != triggerSkill)
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because skill does not match triggerSkill");
            Debug.Log("FOLLOW UP DEBUG: used skill = " + context.skill.SkillName);
            Debug.Log("FOLLOW UP DEBUG: required skill = " + triggerSkill.SkillName);
            return false;
        }

        if (!runner.tracker.CanUse(owner, this, context.target, limit))
        {
            Debug.Log("FOLLOW UP DEBUG: cannot trigger because this special turn was already used");
            return false;
        }

        return ExtraCanTrigger(owner, context, runner);
    }

    protected virtual bool ExtraCanTrigger(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner)
    {
        return true;
    }

    public abstract void Activate(UnitClass owner, SpecialTurnContext context, SpecialTurnRunner runner);
}