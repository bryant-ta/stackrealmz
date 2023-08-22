using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animal))]  // Must be on same object as Animal for events to register
public class AnimalUI : CardUI {
    public Animal mAnimal;
    
    public TextMeshProUGUI manaCostText;
    
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI poisonText;
    
    public TextMeshProUGUI attackText;

    public Image attackBarFill;
    public Image attackBarBack;
    public Image abilityBarFill;
    public Image abilityBarBack;

    void Awake() {
        EventManager.Subscribe<int>(gameObject, EventID.SetManaCost, UpdateManaCostText);
        EventManager.Subscribe(gameObject, EventID.ExitCombat, ShowManaCostText);
        EventManager.Subscribe(gameObject, EventID.EnterCombat, HideManaCostText);
        
        EventManager.Subscribe<int>(gameObject, EventID.Heal, UpdateHpText);
        EventManager.Subscribe<int>(gameObject, EventID.Damage, UpdateHpText);
        EventManager.Subscribe<int>(gameObject, EventID.SetHp, UpdateHpText);
        EventManager.Subscribe<int>(gameObject, EventID.SetArmor, UpdateArmorText);
        EventManager.Subscribe<int>(gameObject, EventID.SetPoison, UpdatePoisonText);
        
        EventManager.Subscribe<int>(gameObject, EventID.SetAttack, UpdateAttackText);
        
        EventManager.Subscribe<CombatTickerArgs>(gameObject, EventID.AttackTick, UpdateAttackBar);
        EventManager.Subscribe(gameObject, EventID.EnterCombat, ShowAttackBar);
        EventManager.Subscribe(gameObject, EventID.ExitCombat, HideAttackBar);

        EventManager.Subscribe<CombatTickerArgs>(gameObject, EventID.AbilityTick, UpdateAbilityBar);
        EventManager.Subscribe(gameObject, EventID.EnterCombat, ShowAbilityBar);
        EventManager.Subscribe(gameObject, EventID.ExitCombat, HideAbilityBar);
    }

    new void Start() {
        base.Start();
        manaCostText.text = mAnimal.animalData.manaCost.ToString();
        hpText.text = mAnimal.animalData.hp.ToString();
        attackText.text = mAnimal.animalData.atk.ToString();
    }
    
    /*******************   Mana   *******************/

    void UpdateManaCostText(int val) {
        manaCostText.text = val.ToString();
    }
    void ShowManaCostText() { manaCostText.gameObject.SetActive(true);}
    void HideManaCostText() { manaCostText.gameObject.SetActive(false);}
    
    /*******************   Health   *******************/
    
    void UpdateHpText(int val) {
        hpText.text = val.ToString();
    }
    void UpdateArmorText(int val) {
        armorText.text = val.ToString();
        
        if (val == 0) armorText.gameObject.SetActive(false);
        else armorText.gameObject.SetActive(true);
    }
    void UpdatePoisonText(int val) {
        poisonText.text = val.ToString();
        
        if (val == 0) poisonText.gameObject.SetActive(false);
        else poisonText.gameObject.SetActive(true);
    }
    
    /*******************   Attack   *******************/
    
    void UpdateAttackText(int val) {
        attackText.text = val.ToString();
    }
    
    void UpdateAttackBar(CombatTickerArgs args) {
        attackBarFill.fillAmount = (float) args.curTick / args.endTick;
    }
    void ShowAttackBar() { attackBarBack.gameObject.SetActive(true);}
    void HideAttackBar() { attackBarBack.gameObject.SetActive(false);}
    
    void UpdateAbilityBar(CombatTickerArgs args) {
        abilityBarFill.fillAmount = (float) args.curTick / args.endTick;
    }
    void ShowAbilityBar() { abilityBarBack.gameObject.SetActive(true);}
    void HideAbilityBar() { abilityBarBack.gameObject.SetActive(false);}
}