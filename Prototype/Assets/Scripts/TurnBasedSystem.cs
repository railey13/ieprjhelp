using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TurnBasedSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] PlayerPrefab;
    [SerializeField] private GameObject[] EnemyPrefab;

    [SerializeField] private Transform PlayerPosition;
    [SerializeField] private Transform EnemyPosition;

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

    private List<PlayerClass> players= new List<PlayerClass>();
    private List<EnemyClass> enemies = new List<EnemyClass>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BattleStart();
    }

    public void SetUIVisible(bool visible)
    {
        doc.rootVisualElement.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
    private void BattleStart()
    {

        Debug.Log("Spawning Player");
        /*
       // GameObject player = Instantiate(PlayerPrefab[0], PlayerPosition.position, Quaternion.identity);
       for (int i =0; i< PlayerPrefab.Length; i++)
        {
            GameObject playerObject = Instantiate(
                PlayerPrefab[i],
                PlayerPosition.position + new Vector3(i * 2, 0, 0),
                Quaternion.identity
                );
            players.Add(playerObject.GetComponent<PlayerClass>());
        }

        Debug.Log("Spawning Enemy");
        for (int i = 0; i < EnemyPrefab.Length; i++)
        {
            GameObject enemyObject = Instantiate(
                EnemyPrefab[i],
                EnemyPosition.position + new Vector3(i * 2, 0, 0),
                Quaternion.identity
            );

            enemies.Add(enemyObject.GetComponent<EnemyClass>());
        }
        */

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

    }
    private void onEndTurnClicked(ClickEvent evt)
    {
        /*if (isAttack == true)
        {
            Debug.Log("Attack");
            //end turn
        }
        else if (isHeal == true)
        {
            Debug.Log("Healed");
        }
        else if (isMove == true)
        {
            Debug.Log("Move");
            SceneManager.LoadScene("Prototype");
        }
        else 
        {
            Debug.Log("Not selected any action"); 
        }
        */

        switch (selectedAction)
        {
            case TurnAction.Attack:
                Debug.Log("Player attacks");
                enemies[0].TakeDamage(67); // pick a real target later
                break;

            case TurnAction.Heal:
                Debug.Log("Player heals");
                break;

            case TurnAction.Move:
                Debug.Log("Player moves");
                // move the player's transform here, don't load a new scene
                break;

            case TurnAction.None:
                Debug.Log("No action selected");
                return; // don't end the turn if nothing was chosen
        }

        selectedAction = TurnAction.None; // reset for next turn
        WinLoseState();
        AdvanceTurn();
    }
    private void AdvanceTurn()
    {
        Debug.Log("Advancing to Enemy Turn");
        currentPhase = TurnPhase.EnemyTurn;
        SetUIVisible(false);

        EnemyMove();

        WinLoseState();
        if (isGameOver) return;

        Debug.Log("Advancing to Player Turn");
        currentPhase = TurnPhase.PlayerTurn;
        PlayerMovement activePlayerMovement = players[0].GetComponent<PlayerMovement>();
        activePlayerMovement.setResetOrigin(true); // allow player movement to reset origin on next turn
        SetUIVisible(true);
    }

    public void OnPlayerMoveConfirmed()
    {
        Debug.Log("Player finished moving");
        selectedAction = TurnAction.None;
        WinLoseState();
        if (isGameOver) return;
        SetUIVisible(true);
        //AdvanceTurn();
    }
    public void OnPlayerMoveCancelled()
    {
        Debug.Log("Player cancelled movement");
        selectedAction = TurnAction.None;
        SetUIVisible(true);
    }
    public void EnemyMove()
    {

    }
    public void ActionState()
    {

    }
    public void WinLoseState()
    {
        bool anyPlayerAlive = true; // add logic
        bool anyEnemyAlive = true; // add logic
 
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
/*    private void AttackEnabled(ClickEvent evt)
    {
        Debug.Log("Attack Clicked");
        isAttack = true;
    }
    private void MoveEnabled(ClickEvent evt)
    {
        Debug.Log("Move Clicked");
        isMove = true;
    }
    private void HealEnabled(ClickEvent evt)
    {
        Debug.Log("Heal Clicked");
        isHeal = true;
    }
    */

    private void AttackEnabled(ClickEvent evt)
    {
        Debug.Log("Attack Clicked");
        selectedAction = TurnAction.Attack;
    }

    private void MoveEnabled(ClickEvent evt)
    {
        Debug.Log("Move Clicked");
        selectedAction = TurnAction.Move;

        PlayerMovement activePlayerMovement = players[0].GetComponent<PlayerMovement>();
        Debug.Log("activePlayerMovement is null? " + (activePlayerMovement == null));
        if (activePlayerMovement != null)
        {
            activePlayerMovement.ActivateMovement();
            SetUIVisible(false);
        }
    }
    private void HealEnabled(ClickEvent evt)
    {
        Debug.Log("Heal Clicked");
        selectedAction = TurnAction.Heal;
    }
    
}
