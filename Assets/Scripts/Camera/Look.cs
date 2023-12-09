using UnityEngine;
using UnityEngine.InputSystem;

public class Look : MonoBehaviour
{
    private PlayerControls controls;

    [HideInInspector] public bool canRotateCamera;
    private bool isDebugCamera = false;

    [SerializeField] private Camera cam;
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    private const float smoothing = 0.1f;

    [HideInInspector] public Transform orientation;

    private Vector2 inputVector;
    private float xRot;
    private float yRot;

    private void Start()
    {
        controls = new PlayerControls();
        controls.Camera.Enable();
    }
    private void OnDestroy()
    {
    }


    private void Update()
    {
        if (!isDebugCamera)
            return;

        GetInput();
        UpdateCamera();
    }

    private void GetInput()
        => inputVector = controls.Camera.Look.ReadValue<Vector2>();

    private void UpdateCamera()
    {
        if(!cam || !canRotateCamera)
            return;

        // Laggy beauty
        yRot += inputVector.x * 0.01f * sensX;//Settings.sensetivity;
        xRot -= inputVector.y * 0.01f * sensY;//Settings.sensetivity;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        cam.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        //orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }

    public void SetRotation(float x, float y)
    {
        xRot = x;
        yRot = y;
    }


    private void ControlCursor(CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;

        if (lockMode == CursorLockMode.Locked)
        {
            canRotateCamera = true;
        }
        else if (lockMode == CursorLockMode.None)
        {
            canRotateCamera = false;
        }
    }

    public void SetWorkMode(bool isDebugCamera)
    {
        this.isDebugCamera = isDebugCamera;

        if (isDebugCamera)
        {
            ControlCursor(CursorLockMode.Locked);
        }
        else
        {
            ControlCursor(CursorLockMode.None);
        }
    } 
}

