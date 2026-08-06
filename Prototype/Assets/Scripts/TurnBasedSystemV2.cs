using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Collections;
public class TurnBasedSystemV2 : MonoBehaviour
{
    [SerializeField] private GameObject[] PlayerPrefab;
    [SerializeField] private GameObject[] EnemyPrefab;

    [SerializeField] private BattleLogger battleLogger;

    [SerializeField] private Transform[] PlayerSpawnPoints;
    [SerializeField] private Transform[] EnemySpawnPoints;
    private float playerSpawnYOffset = 1f;

    [SerializeField] private UIDocument doc;
    private Button PMove;
    private Button PAttack;
    private Button PHeal;
    private Button EndTurnBtn;
    private Button PSkills;

    private bool isMove = false;
    private bool isAttack = false;
    private bool isHeal = false;
    private bool isGameOver = false;
    private enum TurnAction { None, Move, Attack, Skill }
    private TurnAction selectedAction = TurnAction.None;

    private enum TurnPhase { PlayerTurn, EnemyTurn }
    private TurnPhase currentPhase = TurnPhase.PlayerTurn;

    private List<PlayerClass> players = new List<PlayerClass>();
    private List<EnemyClass> enemies = new List<EnemyClass>();
    private List<UnitClass> turnOrder = new();
    private float rangeBuffer = 0.5f;
    private List<EnemyIntent> enemyIntents = new List<EnemyIntent>();


    //////////////////// 

    private int currentTurnIndex = 0;
    private bool isNewRound = false;
    private EnemyClass selectedEnemyTarget;
    private PlayerClass selectedPlayerTarget;
    private bool isTargeting = false;
    public bool GetisTargetting() { return isTargeting; }
    private Skill selectedSkill;
    private int TurnNumber = 1;
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
            hitEnemy = hit.transform.GetComponentInParent<TargetAndHighlight>() != null;
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

        PMove = doc.rootVisualElement.Q<Button>("Move");
        PAttack = doc.rootVisualElement.Q<Button>("Attack");
        // PHeal = doc.rootVisualElement.Q<Button>("Heal");
        PSkills = doc.rootVisualElement.Q<Button>("Skills");
        EndTurnBtn = doc.rootVisualElement.Q<Button>("EndTurnBtn");
        Debug.Log("EndTurnBtn found? " + (EndTurnBtn != null));

        PMove.RegisterCallback<ClickEvent>(MoveEnabled);
        PAttack.RegisterCallback<ClickEvent>(AttackEnabled);
        PSkills.RegisterCallback<ClickEvent>(SkillEnabled);
        // PHeal.RegisterCallback<ClickEvent>(HealEnabled);
        Debug.Log("Registering EndTurn callback");
        EndTurnBtn.RegisterCallback<ClickEvent>(onEndTurnClicked);

        SetUIVisible(true);

        SpawnPlayer();
        SpawnEnemy();

        Debug.Log("PLAYERS: " + players.Count);
        Debug.Log("ENEMIES: " + enemies.Count);

        BuildTurnOrder();
        Debug.Log("TURN ORDER: " + turnOrder.Count);

        UpdateTurnOrderUI();
        HideUnitStats();

