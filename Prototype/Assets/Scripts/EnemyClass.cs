using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    public int hp;
    public int atk;
    public int spd;
    public virtual void BasicAttack(PlayerClass target)
    {
        target.TakeDamage(atk);
    }

    public virtual void TakeDamage(int damage)
    {
        hp-=damage;
    }
}
