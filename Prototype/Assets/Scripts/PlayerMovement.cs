using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(PlayerClass))]
public class PlayerMovement : MonoBehaviour
{
    //ANIMATION
    private Animator animator;
    public static bool AnyPlayerMoving { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Movement Indicator")]
    [SerializeField] private float circleHeight = -5.0f; //how high above the ground the circle is
    [SerializeField] private Color rangeColor = new Color(0f, 0.6f, 1f, 0.25f); // circle color
    [Header("Attack Circle")]
    [SerializeField] private Color attackRangeColor = new Color(1f, 0.2f, 0.2f, 0.25f); // attack color
    private GameObject attackCircleObj;
    [Header("Skill Range Indicator")]
    [SerializeField] private Color skillRangeColor = new Color(0.5f, 0f, 1f, 0.25f); // skills color
    private GameObject skillCircleObj;

    [SerializeField] private InputActionAsset inputAsset;
    private PlayerClass player;
    private float range;
    private Vector3 origin; // circle anchor
    private bool moving; // true = circle active, player can walk
    private InputAction moveAction;
    private InputAction cancelAction;
    private InputAction spaceAction;
    private GameObject circleObj;
    private bool resetOrigin = true;
    private TurnBasedSystemV2 turnSystem;


    void Awake()
    {
        //animation
        animator = GetComponent<Animator>();






        player = GetComponent<PlayerClass>();
        moveAction = inputAsset.FindAction("Player/Move");
        cancelAction = inputAsset.FindAction("Player/Cancel");
        spaceAction = inputAsset.FindAction("Player/Space");

        if (moveAction == null) Debug.LogError("moveAction not found!");
        if (cancelAction == null) Debug.LogError("cancelAction not found!");
        if (spaceAction == null) Debug.LogError("spaceAction not found!");
    }

    void Start()
    {



        Debug.Log(gameObject.name + " PlayerMovement Start() running");
        range = player.movement;
        Debug.Log(gameObject.name + " range = " + range);
        BuildCircleVisual();
        BuildAttackCircleVisual();
        Debug.Log(gameObject.name + " circleObj built? " + (circleObj != null));
        SetCircleVisible(false);
    }

    void OnEnable()
    {
        moveAction.Enable();
        spaceAction.Enable();
        cancelAction.Enable();
        spaceAction.performed += OnConfirmSpacePerformed;
        cancelAction.performed += OnCancelPerformed;
    }

    void OnDisable()
    {
        moveAction.Disable();
        spaceAction.Disable();
        spaceAction.performed -= OnConfirmSpacePerformed;
        cancelAction.Disable();
        cancelAction.performed -= OnCancelPerformed;
    }

    void Update()
    {
        if (!moving) return; // cant move if not allowed to
        HandleWASD();

    }

    public void ActivateMovement()
    {
        OpenMovement();
    }
    void OpenMovement()
    {
        if (resetOrigin)
        {
            origin = transform.position;
            resetOrigin = false;
        }
        // when space, place circle on player
        circleObj.transform.position = new Vector3(
            origin.x, origin.y + circleHeight, origin.z);

        SetCircleVisible(true); // show circle during movement 
        moving = true;
        AnyPlayerMoving = true;
    }

    void ConfirmMovement()
    {
        moving = false;
        SetCircleVisible(false);
        AnyPlayerMoving = false;

        if (turnSystem != null)
            turnSystem.OnPlayerMoveConfirmed();
    }

    void HandleWASD()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        //ANIMATION

        animator.SetBool("IsRunning", input != Vector2.zero);

        if (input == Vector2.zero)
            return;
        ////

        // get camera-relative flat directions
        Transform camT = Camera.main.transform;

        Vector3 camForward = camT.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = camT.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;
        Vector3 proposed = transform.position + moveDir * moveSpeed * Time.deltaTime;

        Vector3 offset = proposed - origin;
        offset.y = 0f;
        if (offset.magnitude > range)
            proposed = origin + offset.normalized * range;

        proposed.y = transform.position.y;
        transform.position = proposed;
    }

    void BuildCircleVisual()
    {
        // cylinder object
        circleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circleObj.name = "MovementRangeCircle";

        // remove collider
        Destroy(circleObj.GetComponent<Collider>());

        // X and Z scale with range, y is thin
        circleObj.transform.localScale = new Vector3(range * 2f, 0.02f, range * 2f);

        // apply a transparent material
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = rangeColor;
        circleObj.GetComponent<MeshRenderer>().material = mat;
    }

    void SetCircleVisible(bool visible)
    {
        circleObj.SetActive(visible); // makes the circle appear/disappear
    }

    // when space is pressed, either open movement or confirm movement
    void OnConfirmSpacePerformed(InputAction.CallbackContext ctx)
    {
        if (moving)
            ConfirmMovement();
    }

    // when cancel is pressed, cancel movement
    void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        if (moving)
        {
            moving = false;
            SetCircleVisible(false);
            AnyPlayerMoving = false;
            transform.position = origin; // reset position to original

            if (turnSystem != null)
                turnSystem.OnPlayerMoveCancelled();
        }
    }

    public void setResetOrigin(bool value)
    {
        resetOrigin = value;
    }

    void BuildAttackCircleVisual()
    {
        attackCircleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        attackCircleObj.name = "AttackRangeCircle";

        DestroyImmediate(attackCircleObj.GetComponent<Collider>());

        float attackRange = player.range;
        attackCircleObj.transform.localScale = new Vector3(attackRange * 2f, 0.02f, attackRange * 2f);

        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = attackRangeColor;
        attackCircleObj.GetComponent<MeshRenderer>().material = mat;

        attackCircleObj.transform.SetParent(transform);
        attackCircleObj.transform.localPosition = new Vector3(0f, circleHeight, 0f);

        attackCircleObj.SetActive(false);
    }
    public void ShowAttackRange()
    {
        if (attackCircleObj != null)
            attackCircleObj.SetActive(true);
    }

    public void HideAttackRange()
    {
        if (attackCircleObj != null)
            attackCircleObj.SetActive(false);
    }

    void BuildSkillCircleVisual(float skillRange)
    {
        skillCircleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        skillCircleObj.name = "SkillRangeCircle";

        DestroyImmediate(skillCircleObj.GetComponent<Collider>());

        skillCircleObj.transform.localScale = new Vector3(skillRange * 2f, 0.02f, skillRange * 2f);

        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = skillRangeColor;
        skillCircleObj.GetComponent<MeshRenderer>().material = mat;

        skillCircleObj.transform.SetParent(transform);
        skillCircleObj.transform.localPosition = new Vector3(0f, circleHeight, 0f);

        skillCircleObj.SetActive(false);
    }

    public void ShowSkillRange(float skillRange)
    {
        if (skillCircleObj != null)
            Destroy(skillCircleObj);

        BuildSkillCircleVisual(skillRange);
        skillCircleObj.SetActive(true);
    }

    public void HideSkillRange()
    {
        if (skillCircleObj != null)
            skillCircleObj.SetActive(false);
    }

    public void SetTurnBasedSystem(TurnBasedSystemV2 system)
    {
        turnSystem = system;
    }
  
}
