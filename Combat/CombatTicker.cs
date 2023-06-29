using UnityEngine;

// Realtime duration of combat ticker = endTick * CombatClock.realTimeTickDuration
public class CombatTicker {
    public int EndTick => endTick;
    int endTick;
    public int StartTick => startTick;
    int startTick;
    public int CurTick => curTick;
    int curTick;

    GameObject ownerObj;
    EventID tickEvent;
    EventID tickerEndEvent;

    bool autoReset;

    public CombatTicker(GameObject ownerObj, EventID tickEvent, EventID tickerEndEvent, int endTick, bool autoReset = true, int startTick = 0) {
        this.ownerObj = ownerObj;
        this.tickEvent = tickEvent;
        this.tickerEndEvent = tickerEndEvent;
        
        this.endTick = endTick;
        this.startTick = startTick;
        curTick = startTick;

        this.autoReset = autoReset;
        
        Start();
    }

    public void Tick(int n = 1) {
        if (curTick >= endTick) {
            curTick = endTick;
            
            if (autoReset) {
                Reset();
            } else {
                Pause();    // if no autoreset, wait for manual reset at full timer 
            }
            
            // if (ownerObj == GameObject.Find("CombatManager")) Debug.Log("invoked");
            EventManager.Invoke(ownerObj, tickerEndEvent);
            
            return; 
        }
        curTick += n;
        // if (ownerObj == GameObject.Find("CombatManager")) Debug.Log(curTick);

        CombatTickerArgs args = new CombatTickerArgs
            {endTick = this.endTick, startTick = this.startTick, curTick = this.curTick};
        EventManager.Invoke(ownerObj, tickEvent, args);
    }
    public void OneTick() { // Required for adding listener to CombatClock, matching delegate signature
        Tick();
    }
    public void UnTick(int n = 1) {
        curTick -= n;
        if (curTick <= 0) {     // minimum for all ticker = 0
            curTick = 0;
        }
        CombatTickerArgs args = new CombatTickerArgs
            {endTick = this.endTick, startTick = this.startTick, curTick = this.curTick};
        EventManager.Invoke(ownerObj, tickEvent, args);
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

    public bool Ready() { return curTick == endTick; }
    
    public void Start() { 
        // if (ownerObj == GameObject.Find("CombatManager")) Debug.Log("started");
        CombatClock.onTick.AddListener(OneTick); }
    public void Pause() { 
        // if (ownerObj == GameObject.Find("CombatManager")) Debug.Log("paused");
        CombatClock.onTick.RemoveListener(OneTick); }
    public void Stop() {
        Reset();
        Pause();
    }

    public void Reset() {
        // if (ownerObj == GameObject.Find("CombatManager")) Debug.Log("reset");
        curTick = startTick;
        CombatTickerArgs args = new CombatTickerArgs
            {endTick = this.endTick, startTick = this.startTick, curTick = this.curTick};
        EventManager.Invoke(ownerObj, tickEvent, args);
    }
}

public struct CombatTickerArgs {
    public int endTick;
    public int startTick;
    public int curTick;
}
