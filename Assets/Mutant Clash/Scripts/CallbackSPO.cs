using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallbackSPO : SPO
{
    public System.Action callback;

    private void Awake()
    {
        gameObject.tag = "BCI";

        Button button = GetComponent<Button>();
        if(button == null)
            button = GetComponentInChildren<Button>();
        if (button != null)
        {
            print("button found, adding on click");
            button.onClick.AddListener(OnSelection);
        }
    }

    public virtual void Display()
    {
        TurnOn();
        transform.localScale = Vector3.one;
    }

    public override void OnSelection()
    {
        callback();
    }
}
