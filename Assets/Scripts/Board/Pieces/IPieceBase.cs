using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPieceBase
{
    public int ProtectionLevel { get; set; }
    public bool IsDead { get; set; }

    public int CurrRowPos { get; set; }

    public int CurrColPos { get; set; }

    public int PieceID { get; set; }

    public abstract bool IsAttackSuccessful(int PieceToAttack, int numberRolled);
}
