using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable {
    bool IsStackable { get; set;  }

    Transform PickUp();
    void Drop();
}