        currentTurnIndex = 0;
        CalculateEnemyIntents();
        StartTurn();

    }
    private void AttackEnabled(ClickEvent evt)
    {
        HideSkillPanel();
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
    public void SelectPlayerTarget(PlayerClass player)
    {
        if (!isTargeting) return;

        SetUIVisible(true);

        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.HideAttackRange();
                pm.HideSkillRange();
            }
        }

        if (selectedPlayerTarget != null)
        {
            TargetAndHighlight previousTargetable = selectedPlayerTarget.GetComponent<TargetAndHighlight>();
            if (previousTargetable != null)
                previousTargetable.SetHighlighted(false);
        }

        selectedPlayerTarget = player;
        isTargeting = false;

        TargetAndHighlight targetable = player.GetComponent<TargetAndHighlight>();
        if (targetable != null)
            targetable.SetHighlighted(true);

        Debug.Log("Player target selected: " + player.UnitName);
    }

    public void SelectEnemyTarget(EnemyClass enemy)
    {
        if (!isTargeting)
            return;
        SetUIVisible(true); // add this
        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.HideAttackRange();
            pm.HideSkillRange();
        }

        if (selectedEnemyTarget != null)
        {
            TargetAndHighlight previousTargetable = selectedEnemyTarget.GetComponent<TargetAndHighlight>();
            if (previousTargetable != null)
                previousTargetable.SetHighlighted(false);
        }
        selectedEnemyTarget = enemy;
        isTargeting = false;
        TargetAndHighlight targetable = enemy.GetComponent<TargetAndHighlight>();
        if (targetable != null)
            targetable.SetHighlighted(true);


        Debug.Log("Target selected: " + enemy.UnitName);
    }
    private void MoveEnabled(ClickEvent evt)
    {
        HideSkillPanel();
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
    ///SKILLS SELECTION TAB
    private void SkillEnabled(ClickEvent evt)
    {
        Debug.Log("Skill Clicked");
        if (!(CurrentUnit is PlayerClass player))
            return;

        Debug.Log("Open Skill Menu");

        ShowSkillMenu(player);
    }
    private void HideSkillPanel()
    {
        VisualElement skillPanel = doc.rootVisualElement.Q<VisualElement>("SkillPanel");
        if (skillPanel != null)
            skillPanel.style.display = DisplayStyle.None;
    }
    private void ShowSkillMenu(PlayerClass player)
    {
        VisualElement skillPanel =
    doc.rootVisualElement.Q<VisualElement>("SkillPanel");

        if (skillPanel == null)
        {
            Debug.LogError("SkillPanel not found");
            return;
        }

        skillPanel.Clear();
        skillPanel.style.display = DisplayStyle.Flex;

        //foreach (Skill skill in player.skills)
        //{
        //    Debug.Log("Adding skill button: " + skill.SkillName);
        //    Button button = new Button();

        //    button.text = skill.SkillName;

        //    button.clicked += () =>
        //    {
        //        SelectSkill(skill);
        //    };

        //    skillPanel.Add(button);
        //}
        //Skill Menu now Shows the Cooldown
        foreach (SkillState state in player.skillStates)
        {
            Button button = new Button();

            button.text = state.skill.SkillName;

            if (!state.IsReady(TurnNumber))
            {
                int remaining =
                    state.skill.cooldown - (TurnNumber - state.lastUsedTurn);

                button.text += $" ({remaining} turns)";
                button.SetEnabled(false);
            }

            button.clicked += () =>
            {
                SelectSkill(state.skill);
            };

            skillPanel.Add(button);
        }
    }

    private void SelectSkill(Skill skill)
    {
        selectedSkill = skill;
        selectedAction = TurnAction.Skill;
        isTargeting = true;

        VisualElement skillPanel =
            doc.rootVisualElement.Q<VisualElement>("SkillPanel");

        skillPanel.style.display = DisplayStyle.None;

        SetUIVisible(false);

        // show skill range circle
        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.ShowSkillRange(skill.range);
        }

        Debug.Log("Selected Skill: " + skill.SkillName);
    }

    /// ////////////////////////////////////////

    private void StartTurn()
    {
        UpdateTurnOrderUI();

        UnitClass unit = CurrentUnit;

        if (unit == null)
            return;

        if (battleLogger != null) battleLogger.AddEntry(unit.UnitName + "'s turn.");

        Debug.Log("TURN START: " + unit.UnitName);

        if (isNewRound)
        {
            CalculateEnemyIntents(); // only recalculate at the start of a new round
        }

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
            // hide all intent displays before enemy acts
            foreach (EnemyClass e in enemies)
            {
                EnemyIntentDisplay d = e.GetComponent<EnemyIntentDisplay>();
                if (d != null)
                    d.SetVisible(false);
            }

            SetUIVisible(false);
            StartCoroutine(StartEnemyTurnAfterDelay(enemy, 1f));
        }
    }

    private void onEndTurnClicked(ClickEvent evt)

    {

        if (!(CurrentUnit is PlayerClass currentPlayer))
            return;



        switch (selectedAction)
        {
            case TurnAction.Attack:
                StartCoroutine(AttackRoutine(currentPlayer));
                return;

            /*
             * 
            //ANIMATION
                            PlayerMovement playerMovement = currentPlayer.GetComponent<PlayerMovement>();

                            if (playerMovement != null)
                            {
                                playerMovement.PlayAttackAnimation();
                            }






                            ////////////////////
                            Debug.Log("Player attacks");
                            // enemies[0].TakeDamage(67); // pick a real target later

                            if (selectedEnemyTarget != null && selectedEnemyTarget.hp > 0)
                            {
                                float distance = Vector3.Distance(selectedEnemyTarget.transform.position, currentPlayer.transform.position);

                                // range check incase player moves AFTER targetting
                                if (distance <= currentPlayer.range + rangeBuffer)
                                {
                                    Debug.Log(currentPlayer.UnitName + " attacks " + selectedEnemyTarget.UnitName);

                                    // new damage system
                                    DamageInfo info = new DamageInfo
                                    {
                                        category = currentPlayer.basicAttackCategory,
                                        subtype = currentPlayer.basicAttackSubtype,
                                        hitCount = currentPlayer.hitCount

                                    };
                                    float dmg = DamageCalculator.CalculateDamage(currentPlayer, selectedEnemyTarget, info);
                                    selectedEnemyTarget.TakeDamage(dmg);

                                    if (battleLogger != null)
                                        battleLogger.AddEntry($"{currentPlayer.UnitName} attacked {selectedEnemyTarget.UnitName} for {dmg} damage!");

                                    break;
                                }
                                else
                                {
                                    Debug.Log("Attack failed");
                                    if (battleLogger != null) battleLogger.AddEntry("Attack failed: Out of range.");
                                }
                            }
                            else if (selectedEnemyTarget == null || selectedEnemyTarget.hp <= 0)
                            {
                                Debug.Log("No valid target selected");
                                if (battleLogger != null) battleLogger.AddEntry("Attack failed: No valid target.");
                            }


            break;
            */



            case TurnAction.Skill:
                if (selectedSkill == null)
                {
                    Debug.Log("No skill selected");
                    return;
                }

                UnitClass skillTarget = null;
                if (selectedEnemyTarget != null)
                    skillTarget = selectedEnemyTarget;
                else if (selectedPlayerTarget != null)
                    skillTarget = selectedPlayerTarget;

                if (skillTarget == null)
                {
                    Debug.Log("No target selected");
                    return;
                }

                float skillDistance = Vector3.Distance(skillTarget.transform.position, currentPlayer.transform.position);
                if (skillDistance <= selectedSkill.range + rangeBuffer)
                {
                    // Enzo changes: I save the target HP before the skill so the follow-up system can check if damage happened
                    int targetHpBefore = skillTarget.hp;

                    selectedSkill.Use(CurrentUnit, skillTarget);

                    // Log the skill usage
                    if (battleLogger != null)
                        battleLogger.AddEntry($"{CurrentUnit.UnitName} used {selectedSkill.SkillName} on {skillTarget.UnitName}.");

                    SkillState usedState =
                    currentPlayer.skillStates.Find(s => s.skill == selectedSkill);

                    if (usedState != null)
                    {
                        usedState.MarkUsed(TurnNumber);
                    }

                    Debug.Log(CurrentUnit.UnitName + " used " + selectedSkill.SkillName + " on " + skillTarget.UnitName);

                    // Enzo changes: report the skill after it happens so special turn scripts can react
                    if (SpecialTurnRunner.Instance == null)
                    {
                        Debug.Log("FOLLOW UP DEBUG: SpecialTurnRunner.Instance is null");
                    }
                    else
                    {
                        Debug.Log("FOLLOW UP DEBUG: reporting skill use");

                        SpecialTurnRunner.Instance.ReportSkillUse(
                            CurrentUnit,
                            skillTarget,
                            selectedSkill,
                            targetHpBefore
                        );
                    }
                }
                else
                {
                    Debug.Log("Skill out of range");
                    if (battleLogger != null) battleLogger.AddEntry("Skill failed: Out of range.");
                }
                break;

            case TurnAction.Move:
                Debug.Log("Player moves");
                if (battleLogger != null) battleLogger.AddEntry($"{currentPlayer.UnitName} moved position.");
                break;

            case TurnAction.None:
                Debug.Log("No action selected");
                break;
        }
        EndTurn();
        // selectedAction = TurnAction.None;
    }


    //ACCODMODATING ANIMATION
    private IEnumerator AttackRoutine(PlayerClass currentPlayer)
    {
        // Play the attack animation
        PlayerMovement playerMovement = currentPlayer.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            currentPlayer.PlayAttackAnimation();
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(0.8f);

        Debug.Log("Player attacks");

        if (selectedEnemyTarget != null && selectedEnemyTarget.hp > 0)
        {
            float distance = Vector3.Distance(
                selectedEnemyTarget.transform.position,
                currentPlayer.transform.position);

            // Range check in case player moved after selecting target
            if (distance <= currentPlayer.range + rangeBuffer)
            {
                Debug.Log(currentPlayer.UnitName + " attacks " + selectedEnemyTarget.UnitName);

                DamageInfo info = new DamageInfo
                {
                    category = currentPlayer.basicAttackCategory,
                    subtype = currentPlayer.basicAttackSubtype,
                    hitCount = currentPlayer.hitCount
                };

                float dmg = DamageCalculator.CalculateDamage(
                    currentPlayer,
                    selectedEnemyTarget,
                    info);

                selectedEnemyTarget.TakeDamage(dmg);

                if (battleLogger != null)
                {
                    battleLogger.AddEntry(
                        $"{currentPlayer.UnitName} attacked {selectedEnemyTarget.UnitName} for {dmg} damage!");
                }
            }
            else
            {
                Debug.Log("Attack failed");

                if (battleLogger != null)
                    battleLogger.AddEntry("Attack failed: Out of range.");
            }
        }
        else
        {
            Debug.Log("No valid target selected");

            if (battleLogger != null)
                battleLogger.AddEntry("Attack failed: No valid target.");
        }

        EndTurn();
    }

    private void NextTurn()
    {
        if (isGameOver) return; // extra safety net
        if (turnOrder.Count == 0)
            return;
        //chat gpt idea of safety check
        int safety = 0;
        bool wrapped = false;

        do
        {
            currentTurnIndex++;
            safety++;
            if (currentTurnIndex >= turnOrder.Count)
            {
                currentTurnIndex = 0;
                wrapped = true;
            }
            if (safety > 100)
            {
                Debug.LogError("Infinite loop prevented in NextTurn()");
                return;
            }

        } while (turnOrder[currentTurnIndex] == null ||
                 turnOrder[currentTurnIndex].hp <= 0);

        isNewRound = wrapped;

        if (wrapped)
        {
            TurnNumber++;
            Debug.Log("===== TURN " + TurnNumber + " =====");
        }

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

    private void EnemySkill(EnemyClass enemy)
    {

    }

    private IEnumerator EnemyTakeTurn(EnemyClass enemy) 
    {
        if (isGameOver)
        {
            yield break;
        }

        if (battleLogger != null) battleLogger.AddEntry(enemy.UnitName + " moves.");

        Debug.Log(enemy.UnitName + " moves. players.Count = " + players.Count);
        players.RemoveAll(p => p == null || p.hp <= 0);

        // find this enemy's pre-calculated intent
        EnemyIntent intent = enemyIntents.Find(i => i.enemy == enemy);

        // if no intent found, skip
        if (intent.enemy == null)
        {
            EndTurn();
            yield break;
        }

        // move to the pre-calculated destination regardless of where players moved
        yield return StartCoroutine(MoveEnemy(enemy, intent.destination));
        Debug.Log(enemy.UnitName + " moves to  position");
        float distance = Vector3.Distance(enemy.transform.position, intent.targetPlayer.transform.position);

        // attack the pre-calculated target if it's still alive
        if (intent.willSkill && intent.chosenSkill != null)
        {
            if (intent.targetPlayer != null && intent.targetPlayer.hp > 0 && distance <= intent.chosenSkill.range + rangeBuffer)
            {
                Debug.Log(enemy.UnitName + " used skill against " + intent.targetPlayer.UnitName);

                // Log the enemy skill usage
                if (battleLogger != null)
                    battleLogger.AddEntry($"{enemy.UnitName} used {intent.chosenSkill.SkillName} on {intent.targetPlayer.UnitName}.");

                intent.chosenSkill.Use(enemy, intent.targetPlayer);

                SkillState usedState =
                enemy.skillStates.Find(s => s.skill == intent.chosenSkill);

                if (usedState != null)
                {
                    usedState.MarkUsed(TurnNumber);
                }
            }
        }
        else if (intent.willAttack)
        {
            if (intent.targetPlayer != null && intent.targetPlayer.hp > 0 && distance <= enemy.range + rangeBuffer)
            {
                Debug.Log(enemy.UnitName + " attacks " + intent.targetPlayer.UnitName);
                DamageInfo info = new DamageInfo
                {
                    category = enemy.basicAttackCategory,
                    subtype = enemy.basicAttackSubtype,
                    hitCount = enemy.hitCount
                };
                enemy.PlayAttackAnimation();

                yield return new WaitForSeconds(0.7f);
                float dmg = DamageCalculator.CalculateDamage(enemy, intent.targetPlayer, info);
                intent.targetPlayer.TakeDamage(dmg);

                if (battleLogger != null)
                    battleLogger.AddEntry($"{enemy.UnitName} attacked {intent.targetPlayer.UnitName} for {dmg} damage.");
            }
            else
            {
                if (intent.targetPlayer == null || intent.targetPlayer.hp <= 0)
                {
                    Debug.Log(enemy.UnitName + " target is dead/invalid, attack cancelled");
                    if (battleLogger != null) battleLogger.AddEntry(enemy.UnitName + " cannot attack: target is invalid.");
                }
                else
                {
                    Debug.Log(enemy.UnitName + " target is out of range, attack cancelled");
                    if (battleLogger != null) battleLogger.AddEntry(enemy.UnitName + " cannot attack: target out of range.");
                }
            }
        }

        WinLoseState();

        if (isGameOver)
        {
            Debug.Log("Game over — halting turn loop");
            yield break;
        }

        StartCoroutine(EndTurnAfterDelay(0.5f));
        // EndTurn();
    }
    private IEnumerator MoveEnemy(EnemyClass enemy, Vector3 destination)
    {
        Animator animator = enemy.GetComponent<Animator>();

        if (animator != null)
            animator.SetBool("IsRunning", true);

        while (Vector3.Distance(enemy.transform.position, destination) > 0.05f)
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                destination,
                5f * Time.deltaTime); // Movement speed

            yield return null;
        }

        enemy.transform.position = destination;

        if (animator != null)
            animator.SetBool("IsRunning", false);
    }


    private void EndTurn()
    {
        selectedSkill = null;
        HideSkillPanel();

        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.HideAttackRange();
            pm.HideSkillRange();
        }
        selectedAction = TurnAction.None;

        if (selectedEnemyTarget != null)
        {
            TargetAndHighlight targetable = selectedEnemyTarget.GetComponent<TargetAndHighlight>();
            if (targetable != null)
                targetable.SetHighlighted(false);
        }
        selectedEnemyTarget = null;

        if (selectedPlayerTarget != null)
        {
            TargetAndHighlight targetable = selectedPlayerTarget.GetComponent<TargetAndHighlight>();
            if (targetable != null)
                targetable.SetHighlighted(false);
        }
        selectedPlayerTarget = null;


        isTargeting = false;
        UpdateTurnOrderUI();
        SetUIVisible(false);
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
            Vector3 spawnPos = PlayerSpawnPoints[i].position + Vector3.up * playerSpawnYOffset;

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

        if (battleLogger != null)
            battleLogger.AddEntry($"{CurrentUnit.UnitName} moves.");

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
        enemies.RemoveAll(e => e == null || e.hp <= 0);
        players.RemoveAll(p => p == null || p.hp <= 0);

        bool anyPlayerAlive = players.Count > 0;
        bool anyEnemyAlive = enemies.Count > 0;

        if (!anyEnemyAlive && !isGameOver)
        {
            Debug.Log("All enemies defeated — Loading Win Scene");
            isGameOver = true;
            SceneManager.LoadScene("WinScene");
        }
        else if (!anyPlayerAlive && !isGameOver)
        {
            Debug.Log("All players defeated — Loading Lose Scene");
            isGameOver = true;
            SceneManager.LoadScene("LoseScene");
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
    /*
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
    */

    public UnitClass GetCurrentUnit()
    {
        return CurrentUnit;
    }
    public void CancelTargeting()
    {
        isTargeting = false;
        selectedAction = TurnAction.None;
        if (selectedSkill != null)
        {
            ShowSkillMenu((PlayerClass)CurrentUnit);
        }
        SetUIVisible(true); // add this
        if (CurrentUnit is PlayerClass currentPlayer)
        {
            PlayerMovement pm = currentPlayer.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.HideAttackRange();
            pm.HideSkillRange();
        }

        Debug.Log("Targeting cancelled — out of range");
    }

    private void UpdateTurnOrderUI()
    {
        VisualElement container = doc.rootVisualElement.Q<VisualElement>("TurnOrderContainer");
        if (container == null) return;

        container.Clear();

        foreach (UnitClass unit in turnOrder)
        {
            if (unit == null || unit.hp <= 0) continue;

            Image icon = new Image();
            icon.sprite = unit.unitIcon;

            icon.style.width = 60;
            icon.style.height = 60;
            icon.style.marginRight = 5;
            icon.style.marginLeft = 5;

            if (unit == CurrentUnit)
            {
                icon.style.borderBottomColor = Color.yellow;
                icon.style.borderBottomWidth = 4;
            }

            container.Add(icon);
        }
    }

    private void CalculateEnemyIntents()
    {
        enemyIntents.Clear();

        foreach (EnemyClass enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0) continue;

            EnemyIntentBehavior behavior = GetIntentBehavior(enemy.enemyType);
            EnemyIntent intent = behavior.CalculateIntent(enemy, players, TurnNumber, rangeBuffer);

            if (intent.targetPlayer == null) continue; // no living target found

            enemyIntents.Add(intent);

            enemy.ShowIntent(intent.willAttack, intent.willSkill, intent.targetPlayer);
            EnemyIntentDisplay display = enemy.GetComponent<EnemyIntentDisplay>();
            if (display != null)
            {
                display.UpdateIntent(intent.destination, enemy.range);
                display.UpdateTargetLight(intent.targetPlayer.transform);
            }
        }
    }

    private EnemyIntentBehavior GetIntentBehavior(enemyType type)
    {
        switch (type)
        {
            case enemyType.Melee1:
                return new Melee1();
            case enemyType.Ranged1:
                return new Ranged1();
            case enemyType.Magician1:
            case enemyType.Boss1:
            default:
                Debug.LogWarning(type + " has no dedicated EnemyIntentBehavior yet, using Melee1 as a fallback.");
                return new Melee1();
        }
    }

    // for showing a unit's stats when clicked
    public void ShowUnitStats(UnitClass unit)
    {
        VisualElement statsPanel = doc.rootVisualElement.Q<VisualElement>("StatsPanel");
        if (statsPanel == null) return;

        statsPanel.style.display = DisplayStyle.Flex;

        statsPanel.Q<Label>("StatsName").text = unit.UnitName;
        statsPanel.Q<Label>("StatsHP").text = "HP: " + unit.hp + "/" + unit.maxHp;
        statsPanel.Q<Label>("StatsATK").text = "ATK: " + unit.atk;
        statsPanel.Q<Label>("StatsRange").text = "Range: " + unit.range;
        statsPanel.Q<Label>("StatsSpeed").text = "Speed: " + unit.speed;
        statsPanel.Q<Label>("StatsMovement").text = "Movement: " + unit.movement;
    }

    public void HideUnitStats()
    {
        VisualElement statsPanel = doc.rootVisualElement.Q<VisualElement>("StatsPanel");
        if (statsPanel != null)
            statsPanel.style.display = DisplayStyle.None;
    }

    private IEnumerator StartEnemyTurnAfterDelay(EnemyClass enemy, float delay)
    {  // delay function
        yield return new WaitForSeconds(delay);
        StartCoroutine(EnemyTakeTurn(enemy)); ;
    }
    private IEnumerator EndTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndTurn();
    }
}