using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementSPO : CallbackSPO
{
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public override float TurnOn()
    {
        image.color = onColour;

        return Time.time;
    }

    public override void TurnOff()
    {
        image.color = offColour;
    }

    public override void Display()
    {

    }

    public override void OnSelection()
    {
        // place spawned unit
        GameLoop.selectedLane = myIndex;

        base.OnSelection();
    }
}
