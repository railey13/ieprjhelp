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

   
    void BattleStart()
    {
        GameObject Player = Instantiate(PlayerPrefab,PlayerPosition);
        GameObject Enemy = Instantiate(EnemyPrefab,EnemyPosition);
        PlayerStats = Player.GetComponent<Stats>();
        EnemyStats = Enemy.GetComponent<Stats>();
        
    }

    public void ActionState()
    {

    }
    public void WinLoseState()
    {

    }
    
    
}
