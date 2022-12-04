using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public float unitDisplayTime;
    public float placementDisplayTime;

    public TimerBar timerDisplay;

    public Vector2[] lanePositions;

    public Color playerColour;

    public bool useBCI = true;

    UnitSelector unitSelector;
    UnitPlacer unitPlacer;

    P300Controller bciController;

    [HideInInspector]
    public float battleTimer;

    public static GameObject selectedUnitPrefab;
    public static int selectedLane;
    float timer;

    public enum GameState
    {
        ShowingUnits,
        SelectingUnits,
        ShowingPlacements,
        SelectingPlacement,
        Battle
    }
    public GameState state;


    private void OnDrawGizmos()
    {
        foreach (Vector2 pos in lanePositions)
        {
            Gizmos.DrawRay(pos, Vector3.right);
        }
    }

    void Start()
    {
        bciController = FindObjectOfType<P300Controller>();

        unitSelector = GetComponentInChildren<UnitSelector>();
        unitSelector.Init();
        unitSelector.SetUnitColour(playerColour);
        unitSelector.onComplete = ShowPlacements;

        unitPlacer = GetComponentInChildren<UnitPlacer>();
        unitPlacer.Init();
        unitPlacer.onComplete = StartBattle;

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
                    case GameState.ShowingUnits:
                        StartUnitSelection();
                        break;
                    case GameState.ShowingPlacements:
                        StartPlacementSelection();
                        break;
                }
            }
        }
        else if (battleTimer > 0)
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
        battleTimer += unitInstance.stats.cost;
        unitInstance.Init(false, playerColour);

        // unpause
        state = GameState.Battle;

    }

    void ShowUnits()
    {
        state = GameState.ShowingUnits;
        timer = unitDisplayTime;
        timerDisplay.StartTimer(unitDisplayTime);

        // show units and timer
        unitSelector.SetActive(true);
    }

    void StartUnitSelection()
    {
        state = GameState.SelectingUnits;

        if (useBCI)
        {
            unitSelector.TurnOffAll();
            bciController.StartStopStimulus();
        }
    }

    void ShowPlacements()
    {
        print("Show placements called");
        state = GameState.ShowingPlacements;
        timer = placementDisplayTime;
        timerDisplay.StartTimer(placementDisplayTime);

        // show placements and timer
        unitPlacer.SetActive(true);
        print("end of show placements");
    }

    void StartPlacementSelection()
    {
        print("start placement selection called");
        state = GameState.SelectingPlacement;

        if (useBCI)
        {
            unitPlacer.TurnOffAll();
            bciController.StartStopStimulus();
        }

        print("end of start placement selection");
    }
}
