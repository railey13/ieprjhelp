using UnityEngine;

public class EnemyClass : UnitClass
{
    public virtual void BasicAttack(PlayerClass target)
    {
        target.TakeDamage(atk);
        Debug.Log(
    UnitName +
    " took " + atk +
    " damage. Current HP: " + hp
);
    }

  
}
