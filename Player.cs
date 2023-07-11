using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] LayerMask dragLayer;
    [SerializeField] LayerMask cardLayer;

    IMoveable heldMoveable;

    public void OnPrimaryDown() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                if (hit.collider.gameObject.TryGetComponent(out IMoveable moveable)) {
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

    public void OnTertiaryDown() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            if (hit.collider != null) {
                EventManager.Invoke(hit.collider.gameObject, EventID.TertiaryDown);
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
        
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////


    void HoldMoveable(IMoveable c) {
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

    void DropMoveable(IMoveable c) {
        if (heldMoveable != null) {
            heldMoveable.Drop();
        }
        heldMoveable = null;
    }
}
