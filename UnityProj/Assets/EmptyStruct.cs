using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public struct EmptyStruct
{
    public void Print()
    {
        Debug.Log(this);
    }
}