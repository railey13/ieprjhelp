using UnityEngine;
using UnityEngine.InputSystem;

public class TargetAndHighlight : MonoBehaviour
{
    private UnitClass unit;
    private TurnBasedSystemV2 turnSystem;
    [SerializeField] private Light targetLight;

    [Header("Hover Highlight")]
    [SerializeField] private GameObject[] highlightObjects; // objects to show/hide on hover
    private bool isHovered;

    [Header("Target Circle")]
    [SerializeField] private float circleRadius = 0.8f;
    [SerializeField] private float circleHeight = 0.05f;
    [SerializeField] private Color enemyTargetColor = new Color(1f, 0.2f, 0.2f, 0.4f);   // red for enemies
    [SerializeField] private Color playerTargetColor = new Color(0.2f, 0.6f, 1f, 0.4f);  // blue for players
    [SerializeField] private Color neutralTargetColor = new Color(0.8f, 0.8f, 0.8f, 0.4f); // grey for non-units
    private GameObject circleObj;
    private float rangeBuffer = 0.5f; // small extra range to account for edge clicks

    // what type of object this is
    private bool isEnemy;
    private bool isPlayer;
    private bool isUnit; // true if enemy or player

    void Awake()
    {
        unit = GetComponent<UnitClass>();
        turnSystem = FindFirstObjectByType<TurnBasedSystemV2>();

        // determine what type this object is
        isEnemy = GetComponent<EnemyClass>() != null;
        isPlayer = GetComponent<PlayerClass>() != null;
        isUnit = isEnemy || isPlayer;

        if (targetLight != null)
            targetLight.enabled = false;

        // only build the target circle for units, not props or terrain
        if (isUnit)
        {
            BuildCircleVisual();
            SetCircleVisible(false);
        }
    }

    void Start()
    {
        SetHighlight(false); // make sure highlight starts off
    }

    void Update()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // check if the mouse is hovering over this object or any of its children
            bool hitThisObject = hit.transform.IsChildOf(transform);
            if (hitThisObject != isHovered)
            {
                isHovered = hitThisObject;
                SetHighlight(isHovered);
            }

            // handle left click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // check if the clicked object has a TargetAndHighlight component
                TargetAndHighlight foundTAH = hit.collider.GetComponentInParent<TargetAndHighlight>();

                if (foundTAH == this)
                {
                    OnClickedSelf(); // clicked this object
                }
                else if (foundTAH == null)
                {
                    OnClickedEmpty(); // clicked empty space or non-targetable object
                }
                // if foundTAH is something else, another object was clicked, do nothing
            }
        }
        else
        {
            // mouse isn't over anything, remove hover
            if (isHovered)
            {
                isHovered = false;
                SetHighlight(false);
            }
        }
    }

    // called when this object is clicked
    private void OnClickedSelf()
    {
        if (isUnit && turnSystem.GetisTargetting())
        {
            // in targeting mode, try to select this as a target
            UnitClass attacker = turnSystem.GetCurrentUnit();
            if (attacker != null)
            {
                float dist = Vector3.Distance(attacker.transform.position, transform.position);
                if (dist <= attacker.range + rangeBuffer)
                {
                    // in range, select as target
                    if (isEnemy)
                        turnSystem.SelectEnemyTarget(GetComponent<EnemyClass>());
                    else if (isPlayer)
                        turnSystem.SelectPlayerTarget(GetComponent<PlayerClass>());
                }
                else
                {
                    // out of range, cancel targeting
                    turnSystem.CancelTargeting();
                }
            }
        }
        else
        {
            // not in targeting mode, show stats and intent info
            if (isUnit && unit != null)
                turnSystem.ShowUnitStats(unit);

            // show enemy intent display when clicked outside of targeting
            if (isEnemy)
            {
                EnemyIntentDisplay display = GetComponent<EnemyIntentDisplay>();
                if (display != null)
                    display.SetVisible(!display.IsVisible()); // toggle on/off
            }
        }
    }

    // called when empty space is clicked
    private void OnClickedEmpty()
    {
        turnSystem.HideUnitStats();

        // hide enemy intent display when clicking away
        if (isEnemy)
        {
            EnemyIntentDisplay display = GetComponent<EnemyIntentDisplay>();
            if (display != null)
                display.SetVisible(false);
        }
    }

    // called by TurnBasedSystemV2 to highlight this unit when selected as a target
    public void SetHighlighted(bool isHighlighted)
    {
        if (targetLight != null)
            targetLight.enabled = isHighlighted;

        if (isUnit)
            SetCircleVisible(isHighlighted); // show/hide target circle
    }

    // shows or hides the hover highlight objects
    public void SetHighlight(bool state)
    {
        if (highlightObjects == null) return;
        foreach (GameObject obj in highlightObjects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }

    // builds the flat cylinder circle that appears under the unit when targeted
    void BuildCircleVisual()
    {
        circleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circleObj.name = "TargetCircle_" + gameObject.name;

        DestroyImmediate(circleObj.GetComponent<Collider>()); // remove collider so it doesnt interfere with raycasts

        circleObj.transform.localScale = new Vector3(circleRadius * 2f, 0.02f, circleRadius * 2f); // flat disc shape

        // pick color based on unit type
        var mat = new Material(Shader.Find("Sprites/Default"));
        if (isEnemy)
            mat.color = enemyTargetColor;
        else if (isPlayer)
            mat.color = playerTargetColor;
        else
            mat.color = neutralTargetColor;

        circleObj.GetComponent<MeshRenderer>().material = mat;

        // attach to this object so it follows it
        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = new Vector3(0f, circleHeight, 0f);
    }

    // shows or hides the target circle
    void SetCircleVisible(bool visible)
    {
        if (circleObj != null)
            circleObj.SetActive(visible);
    }
}