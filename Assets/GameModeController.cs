using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeController : DontDestroyOnSelectedScenes
{
    public string selectedMode = "PvAI";

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        base.OnSceneLoaded(scene, mode);

        if (scene.name.Equals("MainScene"))
        {
            var manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
            if (manager != null)
            {
                Enum.TryParse(selectedMode, out manager.currGameMode);
            }
        }
    }
}
