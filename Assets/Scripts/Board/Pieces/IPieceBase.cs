using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPieceBase
{
    public int CurrRowPos { get; set; }

    public int CurrColPos { get; set; }

    public int PieceID { get; set; }
}
