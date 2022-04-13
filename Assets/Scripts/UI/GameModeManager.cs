using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public GameModeController controller;

    private void Update()
    {
        if (controller == null)
        {
            controller = GameObject.FindGameObjectWithTag("GameModeController").GetComponent<GameModeController>();
        }
    }

    public void PlavervAI()
    {
        if(controller != null)
        {
            controller.selectedMode = "PvAI";
        }
    }

    public void PlayervPlayer()
    {
        if (controller != null)
            controller.selectedMode = "PvP";
    }

    public void AIvAI()
    {
        if (controller != null)
            controller.selectedMode = "AIvAI";
    }
}
