using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGrid : MonoBehaviour {
    public Slot[,] slotGrid;
    public int width, height;

    void Start() {
        slotGrid = new Slot[width, height];
    }
}
