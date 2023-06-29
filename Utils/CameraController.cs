using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform targetZoomIn;
    public float zoomInFOV = 5f;
    public float zoomDuration = 2f;

    Camera mainCamera;
    Vector3 initialPosition;
    float initialFOV;

    bool isZoomed;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        initialPosition = transform.position;
        initialFOV = mainCamera.fieldOfView;
        
        newPosition = transform.position;
        cam = Camera.main;
    }

    public float panSpeed = 20f;
    public float panSmoothness = 10f;

    Camera cam;
 
    Vector3 newPosition;
    [SerializeField] private float movementTime = 5f;
 
    Vector3 dragStartPosition = Vector3.zero;
    Vector3 dragCurrentPosition = Vector3.zero;

    private void Update()
    {
        ApplyMovements();
    }
 
   
    public void DragAndMove(InputAction.CallbackContext context)
    {
        System.Type vector2Type = Vector2.zero.GetType();
 
        string buttonControlPath = "/Mouse/leftButton";
        //string deltaControlPath = "/Mouse/delta";
 
        if (context.started)
        {
            if (context.control.path == buttonControlPath)
            {
                Debug.Log("Button Pressed Down Event - called once when button pressed");
 
                Ray dragStartRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane dragStartPlane = new Plane(Vector3.up, Vector3.zero);
                float dragStartEntry;
 
                if (dragStartPlane.Raycast(dragStartRay, out dragStartEntry))
                {
                    dragStartPosition = dragStartRay.GetPoint(dragStartEntry);
                }
            }
        }
        else if (context.performed)
        {
            if (context.control.path == buttonControlPath)
            {
                Debug.Log("Button Hold Down - called continously till the button is pressed");
 
                Ray dragCurrentRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane dragCurrentPlane = new Plane(Vector3.up, Vector3.zero);
                float dragCurrentEntry;
 
                if (dragCurrentPlane.Raycast(dragCurrentRay, out dragCurrentEntry))
                {
                    dragCurrentPosition = dragCurrentRay.GetPoint(dragCurrentEntry);
                    newPosition = transform.position + dragStartPosition - dragCurrentPosition;
                }
            }
 
        }
        else if (context.canceled)
        {
            if (context.control.path == buttonControlPath)
            {
                Debug.Log("Button released");
            }
        }
    }
 
    private void ApplyMovements()
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, movementTime * Time.deltaTime);
    }

    private const float InternalMoveTargetSpeed = 8;
    private const float InternalMoveSpeed = 4;
    private Vector3 _moveTarget;
    private Vector3 _moveDirection;
    public void Move(InputAction.CallbackContext context) {
        //Read the input value that is being sent by the Input System
        Vector2 value = context.ReadValue<Vector2>();
        //Store the value as a Vector3, making sure to move the Y input on the Z axis.
        _moveDirection = new Vector3(value.x, 0, value.y);
        //Increment the new move Target position of the camera
        _moveTarget += (transform.forward * _moveDirection.z + transform.right * 
            _moveDirection.x) * Time.fixedDeltaTime * InternalMoveTargetSpeed;
    }
    private void LateUpdate()
    {
        //Lerp  the camera to a new move target position
        transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * InternalMoveSpeed);
    }
    
    
    
    public void ZoomToggle() {
        if (isZoomed) {
            isZoomed = false;
            ZoomOut();
        } else {
            isZoomed = true;
            ZoomIn();
        }
    }

    void ZoomIn()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(targetZoomIn.position, zoomInFOV));
    }
    void ZoomOut()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomRoutine(initialPosition, initialFOV));
    }

    IEnumerator ZoomRoutine(Vector3 targetPosition, float targetScale)
    {
        float t = 0f;
        Vector3 startPosition = transform.position;
        float startScale = mainCamera.fieldOfView;

        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.fieldOfView = Mathf.Lerp(startScale, targetScale, t);

            yield return null;
        }
    }
}
