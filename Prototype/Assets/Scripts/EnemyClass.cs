using UnityEngine;

public class EnemyClass : UnitClass
{
    protected override void Awake()
    {
        // Enzo changes: UnitClass already sets up skillStates so I just use that instead of making another one here
        base.Awake();
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