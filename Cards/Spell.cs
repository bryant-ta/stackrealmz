using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Spell : Card {
    public SO_Spell spellData;
    
    public Stat manaCost;
    public CardText cardText;

    // lazy here but finishing up project...
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI manaCostText;

    ISpell spellFunc;
    
    new void Start() {
        Setup(spellData);
        manaCost = new Stat(spellData.manaCost);
        
        cardText = spellData.cardText;
        foreach (var e in cardText.effects) {
            e.effectFunc = EffectTypeLookUp.CreateEffect(e.effectType);
            e.source = this;
        }

        spellFunc = SpellTypeLookUp.LookUp[spellData.spellType];
        
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.StartBattle, Activatable);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.WonBattle, NonActivatable);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.LostBattle, NonActivatable);

        hintText.text = cardText.text + "\nRight-click to Cancel.";
        manaCostText.text = manaCost.Value.ToString();
    }
    
    void Activatable() { EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell); }
    void NonActivatable() { EventManager.Unsubscribe(gameObject, EventID.SecondaryDown, CastSpell); }

    void CastSpell() {
        if (mSlot && GameManager.Instance.Mana >= manaCost.Value) {
            GameManager.Instance.ModifyMana(-manaCost.Value);
            
            StartCoroutine(ExecuteSpell());
            
            ShowHintText();
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
            HideHintText();
        }
        
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell);
    }

    void UnreadySpell() {
        GameManager.Instance.ModifyMana(manaCost.Value);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, CastSpell);
        
        HideHintText();
    }
    
    void ShowHintText() { hintText.gameObject.SetActive(true); }
    void HideHintText() { hintText.gameObject.SetActive(false); }
    
    void UpdateManaCostText(int val) {
        manaCostText.text = val.ToString();
    }
}
