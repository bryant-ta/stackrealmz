using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] Moveable heldMoveable;

    [SerializeField] LayerMask dragLayer;
    [SerializeField] LayerMask cardLayer;

    CameraController mainCameraCtrl;
    
    void Start() {
        mainCameraCtrl = Camera.main.GetComponent<CameraController>();
    }

    public void OnPrimaryDown() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                if (hit.collider.gameObject.TryGetComponent(out Moveable moveable)) {
                    HoldMoveable(moveable);
                }
                
                // EventManager.Invoke(hit.collider.gameObject, EventID.PrimaryDown);
            }
        }
    }
    
    public void OnSecondaryDown() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                EventManager.Invoke(hit.collider.gameObject, EventID.SecondaryDown);
            }
        }
    }
    
    public void OnPrimaryUp() {
        DropMoveable(heldMoveable);
    }

    // public void OnPan()
    // {
    //     mainCameraCtrl.DragAndMove(context);
    // }

    public void OnCameraZoom() {
        mainCameraCtrl.ZoomToggle();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////


    void HoldMoveable(Moveable c) {
        Transform stackTrans = c.PickUp();
        if (stackTrans) {
            heldMoveable = c;
            StartCoroutine(FollowMouse(stackTrans));
        }
    }

    IEnumerator FollowMouse(Transform objTrans) {
        while (heldMoveable != null) {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, dragLayer)) {
                if (hit.collider != null) {
                    objTrans.position = Vector3.Lerp(objTrans.position, hit.point, Constants.CardDragSpeed * Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    void DropMoveable(Moveable c) {
        if (heldMoveable != null) {
            heldMoveable.Drop();
        }
        heldMoveable = null;
    }
}
