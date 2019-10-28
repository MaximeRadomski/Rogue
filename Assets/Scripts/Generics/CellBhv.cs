using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBhv : MonoBehaviour
{
    public int X;
    public int Y;
    public CellType Type;
    public CellState State;

    private void Start()
    {
        if (Type == CellType.Off)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }

    public void ResetDisplay()
    {
        State = CellState.None;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void ShowPm()
    {
        State = CellState.Mouvement;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 1.0f, 0.8f, 1.0f);
    }

    public enum CellType
    {
        Off = 0,
        On = 1,
        Destructible = 2
    }

    public enum CellState
    {
        None = 0,
        Mouvement = 1,
        AttackRange = 2,
        AttackZone = 3
    }
}
