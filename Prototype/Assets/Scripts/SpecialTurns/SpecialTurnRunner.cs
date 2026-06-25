using System.Collections.Generic;
using UnityEngine;

public enum SpecialTurnLimit
{
    OncePerAction,
    OncePerBattle,
    Unlimited
}

public class SpecialTurnContext
{
    public UnitClass user;
    public UnitClass target;
    public Skill skill;
    public bool damagedTarget;
    public int damageAmount;

    public SpecialTurnContext(UnitClass user, UnitClass target, Skill skill, bool damagedTarget, int damageAmount)
    {
        this.user = user;
        this.target = target;
        this.skill = skill;
        this.damagedTarget = damagedTarget;
        this.damageAmount = damageAmount;
    }
}

public class SpecialTurnTracker
{
    private int actionId = 0;
    private HashSet<string> usedFlags = new HashSet<string>();

    public void BeginAction()
    {
        actionId++;
    }

    public bool CanUse(UnitClass owner, SpecialTurnScript specialTurn, UnitClass target, SpecialTurnLimit limit)
    {
        if (limit == SpecialTurnLimit.Unlimited)
            return true;

        string key = BuildKey(owner, specialTurn, target, limit);
        return !usedFlags.Contains(key);
    }

    public void MarkUsed(UnitClass owner, SpecialTurnScript specialTurn, UnitClass target, SpecialTurnLimit limit)
    {
        if (limit == SpecialTurnLimit.Unlimited)
            return;

        string key = BuildKey(owner, specialTurn, target, limit);
        usedFlags.Add(key);
    }

    private string BuildKey(UnitClass owner, SpecialTurnScript specialTurn, UnitClass target, SpecialTurnLimit limit)
    {
        string baseKey = owner.GetInstanceID() + "_" + specialTurn.name + "_" + target.GetInstanceID();

        if (limit == SpecialTurnLimit.OncePerAction)
            return baseKey + "_action_" + actionId;

        if (limit == SpecialTurnLimit.OncePerBattle)
            return baseKey + "_battle";

        return baseKey;
    }
}

public class SpecialTurnRunner : MonoBehaviour
{
    public static SpecialTurnRunner Instance { get; private set; }

    public SpecialTurnTracker tracker = new SpecialTurnTracker();

    private void Awake()
    {
        Instance = this;
    }

    public void ReportSkillUse(UnitClass user, UnitClass target, Skill skill, int targetHpBefore)
    {
        if (target == null)
            return;

        tracker.BeginAction();

        int damageAmount = Mathf.Max(0, targetHpBefore - target.hp);
        bool damagedTarget = damageAmount > 0;

        SpecialTurnContext context = new SpecialTurnContext(
            user,
            target,
            skill,
            damagedTarget,
            damageAmount
        );

        ResolveSpecialTurns(context);
    }

    private void ResolveSpecialTurns(SpecialTurnContext context)
    {
        SpecialTurnHolder[] holders = FindObjectsOfType<SpecialTurnHolder>();

        foreach (SpecialTurnHolder holder in holders)
        {
            if (holder == null)
                continue;

            UnitClass owner = holder.GetComponent<UnitClass>();

            if (owner == null || owner.hp <= 0)
                continue;

            foreach (SpecialTurnScript specialTurn in holder.specialTurns)
            {
                if (specialTurn == null)
                    continue;

                if (specialTurn.CanTrigger(owner, context, this))
                {
                    specialTurn.Activate(owner, context, this);
                }
            }
        }
    }
}
