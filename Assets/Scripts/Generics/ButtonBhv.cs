using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBhv : MonoBehaviour
{
    public delegate void BeginActionDelegate();
    public BeginActionDelegate BeginAction;

    public delegate void DoActionDelegate();
    public DoActionDelegate DoAction;

    public delegate void EndActionDelegate();
    public EndActionDelegate EndAction;
}
