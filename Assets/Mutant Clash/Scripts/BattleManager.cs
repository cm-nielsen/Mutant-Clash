using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using System.Linq;
using System;

public class BattleManager : MonoBehaviour
{
    public static bool active = true;

    public float collisionDistance = 1;

    List<UnitBehavior> lane1;
    List<UnitBehavior> lane2;
    List<UnitBehavior> lane3;
    List<Battle> battles;


    void Start()
    {
        lane1 = new List<UnitBehavior>();
        lane2 = new List<UnitBehavior>();
        lane3 = new List<UnitBehavior>();
        battles = new List<Battle>();
    }


    void Update()
    {
        if (!active)
            return;

        MoveUnits();

        UpdateBattles();
    }

    void MoveUnits()
    {
        MoveUnitsInLane(lane1);
        MoveUnitsInLane(lane2);
        MoveUnitsInLane(lane3);
    }

    void MoveUnitsInLane(List<UnitBehavior> lane)
    {
        Dictionary<UnitBehavior, UnitBehavior> unitCollisions = new Dictionary<UnitBehavior, UnitBehavior>();

        foreach (UnitBehavior unit in lane)
        {
            UnitBehavior collision = unit.MoveAndCollide(lane, collisionDistance);

            if (collision)
            {
                //print($"collision between {unit.name} and {collision.name}");
                if (unitCollisions.ContainsKey(collision) && unitCollisions[collision] == unit)
                {
                    bool battleExists = false;
                    foreach (Battle battle in battles)
                        battleExists |= battle.HasUnits(unit, collision);

                    // mutual collision, start battle
                    if (!battleExists)
                        battles.Add(new Battle(unit, collision, OnUnitDeath));
                }
                unitCollisions[unit] = collision;
            }
        }
    }

    void UpdateBattles()
    {
        List<Battle> completedBattles = new List<Battle>();

        foreach(Battle battle in battles)
        {
            if (battle.Update())
            {
                completedBattles.Add(battle);
            }
        }

        foreach (Battle battle in completedBattles)
            battles.Remove(battle);
    }

    void OnUnitDeath(UnitBehavior deadUnit)
    {
        lane1.Remove(deadUnit);
        lane2.Remove(deadUnit);
        lane3.Remove(deadUnit);
    }

    public void AddUnitToLane(UnitBehavior unit, int lane)
    {
        active = true;

        switch (lane)
        {
            case 0: lane1.Add(unit);break;
            case 1: lane2.Add(unit);break;
            case 2: lane3.Add(unit);break;
        }
    }

    [System.Serializable]
    public class Battle
    {
        const float WindupTime = 0.5f;
        const float PushTime = 0.5f;

        UnitBehavior unit1;
        UnitBehavior unit2;

        BattleState state;
        float timer;

        Action<UnitBehavior> OnUnitDeath;

        enum BattleState
        {
            unit1Windup,
            unit1Push,
            unit2Windup,
            unit2Push
        }

        public Battle(UnitBehavior unit1, UnitBehavior unit2, Action<UnitBehavior> removeUnit)
        {
            this.unit1 = unit1;
            this.unit2 = unit2;
            OnUnitDeath = removeUnit;

            state = BattleState.unit1Windup;

            if (unit2.stats.speed > unit1.stats.speed)
                state = BattleState.unit2Windup;

            ApplyState();
        }

        public bool HasUnits(UnitBehavior a, UnitBehavior b)
        {
            return (unit1 == a && unit2 == b) ||
                (unit2 == a && unit1 == b);
        }

        public bool Update()
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                if (++state > BattleState.unit2Push)
                    state = 0;

                return ApplyState();
            }
            return false;
        }

        bool ApplyState()
        {
            switch (state)
            {
                case BattleState.unit1Windup:
                    unit1.SetSprite(UnitSpriteState.Windup);
                    unit2.SetSprite(UnitSpriteState.Idle);
                    timer = WindupTime;

                    return false;

                case BattleState.unit1Push:
                    unit1.SetSprite(UnitSpriteState.Push);
                    unit2.SetSprite(UnitSpriteState.Sit);
                    timer = PushTime;

                    if(unit2.TakeDamage())
                    {
                        OnUnitDeath(unit2);
                        return true;
                    }
                    return false;

                case BattleState.unit2Windup:
                    unit2.SetSprite(UnitSpriteState.Windup);
                    unit1.SetSprite(UnitSpriteState.Idle);
                    timer = WindupTime;

                    return false;

                case BattleState.unit2Push:
                    unit2.SetSprite(UnitSpriteState.Push);
                    unit1.SetSprite(UnitSpriteState.Sit);
                    timer = PushTime;

                    if (unit1.TakeDamage())
                    {
                        OnUnitDeath(unit1);
                        return true;
                    }
                    return false;
            }
            return false;
        }
    }
}
