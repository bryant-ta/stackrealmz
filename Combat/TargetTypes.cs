using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum TargetType {
    Standard                = 0, // first in enemy row
    Self                    = 1,
    Forward                 = 2,
    Backward                = 3,
    Above                   = 4,
    Below                   = 5,
    Adjacent                = 6,
    Corners                 = 7,
    Stacked                 = 8, // above + below origin
    Team                    = 10, // all cards of a team
    Random                  = 30,
    RandomAdjacent          = 31,
    Select                  = 40, // selection, only used by player (Spells)
    FrontThree              = 50,
    Sweep                   = 51, // Standard + above target + below target
    AllInRow                = 52,
    AllInEnemyRow           = 53,
    Far                     = 60, // enemy in last column
}

[Serializable]
public struct TargetArgs {
    public TargetType targetType;
    [HideInInspector] public CombatSlot originSlot;
    public TargetSlotState targetSlotState;
    public bool targetSameTeam;
    public Group targetGroup; // not used for most target types
}

public enum TargetSlotState {
    NonEmpty = 0,
    Empty = 1,
    Any = 2,
}

public static class TargetTypes {
    // GetTargets returns a list of CombatSlots fitting TargetType.
    // - Returns empty list if no valid targets. SHOULD NEVER RETURN NULL.
    public static List<CombatSlot> GetTargets(TargetArgs args) {
        bool targetEmpty = false;
        bool targetAny = false;
        switch (args.targetSlotState) {
            case TargetSlotState.Empty:
                targetEmpty = true;
                break;
            case TargetSlotState.Any:
                targetAny = true;
                break;
        }
        
        bool enemyCalled = false;
        if (args.originSlot.Animal) enemyCalled = args.originSlot.Animal.isEnemy;

        SlotGrid combatGrid = args.originSlot.SlotGrid;
        List<CombatSlot> targets = new List<CombatSlot>();
        CombatSlot targetSlot;
        
        switch (args.targetType) {
            case TargetType.Self:
                targets.Add(args.originSlot);
                return targets;
            case TargetType.Standard:
                targetSlot = SelectStandard(combatGrid, args.originSlot);
                if (targetSlot) targets.Add(targetSlot);
                return targets;
            case TargetType.Forward:
                targetSlot = combatGrid.SelectSlotRelative(args.originSlot, enemyCalled, new Vector2Int(1, 0)) as CombatSlot;
                if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(args.originSlot, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Backward:
                targetSlot = combatGrid.SelectSlotRelative(args.originSlot, enemyCalled, new Vector2Int(-1, 0)) as CombatSlot;
                if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(args.originSlot, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Above:
                targetSlot = combatGrid.SelectSlotRelative(args.originSlot, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(args.originSlot, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Below:
                targetSlot = combatGrid.SelectSlotRelative(args.originSlot, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(args.originSlot, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Adjacent:
                return SelectTargetsAdjacent(combatGrid, args.originSlot, args.targetSameTeam, targetEmpty, targetAny);
            case TargetType.Corners:
                return SelectTargetsCorner(combatGrid, args.originSlot, args.targetSameTeam, targetEmpty, targetAny);
            case TargetType.Stacked:
                targetSlot = combatGrid.SelectSlotRelative(args.originSlot, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(args.originSlot, targetSlot, true)) targets.Add(targetSlot);
                targetSlot = combatGrid.SelectSlotRelative(args.originSlot, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(args.originSlot, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Team: {
                return SelectTargetsOfTeam(combatGrid, args.originSlot, args.targetSameTeam, args.targetGroup);
            }
            case TargetType.Random:
                break;
            case TargetType.RandomAdjacent:
                break;
            case TargetType.Select: // only used by player (Spells)
                if ((targetAny || (targetEmpty ? args.originSlot.IsEmpty() : !args.originSlot.IsEmpty())) &&
                    (args.targetSameTeam ? !enemyCalled : enemyCalled) &&
                    (args.targetGroup == Group.None || (args.originSlot.Animal && args.originSlot.Animal.group == args.targetGroup)))  {
                    targets.Add(args.originSlot); 
                }
                return targets;
            case TargetType.FrontThree:
                break;
            case TargetType.Sweep: {
                CombatSlot focus = SelectStandard(combatGrid, args.originSlot);
                if (focus) {
                    for (int y = -1; y <= 1; y++) {
                        targetSlot = combatGrid.SelectSlotRelative(focus, false, new Vector2Int(focus.x, y)) as CombatSlot;
                        if (targetSlot && !targetSlot.IsEmpty()) targets.Add(targetSlot);
                    }
                }

                return targets;
            }
            case TargetType.AllInRow:
                for (int x = 0; x <= 3; x++) {
                    targetSlot = combatGrid.SelectSlot(new Vector2Int(x, args.originSlot.y), args.originSlot.Animal.isEnemy) as CombatSlot;
                    if (targetSlot && !targetSlot.IsEmpty()) targets.Add(targetSlot);
                }

                return targets;
            case TargetType.AllInEnemyRow:
                for (int x = 2; x <= 3; x++) {
                    targetSlot = combatGrid.SelectSlot(new Vector2Int(x, args.originSlot.y), args.originSlot.Animal.isEnemy) as CombatSlot;
                    if (targetSlot && !targetSlot.IsEmpty()) targets.Add(targetSlot);
                }

                return targets;
            case TargetType.Far:
                targetSlot = combatGrid.SelectSlot(new Vector2Int(combatGrid.Width - 1, args.originSlot.y), enemyCalled) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(args.originSlot, targetSlot, false)) targets.Add(targetSlot);
                return targets;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.targetType), args.targetType, null);
        }

        Debug.LogError("TargetType out of range, did you forget to add case for a new TargetType?");
        return targets;
    }

    // Note: ensure null return is handled
    static CombatSlot SelectStandard(SlotGrid combatGrid, CombatSlot origin) {
        for (int x = 2; x <= 3; x++) {
            CombatSlot target = combatGrid.SelectSlot(new Vector2Int(x, origin.y), origin.Animal.isEnemy) as CombatSlot;
            if (target && !target.IsEmpty()) {
                return target;
            }
        }

        return null;
    }

    static List<CombatSlot> SelectTargetsOfTeam(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam, Group targetGroup = Group.None) {
        List<CombatSlot> targets = new List<CombatSlot>();
        foreach (CombatSlot slot in combatGrid.slotGrid) {
            if (!slot.IsEmpty() && CheckTeam(origin, slot, targetSameTeam) &&
                ((targetGroup == Group.None) || slot.Animal.group == targetGroup)) {
                targets.Add(slot);
            }
        }

        return targets;
    }

    static List<CombatSlot> SelectTargetsAdjacent(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam, bool targetEmpty, bool targetAny) {
        List<CombatSlot> targets = new List<CombatSlot>();
        CombatSlot targetSlot;

        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(1, 0)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(-1, 0)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(0, 1)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(0, -1)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        return targets;
    }

    static List<CombatSlot> SelectTargetsCorner(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam, bool targetEmpty, bool targetAny) {
        List<CombatSlot> targets = new List<CombatSlot>();
        CombatSlot targetSlot;
        
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(1, 1)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(-1, 1)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(1, -1)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(-1, -1)) as CombatSlot;
        if (targetSlot && (targetAny || (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty())) && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
        return targets;
    }

    // CheckTeam returns true if Animal inputs are on same (or opposite w/ !sameTeam) teams. Safely takes empty CombatSlots.
    static bool CheckTeam(CombatSlot s1, CombatSlot s2, bool sameTeam) {
        return !s1 || (sameTeam && s1.isEnemySlot == s2.isEnemySlot) || !s2 || (!sameTeam && s1.isEnemySlot != s2.isEnemySlot);
    }
    // static bool CheckTeam(Animal a1, Animal a2, bool sameTeam) {
    //     return (sameTeam && a1.isEnemy == a2.isEnemy) || (!sameTeam && a1.isEnemy != a2.isEnemy);
    // }
}