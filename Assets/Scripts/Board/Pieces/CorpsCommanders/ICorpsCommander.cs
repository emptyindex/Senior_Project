
using UnityEngine;

public interface ICorpsCommander
{
    public GameObject MenuCanvas { get; set; } 

    public bool HasTakenCommand { get; set; }
}
