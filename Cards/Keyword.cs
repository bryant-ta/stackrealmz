using System;
using System.Collections.Generic;

[Serializable]
public enum Keyword {
    None = 0,
    Hidden = 1,
}

public static class KeywordToEffectLookUp
{
    static Dictionary<Keyword, Func<IEffect>> LookUp = new Dictionary<Keyword, Func<IEffect>>()
    {
        { Keyword.None, () => null },
        { Keyword.Hidden, () => new HiddenEffect() },
    };

    public static IEffect CreateEffect(Keyword keyword)
    {
        return LookUp[keyword]?.Invoke();
    }
}
