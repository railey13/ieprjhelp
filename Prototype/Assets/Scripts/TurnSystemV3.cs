using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TurnBasedSystemV3 : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("UI")]
    [SerializeField] private UIDocument uiDocument;

    private Button attackButton;
    private Button moveButton;
    private Button skillsButton;
    private Button endTurnButton;

    private List<PlayerClass> players = new();
    private List<EnemyClass> enemies = new();

    private List<UnitClass> allUnits = new();

    private UnitClass currentUnit;

    private EnemyClass selectedTarget;
    private Skill selectedSkill;

    private bool battleRunning = false;
    private bool waitingForPlayer = false;

    private enum TurnState
    {
        Waiting,
        Moving,
        TargetingAttack,
        TargetingSkill
    }

    private TurnState currentState = TurnState.Waiting;
    void Start()
    {
        Debug.Log("TurnBasedSystemV3 Start");
        SetupUI();

        SpawnPlayers();
        SpawnEnemies();

        BuildUnitList();

        battleRunning = true;
    }

    private void Update()
    {
        if (!battleRunning)
            return;

        if (waitingForPlayer)
            return;

        TickChargeTime();
    }

    private void BuildUnitList()
    {
        allUnits.Clear();

        foreach (PlayerClass p in players)
            allUnits.Add(p);

        foreach (EnemyClass e in enemies)
            allUnits.Add(e);
    }

    private void TickChargeTime()
    {
        foreach (UnitClass unit in allUnits)
        {
            if (!unit.IsAlive())
                continue;

            unit.currentCT += unit.speed * Time.deltaTime;

            Debug.Log(unit.UnitName + " CT: " + unit.currentCT);

            if (unit.currentCT >= unit.maxCT)
            {
                unit.currentCT = unit.maxCT;
                BeginTurn(unit);
                return;
            }
        }
    }

    private void BeginTurn(UnitClass unit)
    {
        currentUnit = unit;
        currentUnit.currentCT = 0;

        if (unit is PlayerClass)
        {
            waitingForPlayer = true;
            ShowPlayerUI(true);
        }
        else
        {
            EnemyTurn((EnemyClass)unit);
        }
    }

    public void EndTurn()
    {
        ShowPlayerUI(false);

        waitingForPlayer = false;

        currentUnit = null;
    }

    private void SetupUI()
    {
        // we'll do this later
    }

    private void SpawnPlayers()
    {
       players.Clear();
        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            if (i >= playerSpawnPoints.Length)
                break;

            GameObject obj = Instantiate(playerPrefabs[i], playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);

            PlayerClass player = obj.GetComponent<PlayerClass>();

            if(player != null)
            {
                players.Add(player);
            }
        }
    }

    private void SpawnEnemies()
    {
        enemies.Clear();

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (i >= enemySpawnPoints.Length)
                break;

            GameObject obj = Instantiate(
                enemyPrefabs[i],
                enemySpawnPoints[i].position,
                enemySpawnPoints[i].rotation
            );

            EnemyClass enemy = obj.GetComponent<EnemyClass>();

            if (enemy != null)
                enemies.Add(enemy);
        }
    }

    private void ShowPlayerUI(bool visible)
    {
        // we'll do this later
    }

    private void EnemyTurn(EnemyClass enemy)
    {
        Debug.Log(enemy.UnitName + " acts.");

        EndTurn();
    }
}
