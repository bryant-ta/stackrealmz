using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TargetType {
    Standard                = 0, // first in enemy row
    Self                    = 1,
    AllyForward             = 2, // ally animal only
    AllyBackward            = 3, // ally animal only
    AllyAbove               = 4, // ally animal only
    AllyBelow               = 5, // ally animal only
    AllyAdjacent            = 6, // ally animal only
    AllyCorners             = 7, // ally animal only
    AllyStacked             = 8, // above + below, animal ally only
    Allies                  = 10,
    AlliesOfGroup           = 11,
    Enemies                 = 20,
    EnemiesOfGroup          = 21,
    Random                  = 30,
    RandomAlly              = 31,
    RandomAllyAdjacent      = 32,
    RandomAllyAdjacentEmpty = 33,
    RandomEnemy             = 34,
    SelectAlly              = 40,
    SelectEnemy             = 41,
    SelectGroupAlly         = 42,
    SelectAllySlot          = 43,
    SelectEnemySlot         = 44,
    FrontThree              = 50,
    Sweep                   = 51, // Standard + above target + below target
    AllInRow                = 52,
    AllInEnemyRow           = 53,
    Far                     = 60,  // enemy in last column
    EmptyForward            = 202, // empty ally slot only
    EmptyBackward           = 203, // empty ally slot only
    EmptyAbove              = 204, // empty ally slot only
    EmptyBelow              = 205, // empty ally slot only
    EmptyAdjacent           = 206, // empty ally slot only
    EmptyCorners            = 207, // empty ally slot only
    EmptyStacked            = 208, // above + below, empty ally slot only
    Forward                 = 212, // ally slot only
    Backward                = 213, // ally slot only
    Above                   = 214, // ally slot only
    Below                   = 215, // ally slot only
    Adjacent                = 216, // ally slot only
    Corners                 = 217, // ally slot only
    Stacked                 = 218, // above + below, ally slot only
}

public static class TargetTypes {
    // GetTargets returns a list of CombatSlots fitting TargetType.
    // - Requires target slots to be empty unless targetType contains "Empty"
    // - Returns empty list if no valid targets. SHOULD NEVER RETURN NULL.
    public static List<CombatSlot> GetTargets(TargetType targetType, CombatSlot origin, Group targetGroup = Group.None) {
        SlotGrid combatGrid = origin.SlotGrid;
        List<CombatSlot> targets = new List<CombatSlot>();
        bool enemyCalled = false;
        
        if (origin.Animal) enemyCalled = origin.Animal.isEnemy;

        CombatSlot targetSlot;
        switch (targetType) {
            case TargetType.Self:
                targets.Add(origin);
                return targets;
            case TargetType.Standard:
                targetSlot = SelectStandard(combatGrid, origin);
                if (targetSlot) targets.Add(targetSlot);
                return targets;
            case TargetType.AllyForward:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(1, 0)) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.AllyBackward:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(-1, 0)) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.AllyAbove:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.AllyBelow:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.AllyAdjacent:
                return SelectTargetsAdjacent(combatGrid, origin, true);
            case TargetType.AllyCorners:
                return SelectTargetsCorner(combatGrid, origin, true);
            case TargetType.AllyStacked:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Allies: {
                return SelectTargetsOfTeam(combatGrid, origin, true);
            }
            case TargetType.AlliesOfGroup: {
                return SelectTargetsOfTeam(combatGrid, origin, true, targetGroup);
            }
            case TargetType.Enemies: {
                return SelectTargetsOfTeam(combatGrid, origin, false);
            }
            case TargetType.EnemiesOfGroup: {
                return SelectTargetsOfTeam(combatGrid, origin, false, targetGroup);
            }
            case TargetType.Random:
                break;
            case TargetType.RandomAlly:
                break;
            case TargetType.RandomAllyAdjacent:
                break;
            case TargetType.RandomAllyAdjacentEmpty:
                break;
            case TargetType.RandomEnemy:
                break;
            case TargetType.SelectAlly:
                if (!origin.Animal.isEnemy) targets.Add(origin);
                return targets;
            case TargetType.SelectEnemy:
                if (origin.Animal.isEnemy) targets.Add(origin);
                return targets;
            case TargetType.SelectGroupAlly:
                if (!origin.Animal.isEnemy && origin.Animal.group == targetGroup) targets.Add(origin);
                return targets;
            case TargetType.SelectAllySlot:
                if (!origin.isEnemySlot) targets.Add(origin);
                return targets;
            case TargetType.SelectEnemySlot:
                if (origin.isEnemySlot) targets.Add(origin);
                return targets;
            case TargetType.FrontThree:
                break;
            case TargetType.Sweep: {
                CombatSlot focus = SelectStandard(combatGrid, origin);
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
                    targetSlot = combatGrid.SelectSlot(new Vector2Int(x, origin.y), origin.Animal.isEnemy) as CombatSlot;
                    if (targetSlot && !targetSlot.IsEmpty()) targets.Add(targetSlot);
                }

