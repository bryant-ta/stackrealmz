using UnityEngine;

// Realtime duration of combat ticker = endTick * CombatManager.realTimeTickDuration
public class CombatTicker {
    public int EndTick => endTick;
    int endTick;
    public int StartTick => startTick;
    int startTick;
    public int CurTick => curTick;
    int curTick;

    GameObject ownerObj;
    EventName tickEvent;
    EventName tickerEndEvent;

    public CombatTicker(GameObject ownerObj, EventName tickEvent, EventName tickerEndEvent, int endTick, int startTick = 0) {
        this.ownerObj = ownerObj;
        this.tickEvent = tickEvent;
        this.tickerEndEvent = tickerEndEvent;
        
        this.endTick = endTick;
        this.startTick = startTick;
        curTick = startTick;
        
        Start();
    }

    public void Tick(int n = 1) {
        curTick += n;
        if (curTick >= endTick) {
            EventManager.TriggerEvent(ownerObj, tickerEndEvent);
            Reset();
            return;
        }

        CombatTickerArgs args = new CombatTickerArgs
            {endTick = this.endTick, startTick = this.startTick, curTick = this.curTick};
        EventManager.TriggerEvent(ownerObj, tickEvent, args);
    }
    public void OneTick() { // Required for adding listener to CombatManager, matching delegate signature
        Tick();
    }
    public void UnTick(int n = 1) {
        curTick -= n;
        if (curTick <= 0) {     // minimum for all ticker = 0
            curTick = 0;
        }
        CombatTickerArgs args = new CombatTickerArgs
            {endTick = this.endTick, startTick = this.startTick, curTick = this.curTick};
        EventManager.TriggerEvent(ownerObj, tickEvent, args);
    }

    public void SetEndTick(int n) {
        if (n < 1) {
            Debug.LogError("Cannot set endTick < 1, endTick unchanged");
            return;
        }

        endTick = n;
        if (endTick <= startTick) {
            Debug.LogWarning("Cannot set endTick <= startTick, setting startTick = 0");
            startTick = 0;
        }
        if (endTick < curTick) {
            Tick();             // force update ticker now
        }
    }
    public void SetStartTick(int n) {
        if (n < 0) { 
            Debug.LogError("Cannot set startTick < 0, startTick unchanged");
            return; 
        } else if (n >= endTick) {
            Debug.LogWarning("Cannot set startTick >= endTick, startTick unchanged");
            return;
        }
        
        startTick = n;
    }

    public void Start() { CombatManager.onTick.AddListener(OneTick); }
    public void Pause() { CombatManager.onTick.RemoveListener(OneTick); }

    public void Reset() {
        curTick = startTick;
        CombatTickerArgs args = new CombatTickerArgs
            {endTick = this.endTick, startTick = this.startTick, curTick = this.curTick};
        EventManager.TriggerEvent(ownerObj, tickEvent, args);
    }
}

public struct CombatTickerArgs {
    public int endTick;
    public int startTick;
    public int curTick;
}
