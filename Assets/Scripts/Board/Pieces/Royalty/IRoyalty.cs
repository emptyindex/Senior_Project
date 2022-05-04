using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoyalty
{
    public int[] InitialStartPos { get; }

    public bool HasMoved { get; set; }

    public bool CanMoveAgain(int[] newPos);

    public void ResetPos(int[] newPos);

    public void UpdateMovementNum(int[] newPos);

    public void ResetMovementNum();

    public int GetNumMoved(int[] newPos);
}
