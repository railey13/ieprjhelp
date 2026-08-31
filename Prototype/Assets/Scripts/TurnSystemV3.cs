using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


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

    [Header("GameScreenNames")]
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string loseSceneName = "LoseScene";

    [Header("BattleSystem")]
    [SerializeField] private BattleLogger battleLogger;

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

    private EnemyActions enemyAction;

    private bool battleRunning = false;

    private bool waitingForPlayer = false;

    private float rangeBuffer = 0.5f;
    private int TurnNumber = 1;

    public BattleLogger GetBattleLogger { get { return battleLogger; } }
    public List<PlayerClass> GetPlayerList { get { return players; } }
    public List<EnemyClass> GetEnemyList { get { return enemies; } }
    public float GetRangeBuffer { get {return rangeBuffer;} }
    public int GetTurnNumber { get { return TurnNumber; } }

    public bool IsBattleRunning { get { return battleRunning; } }

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

        // make sure enemyaction script is attached to the same object as turnsystem
        enemyAction = this.GetComponent<EnemyActions>();

        SpawnPlayers();
        SpawnEnemies();

        BuildUnitList();

        if(enemyAction) 
            enemyAction.CalculateEnemyIntents();

        battleRunning = true;
    }

    private void Update()
    {
        if (!battleRunning)
            return;

        if (waitingForPlayer)
        {
            Debug.Log("WAITING FOR PLAYER CT STOPPED");
            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                TestEndTurn();
            }   

            return;
        }

        TickChargeTime();
    }

    //PRESS x
    private void TestEndTurn()
    {
    Debug.Log("Skipped turn");

        EndTurn();

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

    public void WinLoseState() {
        enemies.RemoveAll(e => e == null || e.hp <= 0);
        players.RemoveAll(p => p == null || p.hp <= 0);

        bool anyPlayerAlive = players.Count > 0;
        bool anyEnemyAlive = enemies.Count > 0;

        if (!anyEnemyAlive && battleRunning) {
            Debug.Log("All enemies defeated � Loading Win Scene: " + winSceneName);
            battleRunning = false;

            if (!string.IsNullOrEmpty(winSceneName))
                SceneManager.LoadScene(winSceneName);
            else
                Debug.LogError("Win Scene name is not set in the Inspector!");
        }
        else if (!anyPlayerAlive && battleRunning) {
            Debug.Log("All players defeated � Loading Lose Scene: " + loseSceneName);
            battleRunning = false;

            if (!string.IsNullOrEmpty(loseSceneName))
                SceneManager.LoadScene(loseSceneName);
            else
                Debug.LogError("Lose Scene name is not set in the Inspector!");
        }
    }














    ///////////// PLACEHOLDERS////////////


    private void OnAttackClicked()
    {
        Debug.Log("Attack clicked");
    }

    private void OnMoveClicked()
    {
        Debug.Log("Move clicked");
    }

    private void OnSkillsClicked()
    {
        Debug.Log("Skills clicked");
    }

    private void OnEndTurnClicked()
    {
        Debug.Log("End Turn clicked");
        EndTurn();
    }



    ///////////// PLACEHOLDERS////////////

    private void SetupUI()
    {
   
        VisualElement root = uiDocument.rootVisualElement;

        attackButton = root.Q<Button>("Attack");
        moveButton = root.Q<Button>("Move");
        skillsButton = root.Q<Button>("Skills");
        endTurnButton = root.Q<Button>("EndTurnBtn");

        if (attackButton == null)
            Debug.LogError("Attack button not found!");

        if (moveButton == null)
            Debug.LogError("Move button not found!");

        if (skillsButton == null)
            Debug.LogError("Skills button not found!");

        if (endTurnButton == null)
            Debug.LogError("EndTurnBtn not found!");

        attackButton.clicked += OnAttackClicked;
        moveButton.clicked += OnMoveClicked;
        skillsButton.clicked += OnSkillsClicked;
        endTurnButton.clicked += OnEndTurnClicked;
    }

    private void SpawnPlayers()
    {
        players.Clear();

        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            if (i >= playerSpawnPoints.Length)
                break;

            GameObject obj = Instantiate(
                playerPrefabs[i],
                playerSpawnPoints[i].position,
                playerSpawnPoints[i].rotation
            );

            PlayerClass player =
                obj.GetComponent<PlayerClass>()
                ?? obj.GetComponentInChildren<PlayerClass>();

            if (player != null)
            {
                players.Add(player);
            }
            else
            {
                Debug.LogError(
                    "Player prefab does not contain PlayerClass: "
                    + obj.name
                );
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

        if(enemyAction) 
            StartCoroutine(enemyAction.EnemyTakeTurn(enemy));
    }
}
