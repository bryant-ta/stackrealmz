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
                
                EventManager.Invoke(gameObject, EventID.PrimaryDown, hit.collider.gameObject);
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
    
    public void OnPrimaryUp() {
        DropMoveable(heldMoveable);
    }

    public void OnCameraZoom() {
        
    }

    ////////////////////////////////    Targeting    ///////////////////////////////

    void Start() {
        EventManager.Subscribe<Card>(gameObject, EventID.PrimaryDown, TargetSelected);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CancelSelectTarget);
    }

    bool isSelectingTarget = true;
    bool selectTargetCanceled = false;
    Animal selectedAnimal = null;
    public IEnumerator SelectTarget(EffectOrder effectOrder, Action<Animal> selectAnimal) {
        isSelectingTarget = true;
        selectTargetCanceled = false;
        
        while (isSelectingTarget) {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Debug.DrawLine(effectOrder.origin.transform.position, mousePosition);
            yield return null;
        }
        
        if (selectTargetCanceled) {         // card select canceled, return played card to village
            CombatSlot slot = effectOrder.origin.mSlot as CombatSlot;
            slot.PickUp();
            StartCoroutine(Utils.MoveStackToPoint(effectOrder.origin.mStack,
                WaveManager.Instance.cleanUpDepositPoint.position));
        } else {                            // card selected, return that card if its valid
            selectAnimal(selectedAnimal);
        }
    }

    void TargetSelected(Card c) {
        isSelectingTarget = false; 
        selectedAnimal = c as Animal;   // unsafe
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
