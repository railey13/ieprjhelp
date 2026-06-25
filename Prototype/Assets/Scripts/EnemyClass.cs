using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : UnitClass
{
    public List<SkillState> skillStates = new();

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
        target.TakeDamage(atk);

    }

    public void ShowIntent(bool willAttack,bool willSkill, PlayerClass target)
    {
        string intentText;
        if (willSkill)
        {
            intentText = "Skill used against" + target.UnitName;
        }
        if (willAttack)
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
