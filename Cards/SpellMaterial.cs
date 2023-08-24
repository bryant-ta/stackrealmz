using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMaterial : MonoBehaviour {
    
}

public class SpellElement : SpellMaterial {
    public Effect spellEffect;
}

public class SpellTargetType : SpellMaterial {
    public TargetType spellTargetType;
}

public class SpellModifier : SpellMaterial {
    public int val;
}