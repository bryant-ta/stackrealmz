using System;
using UnityEngine;

[Serializable]
public class Stat {
    [SerializeField] int baseValue;
    [SerializeField] int modifier;

    public int Value => CalculateValue();

    public Stat(int baseValue, int modifier = 0) {
        this.baseValue = baseValue;
        this.modifier = modifier;
    }

    int CalculateValue() {
        int ret = baseValue + modifier;
        if (ret < 0) { ret = 0; }

        return ret;
    }
    
    public void ChangeBaseValue(int delta)
    {
        baseValue = Math.Max(baseValue + delta, 0);
    }
    public void ChangeModifier(int delta)
    {
        modifier = Math.Max(modifier + delta, 0);
    }
}
