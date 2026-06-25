using UnityEngine;
using UnityEngine.InputSystem;

public class UnitTargetable : MonoBehaviour
{
    private UnitClass unit;
    private TurnBasedSystemV2 turnSystem;
    [SerializeField] private Light targetLight;

    [Header("Target Circle")]
    [SerializeField] private float circleRadius = 0.8f;
    [SerializeField] private float circleHeight = 0.05f;
    [SerializeField] private Color enemyTargetColor = new Color(1f, 0.2f, 0.2f, 0.4f);
    [SerializeField] private Color playerTargetColor = new Color(0.2f, 0.6f, 1f, 0.4f);
    private GameObject circleObj;
    private float rangeBuffer = 0.5f;

    private bool isEnemy;

    void Awake()
    {
        unit = GetComponent<UnitClass>();
        turnSystem = FindFirstObjectByType<TurnBasedSystemV2>();
        isEnemy = GetComponent<EnemyClass>() != null;

        Debug.Log("UnitTargetable attached to: " + gameObject.name + " | isEnemy: " + isEnemy);

        if (targetLight != null)
            targetLight.enabled = false;

        BuildCircleVisual();
        SetCircleVisible(false);
    }

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            UnitTargetable found = hit.collider.GetComponentInParent<UnitTargetable>();

            if (found == this)
            {
                if (turnSystem.GetisTargetting())
                {
                    UnitClass attacker = turnSystem.GetCurrentUnit();
                    if (attacker != null)
                    {
                        float dist = Vector3.Distance(attacker.transform.position, transform.position);
                        if (dist <= attacker.range + rangeBuffer)
                        {
                            if (isEnemy)
                                turnSystem.SelectEnemyTarget(GetComponent<EnemyClass>());
                            else
                                turnSystem.SelectPlayerTarget(GetComponent<PlayerClass>());
                        }
                        else
                        {
                            turnSystem.CancelTargeting();
                        }
                    }
                }
                else
                {
                    turnSystem.ShowUnitStats(unit);
                    if (isEnemy) // show intent display only for enemies
                    {
                        EnemyIntentDisplay display = GetComponent<EnemyIntentDisplay>();
                        if (display != null)
                            display.SetVisible(!display.IsVisible());
                    }
                }
            }
            else if (found == null)
            {
                turnSystem.HideUnitStats();
                if (isEnemy)
                {
                    EnemyIntentDisplay display = GetComponent<EnemyIntentDisplay>();
                    if (display != null)
                        display.SetVisible(false);
                }
            }
        }
    }

    public void SetHighlighted(bool isHighlighted)
    {
        if (targetLight != null)
            targetLight.enabled = isHighlighted;
        SetCircleVisible(isHighlighted);
    }

    void BuildCircleVisual()
    {
        circleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circleObj.name = "TargetCircle_" + gameObject.name;

        DestroyImmediate(circleObj.GetComponent<Collider>());

        circleObj.transform.localScale = new Vector3(circleRadius * 2f, 0.02f, circleRadius * 2f);

        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = isEnemy ? enemyTargetColor : playerTargetColor;
        circleObj.GetComponent<MeshRenderer>().material = mat;

        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = new Vector3(0f, circleHeight, 0f);
    }

    void SetCircleVisible(bool visible)
    {
        if (circleObj != null)
            circleObj.SetActive(visible);
    }
}