using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    public string PlayerName;
    public int hp;
    public int atk;
    public int speed;
    public int movement;

    public virtual void BasicAttack(EnemyClass target)
    {
        target.TakeDamage(atk);
    }
    public virtual void TakeDamage(int damage)
    {
        hp-=damage;     
    }
    
}
