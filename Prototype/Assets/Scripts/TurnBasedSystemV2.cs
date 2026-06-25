using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
public class TurnBasedSystemV2 : MonoBehaviour
{
    [SerializeField] private GameObject[] PlayerPrefab;
    [SerializeField] private GameObject[] EnemyPrefab;

    [SerializeField] private Transform[] PlayerSpawnPoints;
    [SerializeField] private Transform[] EnemySpawnPoints;
    private float playerSpawnYOffset = 1f;

    [SerializeField] private UIDocument doc;
    private Button PMove;
    private Button PAttack;
    private Button PHeal;
    private Button EndTurnBtn;

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

    private int currentTurnIndex = 0;
    private EnemyClass selectedTarget;
    private bool isTargeting = false;
    /// /////////////

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BattleStart();
    }

    void Update()
    {
        if (!isTargeting) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // if the click hit nothing, or didn't hit an enemy, cancel targeting
        bool hitEnemy = false;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitEnemy = hit.transform.GetComponentInParent<EnemyTargetable>() != null;
        }

        if (!hitEnemy)
        {
            CancelTargeting();
        }
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

        turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

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
        */

        PMove = doc.rootVisualElement.Q<Button>("Move");
        PAttack = doc.rootVisualElement.Q<Button>("Attack");
        PHeal = doc.rootVisualElement.Q<Button>("Heal");
        EndTurnBtn = doc.rootVisualElement.Q<Button>("EndTurnBtn");

        PMove.RegisterCallback<ClickEvent>(MoveEnabled);
        PAttack.RegisterCallback<ClickEvent>(AttackEnabled);
        PHeal.RegisterCallback<ClickEvent>(HealEnabled);
        EndTurnBtn.RegisterCallback<ClickEvent>(onEndTurnClicked);

        SetUIVisible(true);

        SpawnPlayer();
        SpawnEnemy();

        Debug.Log("PLAYERS: " + players.Count);
        Debug.Log("ENEMIES: " + enemies.Count);

        BuildTurnOrder();
        Debug.Log("TURN ORDER: " + turnOrder.Count);

        currentTurnIndex = 0;
        StartTurn();

    }
    private void AttackEnabled(ClickEvent evt)
    {
        Debug.Log("Select A Target  ");
        selectedAction = TurnAction.Attack;
        isTargeting = true;
        SetUIVisible(false);
        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.ShowAttackRange();
        }
    }
    public void SelectTarget(EnemyClass enemy)
    {
        if (!isTargeting)
            return;
        SetUIVisible(true); // add this
        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.HideAttackRange();
        }

        if (selectedTarget != null)
        {
            EnemyTargetable previousTargetable = selectedTarget.GetComponent<EnemyTargetable>();
            if (previousTargetable != null)
                previousTargetable.SetHighlighted(false);
        }
        selectedTarget = enemy;
        isTargeting = false;
        EnemyTargetable targetable = enemy.GetComponent<EnemyTargetable>();
        if (targetable != null)
            targetable.SetHighlighted(true);


        Debug.Log("Target selected: " + enemy.UnitName);
    }
    private void MoveEnabled(ClickEvent evt)
    {
        Debug.Log("Move Clicked");
        selectedAction = TurnAction.Move;

        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement activePlayerMovement = currentPlayer.GetComponent<PlayerMovement>();
            if (activePlayerMovement != null)
            {
                activePlayerMovement.ActivateMovement();
                SetUIVisible(false);
            }
        }
    }

    private void HealEnabled(ClickEvent evt)
    {
        Debug.Log("Heal Clicked");
        selectedAction = TurnAction.Heal;
    }

    private void StartTurn()
    {/*
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
        */

        UnitClass unit = CurrentUnit;

        if (unit == null)
            return;

        Debug.Log("TURN START: " + unit.UnitName);

        if (unit is PlayerClass currentPlayer)
        {
            SetUIVisible(true);
            PlayerMovement activePlayerMovement = currentPlayer.GetComponent<PlayerMovement>();
            if (activePlayerMovement != null)
            {
                activePlayerMovement.setResetOrigin(true);
            }

        }
        else if (unit is EnemyClass enemy)
        {
            SetUIVisible(false);
            EnemyTakeTurn(enemy);
        }
    }

    private void onEndTurnClicked(ClickEvent evt)
    {
        if (!(CurrentUnit is PlayerClass currentPlayer))
            return;


        switch (selectedAction)
        {
            case TurnAction.Attack:
                Debug.Log("Player attacks");
                // enemies[0].TakeDamage(67); // pick a real target later

                if (selectedTarget != null && selectedTarget.hp > 0)
                {
                    float distance = Vector3.Distance(selectedTarget.transform.position, currentPlayer.transform.position);

                    // range check incase player moves AFTER targetting
                    if (distance <= currentPlayer.range)
                    {
                        Debug.Log(currentPlayer.UnitName + " attacks " + selectedTarget.UnitName);
                        selectedTarget.TakeDamage(CurrentUnit.atk);
                    }
                    else
                    {
                        Debug.Log("Attack failed");
                    }
                }
                else if (selectedTarget == null || selectedTarget.hp <= 0)
                {
                    Debug.Log("No valid target selected");

                }
                break;

            case TurnAction.Heal:
                Debug.Log("Player heals");
                break;

            case TurnAction.Move:
                Debug.Log("Player moves");
                break;

            case TurnAction.None:
                Debug.Log("No action selected");
                break;
        }
        EndTurn();
        // selectedAction = TurnAction.None;



    }

    private void NextTurn()
    {
        if (isGameOver) return; // extra safety net
        if (turnOrder.Count == 0)
            return;
        //chat gpt idea of safety check
        int safety = 0;

        do
        {
            currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
            safety++;

            if (safety > 100)
            {
                Debug.LogError("Infinite loop prevented in NextTurn()");
                return;
            }

        } while (turnOrder[currentTurnIndex] == null ||
                 turnOrder[currentTurnIndex].hp <= 0);

        StartTurn();
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



    private void EnemyTakeTurn(EnemyClass enemy)
    {
        if (isGameOver)
        {
            return;
        }

        Debug.Log(enemy.UnitName + " acts. players.Count = " + players.Count);
        players.RemoveAll(p => p == null || p.hp <= 0);

        PlayerClass target = FindNearestPlayer(enemy.transform.position);
        if (target == null)
        {
            Debug.Log("No target — checking WinLoseState");
            WinLoseState();
            Debug.Log("isGameOver after WinLoseState: " + isGameOver);
            EndTurn();
            return;
        }

        EnemyMove(enemy);

        float distance = Vector3.Distance(enemy.transform.position, target.transform.position);

        // range check
        if (distance <= enemy.range * 2)
        {
            Debug.Log(enemy.UnitName + " attacks " + target.UnitName);
            target.TakeDamage(enemy.atk);
        }



        WinLoseState();

        if (isGameOver)
        {
            Debug.Log("Game over — halting turn loop");
            return;
        }
        EndTurn();
    }
    private void EndTurn()
    {
        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.HideAttackRange();
        }
        selectedAction = TurnAction.None;
        if (selectedTarget != null)
        {
            EnemyTargetable targetable = selectedTarget.GetComponent<EnemyTargetable>();
            if (targetable != null)
                targetable.SetHighlighted(false);
        }
        selectedTarget = null;
        isTargeting = false;
        NextTurn();
    }

    private void SpawnPlayer()
    {
        players.Clear();

        for (int i = 0; i < PlayerPrefab.Length; i++)
        {
            if (i >= PlayerSpawnPoints.Length)
                break;

            // OFFSETS PLAYERS UP
            Vector3 spawnPos = PlayerSpawnPoints[i].position + Vector3.up * playerSpawnYOffset ;

            GameObject playerObject = Instantiate(
                PlayerPrefab[i],
                spawnPos,
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

            PlayerMovement pm = playerObject.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.SetTurnBasedSystem(this);
            }
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
    public void OnPlayerMoveConfirmed()
    {
        Debug.Log("Player finished moving");
        selectedAction = TurnAction.None;
        WinLoseState();
        if (isGameOver) return;
        SetUIVisible(true);
    }

    public void OnPlayerMoveCancelled()
    {
        Debug.Log("Player cancelled movement");
        selectedAction = TurnAction.None;
        SetUIVisible(true);
    }

    public void WinLoseState()
    {
        bool anyPlayerAlive = players.Exists(p => p != null && p.hp > 0);
        bool anyEnemyAlive = enemies.Exists(e => e != null && e.hp > 0);
        enemies.RemoveAll(e => e == null || e.hp <= 0);
        players.RemoveAll(p => p == null || p.hp <= 0);

        if (!anyEnemyAlive)
        {
            Debug.Log("All enemies defeated — Win!");
            isGameOver = true;
        }
        else if (!anyPlayerAlive)
        {
            Debug.Log("All players defeated — Lose.");
            isGameOver = true;
        }
    }


    private PlayerClass FindNearestPlayer(Vector3 fromPosition)
    {
        PlayerClass nearest = null;
        float nearestDist = float.MaxValue;

        foreach (PlayerClass p in players)
        {
            if (p == null || p.hp <= 0) continue;

            float dist = Vector3.Distance(fromPosition, p.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }
    public void EnemyMove(EnemyClass enemy)
    {
        if (enemy == null) return;

        PlayerClass target = FindNearestPlayer(enemy.transform.position);
        if (target == null)
        {
            Debug.Log(enemy.UnitName + " found no living player to move toward");
            return;
        }

        Vector3 toTarget = target.transform.position - enemy.transform.position;
        toTarget.y = 0f; // keep movement flat on the ground plane

        float distance = toTarget.magnitude;
        Vector3 direction = toTarget.normalized;

        float moveDistance = Mathf.Min(distance, enemy.movement);
        Vector3 destination = enemy.transform.position + direction * moveDistance;

        Debug.Log(enemy.UnitName + " moves toward " + target.UnitName +
                  " (" + moveDistance + " units)");

        enemy.transform.position = destination;
    }
    public UnitClass GetCurrentUnit()
    {
        return CurrentUnit;
    }
    public void CancelTargeting()
    {
        isTargeting = false;
        selectedAction = TurnAction.None;
        SetUIVisible(true); // add this
        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.HideAttackRange();
        }

        Debug.Log("Targeting cancelled — out of range");
    }

}
