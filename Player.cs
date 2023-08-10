using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public static Player Instance => _instance;
    static Player _instance;

    [SerializeField] LayerMask dragLayer;
    [SerializeField] LayerMask cardLayer;
    [SerializeField] LayerMask slotLayer;

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
        
        // Click Cards
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                // print(hit.collider.gameObject.name);

                if (hit.collider.gameObject.TryGetComponent(out IMoveable moveable)) {
                    HoldMoveable(moveable);
                }
            }
        }
        
        // Click Slots
        if (Physics.Raycast(ray, out hit, 100.0f, slotLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                // print(hit.collider.gameObject.name);

                if (hit.collider.gameObject.TryGetComponent(out CombatSlot combatSlot)) {
                    EventManager.Invoke(gameObject, EventID.PrimaryDown, combatSlot);
                }
            }
        }
    }

    public void OnSecondaryDown() {
        EventManager.Invoke(gameObject, EventID.SecondaryDown); // note: needs to be before hit.invoke for targeting cancel trigger

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                EventManager.Invoke(hit.collider.gameObject, EventID.SecondaryDown);
            }
        }
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
        EventManager.Subscribe<CombatSlot>(gameObject, EventID.PrimaryDown, TargetSelected);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CancelSelectTarget);
    }
    void DisableTargetMode() {
        EventManager.Unsubscribe<CombatSlot>(gameObject, EventID.PrimaryDown, TargetSelected);
        EventManager.Unsubscribe(gameObject, EventID.SecondaryDown, CancelSelectTarget);
    }

    // SelectTargets runs selection loop for targeting cards (spells). Returns null if targeting is canceled.
    bool selectTargetCanceled = false;
    CombatSlot selected = null;
    public IEnumerator SelectTargets(TargetArgs args, Action<List<CombatSlot>> selectSlots) {
        List<CombatSlot> targetSlots = new List<CombatSlot>();
        selectTargetCanceled = false;

        EnableTargetMode();

        while (targetSlots.Count != args.numTargetTimes) {
            yield return null;
            if (selectTargetCanceled) { // card select canceled
                print("Spell canceled.");
                selectSlots(null);
                yield break;
            }

            if (selected != null) { // card selected
                args.originSlot = selected;
                List<CombatSlot> a = TargetTypes.GetTargets(args);
                if (a.Count > 0) {
                    if (targetSlots.Count == 0 && a[0].IsEmpty()) {
                        selected = null;
                        print("Select non-empty slot for first target");
                        continue;
                    }

                    targetSlots.Add(a[0]);
                } else {
                    print("Invalid target.");
                }
            }

            selected = null;
        }

        DisableTargetMode();

        selectSlots(targetSlots);
    }

    void TargetSelected(CombatSlot c) { selected = c; }
    void CancelSelectTarget() { selectTargetCanceled = true; }

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