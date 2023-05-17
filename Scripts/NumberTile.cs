using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberTile : Tile
{
    //public Sprite clickedSprite;

    public override short Count
    {
        get { return count; }
        set 
        {
            if (value >= 0 && value <= 8)
            {
                count = value;
            }
        }
    }

    protected override void OnMouseOver()
    {
        base.OnMouseOver();

        if (Input.GetMouseButtonDown(0) && visible && Count > 0)
        {
            SendMessageUpwards("ShowSurrounding", pos);
        }
    }

    public override bool Show()
    {
        if (!marked)
        {
            GetComponent<SpriteRenderer>().sprite = changeTo;
            if (Count != 0)
                GetComponentInChildren<TextMeshPro>().text = Count.ToString();
            visible = true;
            return Count == 0;
        }
        return false;
    }

    public override void ShowRaw()
    {
        base.ShowRaw();
        if (Count != 0)
            GetComponentInChildren<TextMeshPro>().text = Count.ToString();
    }

    void Start()
    {
        GetComponentInChildren<TextMeshPro>().text = "";
    }
}
