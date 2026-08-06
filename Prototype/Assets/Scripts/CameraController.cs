using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera birdsEyeCamera;

    [Header("Free Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lookSpeed = 2f;

    [SerializeField] private InputActionAsset inputAsset;
    private InputAction freeCamToggle;
    private InputAction moveAction;
    private InputAction verticalAction;

    private bool isFreeCam = false; // false = locked in place, true = can move/look
    private float yaw = 0f;
    private float pitch = 0f;

    void Awake()
    {
        freeCamToggle = inputAsset.FindAction("Player/Camera");
        moveAction = inputAsset.FindAction("Player/Move");
        verticalAction = inputAsset.FindAction("Player/Vertical");
    }

    void OnEnable()
    {
        freeCamToggle.Enable();
        freeCamToggle.performed += OnFreeCamToggle;
        moveAction.Enable();
        verticalAction.Enable();
    }

    void OnDisable()
    {
        freeCamToggle.Disable();
        freeCamToggle.performed -= OnFreeCamToggle;
        moveAction.Disable();
        verticalAction.Disable();
    }

    void Start()
    {
        // initialize yaw/pitch to match the camera's starting rotation
        yaw = birdsEyeCamera.transform.eulerAngles.y;
        pitch = birdsEyeCamera.transform.eulerAngles.x;
    }

    void Update()
    {
        if (!isFreeCam) return; // locked, don't move or look
        if (PlayerMovement.AnyPlayerMoving) return; // don't allow camera movement while a player is moving

        HandleFreeCamMovement();
        HandleFreeCamLook();
    }

    private void OnFreeCamToggle(InputAction.CallbackContext ctx)
    {
        if (PlayerMovement.AnyPlayerMoving) return; // don't allow toggling while a player is moving

        isFreeCam = !isFreeCam;

        // lock/unlock cursor based on new state
        if (isFreeCam)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Debug.Log("FreeCam: " + isFreeCam);
    }

    private void HandleFreeCamMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector2 vertical = verticalAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.x, vertical.y, input.y);
        move = birdsEyeCamera.transform.TransformDirection(move);

        birdsEyeCamera.transform.position += move * moveSpeed * Time.deltaTime;
    }

    private void HandleFreeCamLook()
    {
        if (!Mouse.current.rightButton.isPressed) return;

        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue();

        yaw += mouseX * lookSpeed;
        pitch -= mouseY * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        birdsEyeCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}