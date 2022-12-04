using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPOSelectionManager : MonoBehaviour
{
    public System.Action onComplete;

    protected CallbackSPO[] selectableChildren;

    public void Init()
    {
        selectableChildren = GetComponentsInChildren<CallbackSPO>();
        foreach (CallbackSPO spo in selectableChildren)
            spo.callback = OnSelection;

        SetActive(false);
    }

    public void SetActive(bool active)
    {
        print($"setting spo selection manager {gameObject.name} active: " + active);
        gameObject.SetActive(active);
        foreach (CallbackSPO spo in selectableChildren)
        {
            spo.includeMe = active;
            if (active)
                spo.Display();
        }
    }

    public void TurnOffAll()
    {
        foreach (CallbackSPO spo in selectableChildren)
            spo.TurnOff();
    }

    protected virtual void OnSelection()
    {
        SetActive(false);
        onComplete();
    }
}
