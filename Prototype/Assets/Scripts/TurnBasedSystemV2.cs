using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TurnBasedSystemV2 : MonoBehaviour
{
    [SerializeField] private GameObject[] PlayerPrefab;
    [SerializeField] private GameObject[] EnemyPrefab;

    [SerializeField] private Transform[] PlayerSpawnPoints;
    [SerializeField] private Transform[] EnemySpawnPoints;

    [SerializeField] private UIDocument doc;
    private Button PMove;
    private Button PAttack;
    private Button PHeal;
    private Button EndTurn;
    // comment the below for enum later
    private bool isMove = false;
    private bool isAttack = false;
    private bool isHeal = false;
    private bool isGameOver = false;
    private enum TurnAction { None, Move, Attack, Heal }
    private TurnAction selectedAction = TurnAction.None;

    private enum TurnPhase { PlayerTurn, EnemyTurn }
    private TurnPhase currentPhase = TurnPhase.PlayerTurn;

    private List<PlayerClass> players = new List<PlayerClass>();
    private List<EnemyClass> enemies = new List<EnemyClass>();
    private List<UnitClass> turnOrder = new();



    //////////////////// 
    
    private int currentTurnIndex=0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BattleStart();


    }

    public void SetUIVisible(bool visible)
    {
        doc.rootVisualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }



    private void BuildTurnOrder()
    {
        turnOrder.Clear();
        foreach (PlayerClass player in players)
        {
            Debug.Log("added");
            turnOrder.Add(player);
        }

        foreach (EnemyClass enemy in enemies)
        {
            turnOrder.Add(enemy);
        }

        turnOrder.Sort((a,b)=>b.speed.CompareTo(a.speed));

        //DEBUG CHECK FOR TURN ORDER
        foreach (UnitClass unit in turnOrder)
        {
            Debug.Log(unit.UnitName + " Speed: " + unit.speed);
        }
    }
    private void BattleStart()
    {
        Debug.Log("BattleStart() called");

        Debug.Log("Players: " + players.Count);
        Debug.Log("Enemies: " + enemies.Count);
        Debug.Log("TurnOrder BEFORE build: " + turnOrder.Count);
        /*
        Debug.Log("Spawning Player");
    

        players.AddRange(FindObjectsOfType<PlayerClass>());
        enemies.AddRange(FindObjectsOfType<EnemyClass>());
        PMove = doc.rootVisualElement.Q<Button>("Move");
        PAttack = doc.rootVisualElement.Q<Button>("Attack");
        PHeal = doc.rootVisualElement.Q<Button>("Heal");
        EndTurn = doc.rootVisualElement.Q<Button>("EndTurn");
        
        PMove.RegisterCallback<ClickEvent>(MoveEnabled);
        PAttack.RegisterCallback<ClickEvent>(AttackEnabled);
        PHeal.RegisterCallback<ClickEvent>(HealEnabled);
        EndTurn.RegisterCallback<ClickEvent>(onEndTurnClicked);
        
        SetUIVisible(true);
        */
        SpawnPlayer();
        SpawnEnemy();

        Debug.Log("PLAYERS: " + players.Count);
        Debug.Log("ENEMIES: " + enemies.Count);

        BuildTurnOrder();
        Debug.Log("TURN ORDER: " + turnOrder.Count);

        currentTurnIndex = 0;
        StartTurn();

    }

    private void StartTurn()
    {
        if (turnOrder == null || turnOrder.Count == 0)
        {
            Debug.LogError("TURN ORDER IS EMPTY — spawn system failed");
            return;
        }

        if (currentTurnIndex < 0 || currentTurnIndex >= turnOrder.Count)
        {
            Debug.LogError("TURN INDEX OUT OF RANGE: " + currentTurnIndex);
            return;
        }

        UnitClass unit = CurrentUnit;

        if (unit == null)
        {
            Debug.LogError("CurrentUnit is NULL (missing UnitClass on prefab)");
            return;
        }

        Debug.Log("Current Turn: " + unit.UnitName);
    }
    private UnitClass CurrentUnit
    {
        get
        {
            if (turnOrder == null || turnOrder.Count == 0)
            {
                Debug.LogError("TurnOrder is EMPTY when accessing CurrentUnit");
                return null;
            }

            if (currentTurnIndex < 0 || currentTurnIndex >= turnOrder.Count)
            {
                Debug.LogError("CurrentTurnIndex out of range: " + currentTurnIndex);
                return null;
            }

            return turnOrder[currentTurnIndex];
        }
    }




    private void SpawnPlayer()
    {
        players.Clear();

        for (int i = 0; i < PlayerPrefab.Length; i++)
        {
            if (i >= PlayerSpawnPoints.Length)
                break;

            GameObject playerObject = Instantiate(
                PlayerPrefab[i],
                PlayerSpawnPoints[i].position,
                PlayerSpawnPoints[i].rotation
            );

            PlayerClass player =
                playerObject.GetComponent<PlayerClass>()
                ?? playerObject.GetComponentInChildren<PlayerClass>();

            if (player == null)
            {
                Debug.LogError("PlayerPrefab missing PlayerClass: " + playerObject.name);
                continue;
            }

            players.Add(player);
        }
    }

    private void SpawnEnemy()
    {
        enemies.Clear();

        for (int i = 0; i < EnemyPrefab.Length; i++)
        {
            if (i >= EnemySpawnPoints.Length)
                break;

            GameObject enemyObject = Instantiate(
                EnemyPrefab[i],
                EnemySpawnPoints[i].position,
                EnemySpawnPoints[i].rotation
            );

            EnemyClass enemy =
                enemyObject.GetComponent<EnemyClass>()
                ?? enemyObject.GetComponentInChildren<EnemyClass>();

            if (enemy == null)
            {
                Debug.LogError("EnemyPrefab missing EnemyClass: " + enemyObject.name);
                continue;
            }

            enemies.Add(enemy);
        }
    }

}
