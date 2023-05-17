using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public Sprite changeTo;
    protected short count = 0;
    public Vector2 pos;
    protected bool visible = false;
    protected bool marked = false;

    public bool Visible
    {
        get { return visible; }
        set { }
    }
    public bool Marked
    {
        get { return marked; }
        set { }
    }

    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !marked)
        {
            SendMessageUpwards("TileClicked", pos);
        }
        else if (Input.GetMouseButtonDown(1) && !visible)
        {
            marked = !marked;
            SendMessageUpwards("MarkTile", pos);
        }

    }

    public abstract short Count
    {
        get;
        set;
    }

    public virtual bool Show()
    {
        return false;
    }

    public virtual void ShowRaw()
    {
        GetComponent<SpriteRenderer>().sprite = changeTo;
    }

   
}
