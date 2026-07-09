using System.Collections.Generic;
using UnityEngine;

public enum enemyType { Melee1, Ranged1, Magician1, Boss1 };
public class EnemyClass : UnitClass
{
    [Header("Enemy Variables")]
    public enemyType enemyType;

    public virtual void BasicAttack(PlayerClass target)
    {
        target.TakeDamage(atk);

    }

    public void ShowIntent(bool willAttack, bool willSkill, PlayerClass target)
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
