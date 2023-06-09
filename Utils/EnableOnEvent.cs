using System.Collections.Generic;
using UnityEngine;

/*
 * EnableOnEvent enables targetObjs when an event is invoked on the ownerObj
 * - Use Setup() when adding component through script
 */
public class EnableOnEvent : MonoBehaviour {
    public List<GameObject> targetObjs;
    public GameObject eventOriginObj;
    public EventID enableEventID;
    public EventID disableEventID;
    
    void Start()
    {
        EventManager.Subscribe(eventOriginObj, enableEventID, EnableTargetObj);
        EventManager.Subscribe(eventOriginObj, disableEventID, DisableTargetObj);
    }

    // public void Setup(GameObject ownerObj, EventID enableEventID, EventID disableEventID) {
    //     this.ownerObj = ownerObj;
    //     this.enableEventID = enableEventID;
    //     this.disableEventID = disableEventID;
    // }

    void EnableTargetObj() {
        foreach (GameObject obj in targetObjs) {
            obj.SetActive(true);
        }
    }
    void DisableTargetObj() {
        foreach (GameObject obj in targetObjs) {
            obj.SetActive(false);
        }
    }
}
