using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Battle")]
public class SO_Battle : ScriptableObject {
    public List<Wave> waves;
}