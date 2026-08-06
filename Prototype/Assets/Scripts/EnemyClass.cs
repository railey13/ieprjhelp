using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : UnitClass
{
    public List<SkillState> skillStates = new List<SkillState>();

    private void Awake()
    {
        skillStates.Clear();

        foreach (Skill skill in skills)
        {
            skillStates.Add(new SkillState
            {
                skill = skill
            });
        }
    }

    public virtual void BasicAttack(PlayerClass target)
    {
        if (target == null)
            return;

        target.TakeDamage(atk);
    }

    public void ShowIntent(bool willAttack, bool willSkill, PlayerClass target)
    {
        if (target == null)
            return;

        string intentText;

        if (willSkill)
        {
            intentText = "Skill used against " + target.UnitName;
        }
        else if (willAttack)
        {
            intentText = "Attack " + target.UnitName;
        }
        else
        {
            intentText = "Move";
        }

        Debug.Log(UnitName + " intends to: " + intentText);
    }
}
