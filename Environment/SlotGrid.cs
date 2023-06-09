using UnityEngine;

public class SlotGrid : MonoBehaviour {
    public Slot[,] slotGrid;
    [SerializeField] int width, height;

    public GameObject slotPrefab;

    void Start() {
        SetupSlotGrid();
    }

    void SetupSlotGrid() {
        slotGrid = new Slot[width, height];
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).TryGetComponent(out Slot s)) {
                if (slotGrid[s.x, s.y] != null) {
                    Debug.LogError("Two slots have same coordinates");
                }
                slotGrid[s.x, s.y] = s;
            } else {
                Debug.LogError("Non-Slot object in SlotGrid children");
            }
        }
    }

    public Slot Forward(Slot origin, bool flip) {
         return SelectSlot(origin, flip, new Vector2Int(1, 0));
    }

    public Slot SelectSlot(Slot origin, bool flip, Vector2Int relativePos) {
        if (flip) relativePos.x = -relativePos.x;
        int targetX = origin.x + relativePos.x;
        int targetY = origin.y + relativePos.y;

        if (targetX > width - 1 || targetX < 0 || targetY > height - 1 || targetY < 0) {
            return null;
        }

        return slotGrid[targetX, targetY];
    }

    void CreateSlotGrid() {
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                Instantiate(slotPrefab, new Vector3(transform.position.x + i, 0, transform.position.z + j),
                    transform.rotation, transform);
            }
        }
    }
}
