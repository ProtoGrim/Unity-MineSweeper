using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MineTile : Tile
{
    private void Awake()
    {
        NumberTile comp;
        if (TryGetComponent<NumberTile>(out comp))
        {
            Destroy(comp);
        }
    }

    public override short Count
    {
        get { return count; }
        set { }
    }


    public override bool Show()
    {
        if (!marked)
        {
            SendMessageUpwards("GameOver", false);
        }
        return false;
    }

    // Update is called once per frame
    void Start()
    {
        count = -1;
        GetComponentInChildren<TextMeshPro>().text = "";
    }
}
