using System.Collections;
using System.Collections.Generic;

public class Spell : Card {
    public SO_Spell spellData;
    
    public Stat manaCost;
    public CardText cardText;

    ISpell spellFunc;
    
    void Start() {
        Setup(spellData);
        manaCost = new Stat(spellData.manaCost);
        
        cardText = spellData.cardText;
        cardText.effect.effectFunc = EffectTypeLookUp.LookUp[cardText.effect.effectType];

        spellFunc = SpellTypeLookUp.LookUp[spellData.spellType];
        
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell);
    }

    void CastSpell() {
        if (mSlot && GameManager.Instance.Mana >= manaCost.Value) {
            GameManager.Instance.ModifyMana(-manaCost.Value);
            
            StartCoroutine(ExecuteSpell());
        }
    }

    IEnumerator ExecuteSpell() {
        EventManager.Unsubscribe(gameObject, EventID.SecondaryDown, CastSpell);
        
        List<CombatSlot> targetSlots = new List<CombatSlot>();
        CombatSlot targetSlot = null;

        // Select spell targets
        print("Spell " + spellData.name + "ready! Select " + cardText.numTargets + " targets.");
        for (int i = 0; i < cardText.numTargets; i++) {
            yield return null;
            yield return StartCoroutine(Player.Instance.SelectTarget((ret) => {
                targetSlot = ret;
            }));

            // Target select canceled, abort
            if (!targetSlot) {
                UnreadySpell();
                yield break;
            }
            
            targetSlots.Add(targetSlot);
        }
        
        // Execute spell on targets
        spellFunc.Execute(targetSlots, cardText);
        
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell);
    }

    void UnreadySpell() {
        GameManager.Instance.ModifyMana(manaCost.Value);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell);
    }
}
