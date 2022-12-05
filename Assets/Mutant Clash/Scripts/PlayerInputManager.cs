using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public float unitDisplayTime;
    public float placementDisplayTime;

    public TimerBar timerDisplay;

    public Vector2[] lanePositions;

    public Color playerColour;

    public bool moveLeft = false;
    public bool useBCI = true;

    public float startingOffset;

    UnitSelector unitSelector;
    UnitPlacer unitPlacer;

    P300Controller bciController;
    BattleManager battleManager;

    float battleTimer;

    GameObject selectedUnitPrefab;
    int selectedLane;
    float timer;

    public enum State
    {
        ShowingUnits,
        SelectingUnits,
        ShowingPlacements,
        SelectingPlacement,
        Battle
    }
    public State state;


    private void OnDrawGizmos()
    {
        foreach (Vector2 pos in lanePositions)
        {
            Gizmos.DrawRay(pos, moveLeft ? Vector3.left : Vector3.right);
        }
    }

    void Start()
    {
        bciController = FindObjectOfType<P300Controller>();
        battleManager = FindObjectOfType<BattleManager>();

        unitSelector = GetComponentInChildren<UnitSelector>();
        unitSelector.Init();
        unitSelector.SetUnitColour(playerColour);
        unitSelector.onComplete = OnUnitSelected;

        unitPlacer = GetComponentInChildren<UnitPlacer>();
        unitPlacer.Init();
        unitPlacer.onComplete = OnPlacementSelected;

        battleTimer = startingOffset;

        if (battleTimer <= 0)
            ShowUnits();
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                switch (state)
                {
                    case State.ShowingUnits:
                        StartUnitSelection();
                        break;
                    case State.ShowingPlacements:
                        StartPlacementSelection();
                        break;
                }
            }
        }
        else if (BattleManager.active && battleTimer > 0)
        {
            // do battle stuff
            battleTimer -= Time.deltaTime;

            if(battleTimer <= 0)
            {
                ShowUnits();
            }
        }
    }

    void StartBattle()
    {
        // spawn unit
        UnitBehavior unitInstance = Instantiate(selectedUnitPrefab).GetComponent<UnitBehavior>();
        unitInstance.transform.position = lanePositions[selectedLane];
        battleTimer += unitInstance.stats.cost * 2;
        unitInstance.Init(moveLeft, playerColour);

        // unpause
        state = State.Battle;
        battleManager.AddUnitToLane(unitInstance, selectedLane);
    }

    void OnUnitSelected(GameObject unitPrefab)
    {
        selectedUnitPrefab = unitPrefab;

        ShowPlacements();
    }

    void OnPlacementSelected(int laneIndex)
    {
        selectedLane = laneIndex;

        StartBattle();
    }

    void ShowUnits()
    {
        state = State.ShowingUnits;
        BattleManager.active = false;
        timer = unitDisplayTime;

        // pause unit actions

        // show units and timer
        timerDisplay.StartTimer(unitDisplayTime);
        unitSelector.SetActive(true);
    }

    void StartUnitSelection()
    {
        state = State.SelectingUnits;

        if (useBCI)
        {
            unitSelector.TurnOffAll();
            bciController.StartStopStimulus();
        }
    }

    void ShowPlacements()
    {
        state = State.ShowingPlacements;
        timer = placementDisplayTime;

        // show placements and timer
        timerDisplay.StartTimer(placementDisplayTime);
        unitPlacer.SetActive(true);
    }

    void StartPlacementSelection()
    {
        state = State.SelectingPlacement;

        if (useBCI)
        {
            unitPlacer.TurnOffAll();
            bciController.StartStopStimulus();
        }
    }
}
