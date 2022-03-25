using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Player that all players inherit from. 
/// This exists so the GameManager can communicate turn status
/// to both AI and Human players without knowing it's class.
/// </summary>
public abstract class BasePlayer : MonoBehaviour
{
    public bool canMove = false;

    public bool isGameOver = false;

    /* delegate void AttackRollNeededEvent();
    public event AttackRollNeededEvent AttackRollNeeded;*/

    public GameManager Manager { get; set; }

    public abstract void SetPieces(List<GameObject> pieces);

    /// <summary>
    /// This determines whether it is this player's turn to move.
    /// </summary>
    /// <param name="newVal">The turn status.</param>
    public void IsTurn(bool newVal)
    {
        canMove = newVal;
    }

    /*protected void InvokeAttackRoll()
    {
        AttackRollNeeded.Invoke();
    }*/
}
