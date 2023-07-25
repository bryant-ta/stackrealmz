using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animal))]  // Must be on same object as Animal for events to register
public class AnimalUI : CardUI {
    public Animal mAnimal;
    
    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;

    public Image attackBarFill;
    public Image abilityBarFill;

    void Start() {
        base.Start();
        manaCostText.text = mAnimal.animalData.manaCost.ToString();
        hpText.text = mAnimal.animalData.hp.ToString();
        attackText.text = mAnimal.animalData.atkDmg.ToString();

        EventManager.Subscribe<int>(gameObject, EventID.SetManaCost, UpdateManaCostText);
        EventManager.Subscribe<int>(gameObject, EventID.Heal, UpdateHpText);
        EventManager.Subscribe<int>(gameObject, EventID.Damage, UpdateHpText);
        EventManager.Subscribe<int>(gameObject, EventID.SetHp, UpdateHpText);
        EventManager.Subscribe<int>(gameObject, EventID.SetAttack, UpdateAttackText);
        EventManager.Subscribe<CombatTickerArgs>(gameObject, EventID.AttackTick, UpdateAttackBar);
        EventManager.Subscribe<CombatTickerArgs>(gameObject, EventID.AbilityTick, UpdateAbilityBar);
    }

    void UpdateManaCostText(int val) {
        manaCostText.text = val.ToString();
    } 
    void UpdateHpText(int val) {
        hpText.text = val.ToString();
    }
    void UpdateAttackText(int val) {
        attackText.text = val.ToString();
    }
    
    void UpdateAttackBar(CombatTickerArgs args) {
        attackBarFill.fillAmount = (float) args.curTick / args.endTick;
    }
    void UpdateAbilityBar(CombatTickerArgs args) {
        abilityBarFill.fillAmount = (float) args.curTick / args.endTick;
    }
}
