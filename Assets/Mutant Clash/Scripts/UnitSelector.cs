using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : SPOSelectionManager
{
    public void SetUnitColour(Color colour)
    {
        foreach (CallbackSPO spo in selectableChildren)
        {
            (spo as UnitCardSPO).SetUnitColour(colour);
        }
    }

    protected override void OnSelection()
    {
        base.OnSelection();
    }
}
