using UnityEngine;

[CreateAssetMenu(menuName = "EventChannel")]
public class SO_EventChannel : ScriptableObject {
    public delegate void Callback();
    public delegate void CallbackInt(int n);
    public delegate void CallbackVector2Int(Vector2Int n);

    public Callback OnEvent;
    public CallbackInt OnEventInt;
    public CallbackVector2Int OnEventVector2Int;

    public void Raise() { OnEvent?.Invoke(); }
    public void Raise(int n) { OnEventInt?.Invoke(n); }
    public void Raise(Vector2Int v) { OnEventVector2Int?.Invoke(v); }
}