using System;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType {
    Standard                = 0, // first in enemy row
    Self                    = 1,
    Forward                 = 2, // ally only
    Backward                = 3, // ally only
    Above                   = 4, // ally only
    Below                   = 5, // ally only
    Adjacent                = 6, // ally only
    Corners                 = 7, // ally only
    Stacked                 = 8, // above + below, ally only
    Allies                  = 10,
    AlliesOfGroup           = 11,
    Enemies                 = 20,
    EnemiesOfGroup          = 21,
    Random                  = 30,
    RandomAlly              = 31,
    RandomAllyAdjacent      = 32,
    RandomAllyAdjacentEmpty = 33,
    RandomEnemy             = 34,
    Selected                = 40,
    FrontThree              = 50,
    Sweep                   = 51, // Standard + above target + below target
    AllInRow                = 52,
    AllInEnemyRow           = 53,
    EmptyInAdjacentAlly     = 60,
}

public static class TargetTypes {
    public static List<Animal> GetTargets(TargetType targetType, CombatSlot origin, Group targetGroup = Group.None) {
        SlotGrid combatGrid = origin.SlotGrid;
        List<Animal> targets = new List<Animal>();
        bool enemyCalled = origin.Animal.isEnemy;

        CombatSlot target;
        Animal targetAnimal;
        switch (targetType) {
            case TargetType.Self:
                targets.Add(origin.Animal);
                return targets;
            case TargetType.Standard:
                targetAnimal = SelectStandard(combatGrid, origin);
                if (targetAnimal) targets.Add(targetAnimal);
                return targets;
            case TargetType.Forward:
                target = combatGrid.Forward(origin, enemyCalled) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                return targets;
            case TargetType.Backward:
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(-1, 0)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                return targets;
            case TargetType.Above:
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                return targets;
            case TargetType.Below:
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                return targets;
            case TargetType.Adjacent:
                return SelectTargetsAdjacent(combatGrid, origin, true);
            case TargetType.Corners:
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(1, 1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(-1, 1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(1, -1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(-1, -1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                return targets;
            case TargetType.Stacked:
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, 1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
                target = combatGrid.SelectSlotRelative(origin, enemyCalled, new Vector2Int(0, -1)) as CombatSlot;
                if (target && CheckTeam(origin, target, true)) targets.Add(target.Animal);
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
            case TargetType.Selected:
                Debug.Log("Target type not supported: Selected");
                break;
            case TargetType.FrontThree:
                break;
            case TargetType.Sweep: {
                Animal focus = SelectStandard(combatGrid, origin);

                for (int y = -1; y <= 1; y++) {
                    target =
                        combatGrid.SelectSlotRelative(focus.mSlot, false, new Vector2Int(focus.mSlot.x, y)) as
                            CombatSlot;
                    if (target && !target.IsEmpty()) targets.Add(target.Animal);
                }

                return targets;
            }
            case TargetType.AllInRow:
                for (int x = 0; x <= 3; x++) {
                    target = combatGrid.SelectSlot(new Vector2Int(x, origin.y), origin.Animal.isEnemy) as CombatSlot;
                    if (target && !target.IsEmpty()) targets.Add(target.Animal);
                }

                return targets;
            case TargetType.AllInEnemyRow:
                for (int x = 2; x <= 3; x++) {
                    target = combatGrid.SelectSlot(new Vector2Int(x, origin.y), origin.Animal.isEnemy) as CombatSlot;
                    if (target && !target.IsEmpty()) targets.Add(target.Animal);
                }

                return targets;
            case TargetType.EmptyInAdjacentAlly:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
        }

        Debug.LogError("TargetType out of range, did you forget to add case for a new TargetType?");
        return targets;
    }

    static Animal SelectStandard(SlotGrid combatGrid, CombatSlot origin) {
        for (int x = 2; x <= 3; x++) {
            CombatSlot target = combatGrid.SelectSlot(new Vector2Int(x, origin.y), origin.Animal.isEnemy) as CombatSlot;
            if (target && !target.IsEmpty()) {
                return target.Animal;
            }
        }

        return null;
    }

    static List<Animal> SelectTargetsOfTeam(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam,
        Group targetGroup = Group.None) {
        List<Animal> targets = new List<Animal>();
        foreach (CombatSlot slot in combatGrid.slotGrid) {
            if (!slot.IsEmpty() && CheckTeam(origin.Animal, slot.Animal, targetSameTeam) &&
                ((targetGroup == Group.None) || slot.Animal.animalData.group == targetGroup)) {
                targets.Add(slot.Animal);
            }
        }

        return targets;
    }

    static List<Animal> SelectTargetsAdjacent(SlotGrid combatGrid, CombatSlot origin, bool targetSameTeam) {
        List<Animal> targets = new List<Animal>();
        CombatSlot slot;

        slot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(1, 0)) as CombatSlot;
        if (slot && !slot.IsEmpty() && CheckTeam(origin.Animal, slot.Animal, targetSameTeam)) targets.Add(slot.Animal);
        slot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(-1, 0)) as CombatSlot;
        if (slot && !slot.IsEmpty() && CheckTeam(origin.Animal, slot.Animal, targetSameTeam)) targets.Add(slot.Animal);
        slot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(0, 1)) as CombatSlot;
        if (slot && !slot.IsEmpty() && CheckTeam(origin.Animal, slot.Animal, targetSameTeam)) targets.Add(slot.Animal);
        slot = combatGrid.SelectSlotRelative(origin, origin.Animal.isEnemy, new Vector2Int(0, -1)) as CombatSlot;
        if (slot && !slot.IsEmpty() && CheckTeam(origin.Animal, slot.Animal, targetSameTeam)) targets.Add(slot.Animal);
        return targets;
    }

    // CheckTeam returns true if Animal inputs are on same (or opposite w/ !sameTeam) teams. Safely takes empty CombatSlots.
    static bool CheckTeam(CombatSlot s1, CombatSlot s2, bool sameTeam) {
        if (!s1 && s1.IsEmpty() || !s2 && s2.IsEmpty()) return false;
        return CheckTeam(s1.Animal, s2.Animal, sameTeam);
    }
    static bool CheckTeam(Animal a1, Animal a2, bool sameTeam) {
        return (sameTeam && a1.isEnemy == a2.isEnemy) || (!sameTeam && a1.isEnemy != a2.isEnemy);
    }
}