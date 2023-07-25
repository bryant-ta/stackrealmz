using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// UIManager singleton handles global UI elements
// Load Order: before GameManager ... Idk why this works?
public class UIManager : MonoBehaviour {
    public static UIManager Instance => _instance;
    static UIManager _instance;

    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI moneyText;
    public Image timeBarFill;
    public Image waveProgressFill;
    public Image manaBarFill;
    public TextMeshProUGUI manaText;

    // Inspector
    public GameObject inspectorObj;
    public TextMeshProUGUI inspectorHeaderText;
    public TextMeshProUGUI inspectorBodyText;
    public GameObject worldBackground; // for hiding recipe viewer
    public GameObject craftsRecipePanel;
    public Transform craftsGrid;

    public GameManager gameMngr;
    public WaveManager waveMngr;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.Heal, UpdateHpText);
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.Damage, UpdateHpText);
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.SetHp, UpdateHpText);
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.SetMaxHp, UpdateMaxHpText);
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.ModifyMoney, UpdateMoneyText);

        EventManager.Subscribe<CombatTickerArgs>(waveMngr.gameObject, EventID.WaveTick, UpdateWaveProgressBar);
        
        EventManager.Subscribe<ManaArgs>(gameMngr.gameObject, EventID.ModifyMana, UpdateManaBar);
        EventManager.Subscribe<ManaArgs>(gameMngr.gameObject, EventID.ModifyMaxMana, UpdateManaBar);

        // Inspector
        EventManager.Subscribe<Card>(gameObject, EventID.TertiaryDown, UpdateInspector);
        EventManager.Subscribe(worldBackground, EventID.TertiaryDown, HideInspector);
    }

    // TODO: Consider separating into 2 objects when learning more about UI
    void UpdateHpText(int val) { lifeText.text = val + "/" + gameMngr.playerLife.maxHp; }
    void UpdateMaxHpText(int val) { lifeText.text = gameMngr.playerLife.hp + "/" + val; }

    void UpdateMoneyText(int val) { moneyText.text = val.ToString(); }

    public void UpdateTimeProgressBar(float percent) { timeBarFill.fillAmount = percent; }
    void UpdateWaveProgressBar(CombatTickerArgs args) {
        waveProgressFill.fillAmount = (float) args.curTick / args.endTick;
    }

    void UpdateManaBar(ManaArgs args) {
        manaBarFill.fillAmount = (float) args.curMana / args.maxMana;
        manaText.text = args.curMana.ToString();
    }

    void UpdateInspector(Card targetCard) {
        inspectorObj.SetActive(true);

        string bodyText = "";
        if (targetCard is Animal a && a.isInCombat) {       // Show effects on Animal
            bodyText = a.EffectCtrl.ActiveEffectsToString();
        } else {                                            // Show target recipe
            List<Recipe> targetRecipes = CardFactory.LookupRecipesWithProduct(targetCard.cardData);
            foreach (Recipe r in targetRecipes) {
                bodyText += "> ";
                foreach (string s in r.materials) {
                    bodyText += s + ", ";
                }

                bodyText = bodyText.Remove(bodyText.Length - 2);
                bodyText += "\n";
            }
        }

        inspectorHeaderText.text = targetCard.name;
        inspectorBodyText.text = bodyText;
        
        // // TODO: determine if this is even needed, currently broken
        // // Crafts recipe
        // List<Recipe> craftsRecipes = CardFactory.LookupRecipesWithMaterial(targetCard);
        // foreach (Recipe r in craftsRecipes) {
        //     string craftsText = "> ";
        //     foreach (SO_Card s in r.products) {
        //         craftsText += s.name + ", ";
        //     }
        //
        //     craftsText = craftsText.Remove(craftsText.Length - 2);
        //     
        //     TextMeshProUGUI craftsPanelText = Instantiate(craftsRecipePanel, craftsGrid).GetComponent<TextMeshProUGUI>();
        //     craftsPanelText.text = craftsText;
        // }
    }
    void HideInspector() {
        inspectorObj.SetActive(false);
    }
}
