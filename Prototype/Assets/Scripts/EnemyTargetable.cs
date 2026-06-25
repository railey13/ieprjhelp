using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class EnemyTargetable : MonoBehaviour
{
    private EnemyClass enemy;
    private TurnBasedSystemV2 turnSystem;
    [SerializeField] private Light targetLight;

    [Header("Target Circle")]
    [SerializeField] private float circleRadius = 0.8f;
    [SerializeField] private float circleHeight = 0.05f;
    [SerializeField] private Color targetColor = new Color(1f, 0.2f, 0.2f, 0.4f);
    private GameObject circleObj;
    private float rangeBuffer = 0.5f;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        Debug.Log(gameObject.name + " has collider: " + (col != null));
        enemy = GetComponent<EnemyClass>();
        turnSystem = FindFirstObjectByType<TurnBasedSystemV2>();
        Debug.Log("EnemyTargetable attached to: " + gameObject.name);

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
            EnemyTargetable found = hit.collider.GetComponentInParent<EnemyTargetable>();

            if (found == this)
            {
                if (turnSystem.GetisTargetting())
                {
                    UnitClass attacker = turnSystem.GetCurrentUnit();
                    if (attacker != null)
                    {
                        float dist = Vector3.Distance(attacker.transform.position, transform.position);
                        if (dist <= attacker.range + rangeBuffer)
                            turnSystem.SelectTarget(enemy);
                        else
                            turnSystem.CancelTargeting();
                    }
                }
                else
                {
                    EnemyIntentDisplay display = GetComponent<EnemyIntentDisplay>();
                    if (display != null)
                        display.SetVisible(!display.IsVisible());
                }
            }
            else if (found == null)
            {
                // clicked empty space, hide this enemy's display
                EnemyIntentDisplay display = GetComponent<EnemyIntentDisplay>();
                if (display != null)
                    display.SetVisible(false);
            }
            // if found != null && found != this, another enemy was clicked, do nothing
        }
    }

    public void SetHighlighted(bool isHighlighted)
    {
        Debug.Log("SetHighlighted called: " + isHighlighted);
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
        mat.color = targetColor;
        circleObj.GetComponent<MeshRenderer>().material = mat;

        // attach to this enemy so it follows if they move
        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = new Vector3(0f, circleHeight, 0f);
    }

    void SetCircleVisible(bool visible)
    {
        if (circleObj != null)
            circleObj.SetActive(visible);
    }
}