    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

/* Adapted from OneWheelStudio - Adventures-in-C-Sharp (https://github.com/onewheelstudio/Adventures-in-C-Sharp/blob/main/License)
   
   Used under:
   
    MIT License

    Copyright (c) 2022 onewheelstudio

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
 */
    public class CameraMovement : MonoBehaviour
    {
        CameraControls cameraActions;
        InputAction movement;
        Camera mainCamera;
        Transform mainCameraTransform;

        [Header("Horizontal Translation")]
        [SerializeField] float maxSpeed = 5f;
        float speed;
        [SerializeField] float acceleration = 10f;
        [SerializeField] float damping = 15f;

        [Header("Vertical Translation")]
        [SerializeField] float stepSize = 2f;
        [SerializeField] float zoomDampening = 7.5f;
        [SerializeField] float minZoom = 5f;
        [SerializeField] float maxZoom = 50f;

        [Header("Edge Movement")]
        [SerializeField] [Range(0f,0.1f)] float edgeTolerance = 0.05f;

        //value set in various functions 
        //used to update the position of the camera base object.
        Vector3 targetPosition;

        float zoomSize;

        //used to track and maintain velocity w/o a rigidbody
        Vector3 horizontalVelocity;
        Vector3 lastPosition;

        //tracks where the dragging action started
        Vector3 startDrag;

        void Awake()
        {
            cameraActions = new CameraControls();
            mainCamera = GetComponentInChildren<Camera>();
            mainCameraTransform = mainCamera.transform;
        }

        void OnEnable()
        {
            zoomSize = mainCamera.orthographicSize;
            mainCameraTransform.LookAt(transform);

            lastPosition = transform.position;

            movement = cameraActions.Camera.MoveCamera;
            cameraActions.Camera.ZoomCamera.performed += ZoomCamera;
            cameraActions.Camera.Enable();
        }

        void OnDisable()
        {
            cameraActions.Camera.ZoomCamera.performed -= ZoomCamera;
            cameraActions.Camera.Disable();
        }

        void Update()
        {
            //inputs
            GetKeyboardMovement();
            // CheckMouseAtScreenEdge();
            DragCamera();

            //move base and camera objects
            UpdateVelocity();
            UpdateBasePosition();
            UpdateZoom();
        }

        void UpdateVelocity()
        {
            horizontalVelocity = (transform.position - lastPosition) / Time.deltaTime;
            horizontalVelocity.y = 0f;
            lastPosition = transform.position;
        }

        void GetKeyboardMovement()
        {
            Vector3 inputValue = movement.ReadValue<Vector2>().x * Vector3.right
                        + movement.ReadValue<Vector2>().y * Vector3.forward;

            inputValue = inputValue.normalized;

            if (inputValue.sqrMagnitude > 0.1f)
                targetPosition += inputValue;
        }

        void DragCamera()
        {
            if (!Mouse.current.middleButton.isPressed)
                return;

            //create plane to raycast to
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
            if(plane.Raycast(ray, out float distance))
            {
                if (Mouse.current.middleButton.wasPressedThisFrame)
                    startDrag = ray.GetPoint(distance);
                else
                    targetPosition += startDrag - ray.GetPoint(distance);
                
            }
        }

        void CheckMouseAtScreenEdge()
        {
            //mouse position is in pixels
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 moveDirection = Vector3.zero;

            //horizontal scrolling
            if (mousePosition.x < edgeTolerance * Screen.width)
                moveDirection += -GetCameraRight();
            else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
                moveDirection += GetCameraRight();

            //vertical scrolling
            if (mousePosition.y < edgeTolerance * Screen.height)
                moveDirection += -GetCameraForward();
            else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
                moveDirection += GetCameraForward();

            targetPosition += moveDirection;
        }

        void UpdateBasePosition()
        {
            if (targetPosition.sqrMagnitude > 0.1f)
            {
                //create a ramp up or acceleration
                speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
                transform.position += targetPosition * speed * Time.deltaTime;
            }
            else
            {
                //create smooth slow down
                horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
                transform.position += horizontalVelocity * Time.deltaTime;
            }

            //reset for next frame
            targetPosition = Vector3.zero;
        }

        void ZoomCamera(InputAction.CallbackContext obj)
        {
            float inputValue = -obj.ReadValue<Vector2>().y / 100f;

            if (Mathf.Abs(inputValue) > 0.1f)
            {
                zoomSize = mainCamera.orthographicSize + inputValue * stepSize;

                if (zoomSize < minZoom)
                    zoomSize = minZoom;
                else if (zoomSize > maxZoom)
                    zoomSize = maxZoom;
            }
        }

        void UpdateZoom()
        {
            //set zoom target
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoomSize, Time.deltaTime * zoomDampening);
            
            // mainCameraTransform.LookAt(transform);
        }

        //gets the horizontal forward vector of the camera
        Vector3 GetCameraForward()
        {
            Vector3 forward = mainCameraTransform.forward;
            forward.y = 0f;
            return forward;
        }

        //gets the horizontal right vector of the camera
        Vector3 GetCameraRight()
        {
            Vector3 right = mainCameraTransform.right;
            right.y = 0f;
            return right;
        }
    }
