using System.Collections;
using System.Collections.Generic;

public class Spell : Card {
    public SO_Spell spellData;
    
    public Stat manaCost;
    public CardText cardText;

    ISpell spellFunc;
    
    new void Start() {
        Setup(spellData);
        manaCost = new Stat(spellData.manaCost);
        
        cardText = spellData.cardText;
        cardText.effect.effectFunc = EffectTypeLookUp.CreateEffect(cardText.effect.effectType);

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
        print("Spell " + spellData.name + "ready! Select " + cardText.targetArgs.numTargetTimes + " targets.");
        yield return StartCoroutine(Player.Instance.SelectTargets(new TargetArgs() {
            targetType =  cardText.targetArgs.targetType,
            originSlot = null,
            targetSlotState = cardText.targetArgs.targetSlotState,
            targetSameTeam = cardText.targetArgs.targetSameTeam,
            targetGroup =  cardText.targetArgs.targetGroup,
            numTargetTimes = cardText.targetArgs.numTargetTimes,
        }, (ret) => { targetSlots = ret; }));
        
        // Target select canceled, abort
        if (targetSlots == null) {
            UnreadySpell();
            yield break;
        }
        
        // Execute spell on targets
        if (targetSlots.Count > 0) {
            spellFunc.Execute(targetSlots, cardText);
        }
        
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell);
    }

    void UnreadySpell() {
        GameManager.Instance.ModifyMana(manaCost.Value);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell);
    }
}
