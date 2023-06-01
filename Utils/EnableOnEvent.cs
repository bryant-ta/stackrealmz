using UnityEngine;

public class EnableOnEvent : MonoBehaviour {
    public GameObject targetObj;
    public GameObject ownerObj;
    public EventID enableEventID;
    public EventID disableEventID;
    
    void Start()
    {
        EventManager.Subscribe(ownerObj, enableEventID, EnableTargetObj);
        EventManager.Subscribe(ownerObj, disableEventID, DisableTargetObj);
    }

    void EnableTargetObj() {
        targetObj.SetActive(true);
    }
    void DisableTargetObj() {
        targetObj.SetActive(false);
    }
}
