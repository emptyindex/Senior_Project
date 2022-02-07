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
    /// <summary>
    /// This determines whether it is this player's turn to move.
    /// </summary>
    /// <param name="newVal">The turn status.</param>
    public abstract void IsTurn(bool newVal);
}
