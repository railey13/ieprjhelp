using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerClass))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Range Indicator")]
    [SerializeField] private float circleHeight = 0.05f; //how high above the ground the circle is
    [SerializeField] private Color rangeColor = new Color(0f, 0.6f, 1f, 0.25f); // circle color

    [SerializeField] private InputActionAsset inputAsset;
    private PlayerClass player;
    private float range;
    private Vector3 origin; // circle anchor
    private bool moving; // true = circle active, player can walk
    private InputAction moveAction;
    private InputAction spaceAction;
    private GameObject circleObj;

    void Awake() 
    { 
        player = GetComponent<PlayerClass>(); 
        moveAction = inputAsset.FindAction("Player/Move");
        spaceAction = inputAsset.FindAction("Player/Space");
    }

    void Start()
    {
        range = player.movement;
        BuildCircleVisual();
        SetCircleVisible(false);
    }

        void OnEnable()
    {
        moveAction.Enable();
        spaceAction.Enable();
        spaceAction.performed += OnConfirmPerformed;
    }

    void OnDisable()
    {
        moveAction.Disable();
        spaceAction.Disable();
        spaceAction.performed -= OnConfirmPerformed;
    }

    void Update()
    {
        if (!moving) return; // cant move if not allowed to
        HandleWASD();
    }


    void OpenMovement()
    {
        origin = transform.position;

        // when space, place circle on player
        circleObj.transform.position = new Vector3( 
            origin.x, origin.y + circleHeight, origin.z);

        SetCircleVisible(true); // show circle during movement 
        moving = true;
    }

    void ConfirmMovement()
    {
        moving = false; // cant move
        SetCircleVisible(false); // hide circle 
    }

    void HandleWASD()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input == Vector2.zero) return;
        // movement
        Vector3 proposed = transform.position + new Vector3(input.x, 0f, input.y).normalized * moveSpeed * Time.deltaTime; 

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
    void OnConfirmPerformed(InputAction.CallbackContext ctx)
    {
        if (!moving) 
            OpenMovement();
        else 
            ConfirmMovement();
    }

}

