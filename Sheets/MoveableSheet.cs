using UnityEngine;

[RequireComponent(typeof(Sheet))]
public class MoveableSheet : MonoBehaviour, IMoveable {
    public bool IsStackable { get => isStackable; set => isStackable = value; }
    [SerializeField] bool isStackable;
    
    public Transform PickUp() {
        return transform;
    }

    public void Drop()
    {
        StartCoroutine(Utils.MoveObjToPoint(transform, new Vector3(transform.position.x, Constants.SheetYLayer, transform.position.z)));
    }
}
