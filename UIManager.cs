using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UIManager singleton handles global UI elements
// Load Order: before GameManager ... Idk why this works?
public class UIManager : MonoBehaviour
{
    static UIManager _instance;
    
    public TextMeshProUGUI moneyText;
    public Image waveProgressFill;

    public GameManager gameMngr;
    public WaveManager waveMngr;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }

    void Start() {
        EventManager.Subscribe<int>(gameMngr.gameObject, EventID.ModifyMoney, UpdateMoneyText);
        EventManager.Subscribe<CombatTickerArgs>(waveMngr.gameObject, EventID.WaveTick, UpdateWaveProgressBar);
    }

    void UpdateMoneyText(int val) {
        moneyText.text = val.ToString();
    }

    void UpdateWaveProgressBar(CombatTickerArgs args) {
        waveProgressFill.fillAmount = (float) args.curTick / args.endTick;
    }
}
