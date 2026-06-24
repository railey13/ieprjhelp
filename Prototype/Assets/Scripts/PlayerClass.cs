using UnityEngine;

public class PlayerClass : UnitClass
{

    public virtual void BasicAttack(EnemyClass target)
    {
        target.TakeDamage(atk);
    }
    
   
}
