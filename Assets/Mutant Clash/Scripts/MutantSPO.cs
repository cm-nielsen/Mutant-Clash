using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantSPO : CallbackSPO
{
    public float selectScale = 2;
    //public Color deselectColour = Color.black;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float TurnOn()
    {
        transform.localScale = Vector3.one * selectScale;
        spriteRenderer.color = Color.white;

        return base.TurnOn();
    }

    public override void TurnOff()
    {
        transform.localScale = Vector3.one;
        //spriteRenderer.color = deselectColour;
        base.TurnOff();
    }

    public override void OnSelection()
    {
        base.OnSelection();
        print("I was selected! My index is: " + myIndex);
    }
}
