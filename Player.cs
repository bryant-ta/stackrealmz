using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public Moveable heldCard;

    [SerializeField] LayerMask dragLayer;
    [SerializeField] LayerMask cardLayer;

    CameraController mainCameraCtrl;
    
    void Start() {
        mainCameraCtrl = Camera.main.GetComponent<CameraController>();
    }

    void OnPrimaryDown() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                if (hit.collider.gameObject.TryGetComponent(out Moveable moveable)) {
                    HoldCard(moveable);
                }
                
                // EventManager.Invoke(hit.collider.gameObject, EventID.PrimaryDown);
            }
        }
    }
    
    void OnSecondaryDown() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                EventManager.Invoke(hit.collider.gameObject, EventID.SecondaryDown);
            }
        }
    }
    
    void OnPrimaryUp() {
        DropCard(heldCard);
    }

    void OnCameraZoom() {
        mainCameraCtrl.ZoomToggle();
    }
    
    /////////////////////////////////////////////////////////////////////////////////////////////////


    void HoldCard(Moveable c) {
        Transform stackTrans = c.PickUp();
        if (stackTrans) {
            heldCard = c;
            StartCoroutine(FollowMouse(stackTrans));
        }
    }

    IEnumerator FollowMouse(Transform objTrans) {
        while (heldCard != null) {
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

    void DropCard(Moveable c) {
        if (heldCard != null) {
            heldCard.Drop();
        }
        heldCard = null;
    }
}
