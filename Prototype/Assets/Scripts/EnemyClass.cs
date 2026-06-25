using UnityEngine;

public class EnemyClass : UnitClass
{
    public virtual void BasicAttack(PlayerClass target)
    {
        target.TakeDamage(atk);

    }

    public void ShowIntent(bool willAttack, PlayerClass target)
    {
        string intentText;
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
