using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public static Player Instance => _instance;
    static Player _instance;

    [SerializeField] LayerMask dragLayer;
    [SerializeField] LayerMask cardLayer;

    IMoveable heldMoveable;
    Camera mainCamera;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        mainCamera = Camera.main;
    }

    public void OnPrimaryDown() {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                if (hit.collider.gameObject.TryGetComponent(out IMoveable moveable)) {
                    HoldMoveable(moveable);
                }

                if (hit.collider.gameObject.TryGetComponent(out Card card)) {
                    EventManager.Invoke(gameObject, EventID.PrimaryDown, card);
                }
            }
        }
    }

    public void OnSecondaryDown() {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                EventManager.Invoke(hit.collider.gameObject, EventID.SecondaryDown);
            }
        }

        EventManager.Invoke(gameObject, EventID.SecondaryDown);
    }

    public void OnTertiaryDown() {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            if (hit.collider != null) {
                EventManager.Invoke(hit.collider.gameObject, EventID.TertiaryDown);
            }
        }
    }

    public void OnPrimaryUp() { DropMoveable(heldMoveable); }

    public void OnCameraZoom() { }

    ////////////////////////////////    Targeting    ///////////////////////////////

    void EnableTargetMode() {
        EventManager.Subscribe<Card>(gameObject, EventID.PrimaryDown, TargetSelected);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CancelSelectTarget);
    }
    void DisableTargetMode() {
        EventManager.Unsubscribe<Card>(gameObject, EventID.PrimaryDown, TargetSelected);
        EventManager.Unsubscribe(gameObject, EventID.SecondaryDown, CancelSelectTarget);
    }

    bool isSelectingTarget = true;
    bool selectTargetCanceled = false;
    CombatSlot selectedSlot = null;
    public IEnumerator SelectTarget(Action<CombatSlot> selectSlot) {
        isSelectingTarget = true;
        selectTargetCanceled = false;

        EnableTargetMode();

        while (isSelectingTarget) {
            // Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            // add visual effect of selecting here
            yield return null;
        }

        DisableTargetMode();

        if (selectTargetCanceled) { // card select canceled
            selectSlot(null);
        } else { // card selected, return that card if its valid
            selectSlot(selectedSlot);
        }
    }

    void TargetSelected(Card c) {
        isSelectingTarget = false;
        if (c is Animal animal)
            selectedSlot = animal.mSlot as CombatSlot;
        else
            print("Target selected is not an Animal");
    }
    void CancelSelectTarget() {
        isSelectingTarget = false;
        selectTargetCanceled = true;
    }

    ////////////////////////////////    Holding Cards    ///////////////////////////////

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