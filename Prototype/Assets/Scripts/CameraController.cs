using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera birdsEyeCamera;
    [SerializeField] private Camera freeCam;

    [Header("Free Cam Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lookSpeed = 2f;

    [SerializeField] private InputActionAsset inputAsset;
    private InputAction freeCamToggle;
    private InputAction moveAction;
    private InputAction verticalAction;

    private bool isFreeCam = false;
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
        // start in birds eye
        birdsEyeCamera.gameObject.SetActive(true);
        freeCam.gameObject.SetActive(false);

        // initialize free cam rotation to match its starting rotation
        yaw = freeCam.transform.eulerAngles.y;
        pitch = freeCam.transform.eulerAngles.x;
    }

    void Update()
    {
        if (!isFreeCam) return;
        HandleFreeCamMovement();
        HandleFreeCamLook();
    }

    private void OnFreeCamToggle(InputAction.CallbackContext ctx)
    {
        isFreeCam = !isFreeCam;

        birdsEyeCamera.gameObject.SetActive(!isFreeCam);
        freeCam.gameObject.SetActive(isFreeCam);

        // lock/unlock cursor
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("Right click held down");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Debug.Log("Right click released");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (!Mouse.current.rightButton.isPressed) return;

        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue();

        yaw += mouseX * lookSpeed;
        pitch -= mouseY * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f); // no flipping camera upside down

        freeCam.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        Cursor.visible = !isFreeCam;

        Debug.Log("FreeCam: " + isFreeCam);
    }

    private void HandleFreeCamMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector2 vertical = verticalAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.x, vertical.y, input.y);

        // move relative to camera facing direction
        move = freeCam.transform.TransformDirection(move);

        freeCam.transform.position += move * moveSpeed * Time.deltaTime;
    }

    private void HandleFreeCamLook()
    {
        // only rotate when right mouse is held so accidental mouselook doesnt happen
        if (!Mouse.current.rightButton.isPressed) return;

        float mouseX = Mouse.current.delta.x.ReadValue();
        float mouseY = Mouse.current.delta.y.ReadValue();

        yaw += mouseX * lookSpeed;
        pitch -= mouseY * lookSpeed;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        freeCam.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}