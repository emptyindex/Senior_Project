using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProtectionBoard
{
    public void UpdateProtectionMap(int row, int col, int[,] board);

    public void UpdateDangerMap(int row, int col, int[,] board);
}
