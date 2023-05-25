using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animal))]  // Must be on same object as Animal for events to register
public class AnimalUI : MonoBehaviour {
    public Animal mAnimal;
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;

    public Image attackBar;
    public Image attackBarFill;

    void Start() {
        nameText.text = mAnimal.animalData.name;
        hpText.text = mAnimal.animalData.hp.ToString();
        attackText.text = mAnimal.animalData.atkDmg.ToString();

        EventManager.AddListener<int>(gameObject, EventName.OnHeal, UpdateHPText);
        EventManager.AddListener<int>(gameObject, EventName.OnDamage, UpdateHPText);
        EventManager.AddListener<int>(gameObject, EventName.OnSetHp, UpdateHPText);
        EventManager.AddListener<CombatTickerArgs>(gameObject, EventName.OnAttackTick, UpdateAttackBar);
    }

    public void UpdateHPText(int n) {
        print("hello");
        hpText.text = n.ToString();
    }
    
    public void UpdateAttackText(int n) {
        attackText.text = n.ToString();
    }

    public void UpdateAttackBar(CombatTickerArgs args) {
        attackBarFill.fillAmount = (float) args.curTick / args.endTick;


    }
}