                return targets;
            case TargetType.AllInEnemyRow:
                for (int x = 2; x <= 3; x++) {
                    targetSlot = combatGrid.SelectSlot(new Vector2Int(x, origin.y), origin.Animal.isEnemy) as CombatSlot;
                    if (targetSlot && !targetSlot.IsEmpty()) targets.Add(targetSlot);
                }

                return targets;
            case TargetType.Far:
                targetSlot = combatGrid.SelectSlot(new Vector2Int(combatGrid.Width - 1, origin.y), enemyCalled) as CombatSlot;
                if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, false)) targets.Add(targetSlot);
                return targets;
            case TargetType.EmptyForward:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(1, 0)) as CombatSlot;
                if (targetSlot && targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.EmptyBackward:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(-1, 0)) as CombatSlot;
                if (targetSlot && targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.EmptyAbove:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.EmptyBelow:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.EmptyAdjacent:
                return SelectTargetsAdjacent(combatGrid, origin, true, true);
            case TargetType.EmptyCorners:
                return SelectTargetsCorner(combatGrid, origin, true, true);
            case TargetType.EmptyStacked:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Forward:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(1, 0)) as CombatSlot;
                if (targetSlot && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Backward:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(-1, 0)) as CombatSlot;
                if (targetSlot && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Above:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Below:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            case TargetType.Adjacent:
                return SelectTargetsAdjacent(combatGrid, origin, true, true).Concat(SelectTargetsAdjacent(combatGrid, origin, true, false)).ToList();
            case TargetType.Corners:
                return SelectTargetsCorner(combatGrid, origin, true, true).Concat(SelectTargetsCorner(combatGrid, origin, true, false)).ToList();
            case TargetType.Stacked:
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (targetSlot && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                targetSlot = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (targetSlot && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
                return targets;
            default:
                throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
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

    static List<CombatSlot> SelectTargetsOfTeam(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam,
        Group targetGroup = Group.None) {
        List<CombatSlot> targets = new List<CombatSlot>();
        foreach (CombatSlot slot in combatGrid.slotGrid) {
            if (!slot.IsEmpty() && CheckTeam(origin, slot, targetSameTeam) &&
                ((targetGroup == Group.None) || slot.Animal.group == targetGroup)) {
                targets.Add(slot);
            }
        }

        return targets;
    }

    static List<CombatSlot> SelectTargetsAdjacent(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam, bool targetEmpty = false) {
        List<CombatSlot> targets = new List<CombatSlot>();
        CombatSlot targetSlot;

        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(1, 0)) as CombatSlot;
        if (targetSlot && (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty()) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(-1, 0)) as CombatSlot;
        if (targetSlot && (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty()) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(0, 1)) as CombatSlot;
        if (targetSlot && (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty()) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(0, -1)) as CombatSlot;
        if (targetSlot && (targetEmpty ? targetSlot.IsEmpty() : !targetSlot.IsEmpty()) && CheckTeam(origin, targetSlot, targetSameTeam)) targets.Add(targetSlot);
        return targets;
    }

    static List<CombatSlot> SelectTargetsCorner(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam, bool targetEmpty = false) {
        List<CombatSlot> targets = new List<CombatSlot>();
        CombatSlot targetSlot;
        
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(1, 1)) as CombatSlot;
        if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(-1, 1)) as CombatSlot;
        if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(1, -1)) as CombatSlot;
        if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
        targetSlot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(-1, -1)) as CombatSlot;
        if (targetSlot && !targetSlot.IsEmpty() && CheckTeam(origin, targetSlot, true)) targets.Add(targetSlot);
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