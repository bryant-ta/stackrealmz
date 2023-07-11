using System;
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
            }
        }
    }

    public Slot Forward(Slot origin, bool flip) {
         return SelectSlotRelative(origin, flip, new Vector2Int(1, 0));
    }

    public Slot SelectSlotRelative(Slot origin, bool flip, Vector2Int relativePos) {
        if (flip) relativePos.x = -relativePos.x;
        int targetX = origin.x + relativePos.x;
        int targetY = origin.y + relativePos.y;

        if (targetX > width - 1 || targetX < 0 || targetY > height - 1 || targetY < 0) {
            return null;
        }

        return slotGrid[targetX, targetY];
    }

    public Slot SelectSlot(Vector2Int pos, bool flip) {
        if (flip) pos.x = Math.Abs(pos.x - width + 1);
        
        if (pos.x > width - 1 || pos.x < 0 || pos.y > height - 1 || pos.y < 0) {
            return null;
        }

        return slotGrid[pos.x, pos.y];
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
