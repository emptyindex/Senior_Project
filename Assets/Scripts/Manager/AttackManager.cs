using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public delegate void AttackRollNeededEvent();
    public event AttackRollNeededEvent AttackRollNeeded;
    public void InvokeAttackRoll()
    {
        AttackRollNeeded.Invoke();
    }
}
