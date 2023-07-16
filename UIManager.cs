using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    // Recipe Viewer
    public GameObject recipeViewer;
    public TextMeshProUGUI recipeProductText;
    public TextMeshProUGUI recipeMaterialsText;
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

        // Recipe Viewer
        EventManager.Subscribe<SO_Card>(gameObject, EventID.TertiaryDown, UpdateRecipeViewer);
        EventManager.Subscribe(worldBackground, EventID.TertiaryDown, HideRecipeViewer);
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

    void UpdateRecipeViewer(SO_Card targetCard) {
        recipeViewer.SetActive(true);
        
        // Target recipe
        string materialsText = "";
        List<Recipe> targetRecipes = CardFactory.LookupRecipesWithProduct(targetCard);
        foreach (Recipe r in targetRecipes) {
            materialsText += "> ";
            foreach (string s in r.materials) {
                materialsText += s + ", ";
            }

            materialsText = materialsText.Remove(materialsText.Length - 2);
            materialsText += "\n";
        }

        recipeProductText.text = targetCard.name;
        recipeMaterialsText.text = materialsText;
        
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
    void HideRecipeViewer() {
        recipeViewer.SetActive(false);
    }
}
