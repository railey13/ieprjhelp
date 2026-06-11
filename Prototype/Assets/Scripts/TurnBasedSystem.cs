using UnityEngine;
using System.Collections.Generic;

public class TurnBasedSystem : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject EnemyPrefab;

    [SerializeField] private Transform PlayerPosition;
    [SerializeField] private Transform EnemyPosition;

    Stats PlayerStats;
    Stats EnemyStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BattleStart();
    }

   
    private void BattleStart()
    {

        Debug.Log("Spawning Player");
        GameObject Player = Instantiate(PlayerPrefab,PlayerPosition.position,Quaternion.identity);

        Debug.Log("Spawning Enemy");
        GameObject Enemy = Instantiate(EnemyPrefab,EnemyPosition);

        PlayerStats = Player.GetComponent<Stats>();
        Debug.Log(PlayerStats.Name);
        EnemyStats = Enemy.GetComponent<Stats>();
        
    }

    public void EnemyMove()
    {

    }
    public void ActionState()
    {

    }
    public void WinLoseState()
    {

    }
    
    
}
