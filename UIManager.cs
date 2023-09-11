using System.Collections;
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
    public RectTransform hpDeltaPos;
    
    public TextMeshProUGUI moneyText;
    public GameObject valueDeltaAnim;
    public RectTransform moneyDeltaPos;

    public TextMeshProUGUI manaText;
    public Image manaBarFill;
    
    public TextMeshProUGUI waveText;
    public Image waveProgressFill;

    public TextMeshProUGUI dayText;
    public Image timeBarFill;

    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

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
        EventManager.Subscribe<DeltaArgs>(gameMngr.gameObject, EventID.Heal, UpdateHpText);
        EventManager.Subscribe<DeltaArgs>(gameMngr.gameObject, EventID.Damage, UpdateHpText);
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.SetHp, SetHpText);
        
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.SetMaxHp, UpdateMaxHpText);
        
        EventManager.Subscribe<DeltaArgs>(gameMngr.gameObject, EventID.ModifyMoney, UpdateMoneyText);
        
        EventManager.Subscribe<ManaArgs>(gameMngr.gameObject, EventID.ModifyMana, UpdateManaBar);
        EventManager.Subscribe<ManaArgs>(gameMngr.gameObject, EventID.ModifyMaxMana, UpdateManaBar);
        
        EventManager.Subscribe<WaveArgs>(waveMngr.gameObject, EventID.StartWave, UpdateWaveText);
        EventManager.Subscribe<CombatTickerArgs>(waveMngr.gameObject, EventID.WaveTick, UpdateWaveProgressBar);
        
        EventManager.Subscribe(gameMngr.gameObject, EventID.WonGame, ShowWinText);
        EventManager.Subscribe(gameMngr.gameObject, EventID.LostGame, ShowLoseText);
    }

    // TODO: Consider separating into 2 objects when learning more about UI
    void UpdateHpText(DeltaArgs args) {
        lifeText.text = args.newValue + "/" + gameMngr.playerLife.maxHp;
        StartCoroutine(DoValueChangeAnim(hpDeltaPos, args));
    }
    void SetHpText(int val) {
        lifeText.text = val + "/" + gameMngr.playerLife.maxHp;
    }
    void UpdateMaxHpText(int val) { lifeText.text = gameMngr.playerLife.hp + "/" + val; }

    void UpdateMoneyText(DeltaArgs args) {
        moneyText.text = args.newValue.ToString();
        StartCoroutine(DoValueChangeAnim(moneyDeltaPos, args));
    }

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

    void ShowWinText() { winText.gameObject.SetActive(true); }
    void ShowLoseText() { loseText.gameObject.SetActive(true); }

    bool delay;
    IEnumerator DoValueChangeAnim(Transform origin, DeltaArgs args) {
        // wait for any recent anim to finish
        while (delay) {
            yield return null;
        }

        delay = true;
        
        ValueChangeAnimation v = Instantiate(valueDeltaAnim, origin).GetComponent<ValueChangeAnimation>();
        if (args.deltaValue < 0) {
            v.deltaText.text = args.deltaValue.ToString();
            v.deltaText.color = Color.red;
        } else {
            v.deltaText.text = "+" + args.deltaValue;
            v.deltaText.color = Color.green;
        }
        
        // small delay before next value change anim
        float timer = Time.time + 0.25f;
        while (Time.time < timer) {
            yield return null;
        }

        delay = false;
    }
}

public struct DeltaArgs {
    public int newValue;
    public int deltaValue;
}