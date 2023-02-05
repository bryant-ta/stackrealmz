using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public float dragSpeed;
    
    public Card heldCard;

    [SerializeField] LayerMask dragLayer;
    [SerializeField] LayerMask cardLayer;
    
    void OnPrimaryDown() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, cardLayer, QueryTriggerInteraction.Ignore)) {
            if (hit.collider != null) {
                Card c = hit.collider.gameObject.GetComponent<Card>();
                if (c != null) {
                    HoldCard(c);
                }
            }
        }
    }
    
    void OnPrimaryUp() {
        DropCard(heldCard);
    }

    void HoldCard(Card c) {
        heldCard = c;
        heldCard.Pickup();
        StartCoroutine(FollowMouse());
    }

    IEnumerator FollowMouse() {
        GameObject obj = heldCard.gameObject;
        
        while (heldCard != null) {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f, dragLayer)) {
                if (hit.collider != null) {
                    obj.transform.position = Vector3.Lerp(obj.transform.position, hit.point, dragSpeed*Time.deltaTime);
                }
            }
            yield return null;
        }


        yield return null;
    }

    void DropCard(Card c) {
        if (heldCard != null) {
            heldCard.Fall();
        }
        heldCard = null;
    }
}
