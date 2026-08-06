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

        // Enzo changes: check if the follow-up manager is active in the scene
        Debug.Log("FOLLOW UP DEBUG: SpecialTurnRunner is active");
    }

    public void ReportSkillUse(UnitClass user, UnitClass target, Skill skill, int targetHpBefore)
    {
        if (user == null)
        {
            Debug.Log("FOLLOW UP DEBUG: user is null");
            return;
        }

        if (target == null)
        {
            Debug.Log("FOLLOW UP DEBUG: target is null");
            return;
        }

        tracker.BeginAction();

        // Enzo changes: skill can be null here because basic attacks are not skill assets
        string actionName = skill != null ? skill.SkillName : "Basic Attack";

        int damageAmount = Mathf.Max(0, targetHpBefore - target.hp);
        bool damagedTarget = damageAmount > 0;

        Debug.Log("FOLLOW UP DEBUG: user = " + user.UnitName);
        Debug.Log("FOLLOW UP DEBUG: action = " + actionName);
        Debug.Log("FOLLOW UP DEBUG: target = " + target.UnitName);
        Debug.Log("FOLLOW UP DEBUG: target hp before = " + targetHpBefore + ", after = " + target.hp);
        Debug.Log("FOLLOW UP DEBUG: damage amount = " + damageAmount);

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
        // Enzo changes: find every unit that has follow-up scripts
        SpecialTurnHolder[] holders = FindObjectsOfType<SpecialTurnHolder>();

        Debug.Log("FOLLOW UP DEBUG: holders found = " + holders.Length);

        foreach (SpecialTurnHolder holder in holders)
        {
            if (holder == null)
                continue;

            UnitClass owner = holder.GetComponent<UnitClass>();

            if (owner == null)
            {
                Debug.Log("FOLLOW UP DEBUG: holder has no UnitClass");
                continue;
            }

            if (owner.hp <= 0)
            {
                Debug.Log("FOLLOW UP DEBUG: " + owner.UnitName + " is dead");
                continue;
            }

            if (holder.specialTurns == null || holder.specialTurns.Count == 0)
            {
                Debug.Log("FOLLOW UP DEBUG: " + owner.UnitName + " has no special turns");
                continue;
            }

            foreach (SpecialTurnScript specialTurn in holder.specialTurns)
            {
                if (specialTurn == null)
                {
                    Debug.Log("FOLLOW UP DEBUG: special turn is null on " + owner.UnitName);
                    continue;
                }

                Debug.Log("FOLLOW UP DEBUG: checking " + specialTurn.specialTurnName + " for " + owner.UnitName);

                if (specialTurn.CanTrigger(owner, context, this))
                {
                    Debug.Log("FOLLOW UP DEBUG: activating " + specialTurn.specialTurnName);
                    specialTurn.Activate(owner, context, this);
                }
                else
                {
                    Debug.Log("FOLLOW UP DEBUG: cannot trigger " + specialTurn.specialTurnName);
                }
            }
        }
    }
}
