using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyTargetable : MonoBehaviour
{
    private EnemyClass enemy;
    private TurnBasedSystemV2 turnSystem;

    void Awake()
    {
        enemy = GetComponent<EnemyClass>();
        turnSystem = FindFirstObjectByType<TurnBasedSystemV2>();
        Debug.Log("EnemyTargetable attached to: " + gameObject.name);
    }

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                turnSystem.SelectTarget(enemy);
            }
        }
    }
}