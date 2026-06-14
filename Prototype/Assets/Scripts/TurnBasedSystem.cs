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
    private bool isMove = false;
    private bool isAttack = false;
    private bool isHeal = false;


    private List<PlayerClass> players= new List<PlayerClass>();
    private List<EnemyClass> enemies = new List<EnemyClass>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BattleStart();
    }


    private void BattleStart()
    {

        Debug.Log("Spawning Player");
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

        PMove = doc.rootVisualElement.Q<Button>("Move");
        PAttack = doc.rootVisualElement.Q<Button>("Attack");
        PHeal = doc.rootVisualElement.Q<Button>("Heal");
        EndTurn = doc.rootVisualElement.Q<Button>("EndTurn");
        PMove.RegisterCallback<ClickEvent>(MoveEnabled);
        PAttack.RegisterCallback<ClickEvent>(AttackEnabled);
        PHeal.RegisterCallback<ClickEvent>(HealEnabled);
        EndTurn.RegisterCallback<ClickEvent>(onEndTurnClicked);



    }
    private void onEndTurnClicked(ClickEvent evt)
    {
        if (isAttack == true)
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
    private void AttackEnabled(ClickEvent evt)
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
    
    
}
