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
    public TextMeshProUGUI manaText;
    public Image manaBarFill;
    
    public TextMeshProUGUI waveText;
    public Image waveProgressFill;

    public TextMeshProUGUI dayText;
    public Image timeBarFill;

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
        
        EventManager.Subscribe<ManaArgs>(gameMngr.gameObject, EventID.ModifyMana, UpdateManaBar);
        EventManager.Subscribe<ManaArgs>(gameMngr.gameObject, EventID.ModifyMaxMana, UpdateManaBar);
        
        EventManager.Subscribe<WaveArgs>(waveMngr.gameObject, EventID.StartWave, UpdateWaveText);
        EventManager.Subscribe<CombatTickerArgs>(waveMngr.gameObject, EventID.WaveTick, UpdateWaveProgressBar);
    }

    // TODO: Consider separating into 2 objects when learning more about UI
    void UpdateHpText(int val) { lifeText.text = val + "/" + gameMngr.playerLife.maxHp; }
    void UpdateMaxHpText(int val) { lifeText.text = gameMngr.playerLife.hp + "/" + val; }

    void UpdateMoneyText(int val) { moneyText.text = val.ToString(); }

    void UpdateManaBar(ManaArgs args) {
        manaBarFill.fillAmount = (float) args.curMana / args.maxMana;
        manaText.text = args.curMana.ToString();
    }
    
    void UpdateWaveText(WaveArgs args) { waveText.text = "Waves Left: " + args.wavesLeft; }
    void UpdateWaveProgressBar(CombatTickerArgs args) {
        waveProgressFill.fillAmount = (float) args.curTick / args.endTick;
    }

    public void UpdateDayText(int day) { dayText.text = "Day " + day; }
    public void UpdateTimeProgressBar(float percent) { timeBarFill.fillAmount = percent; }
}
