﻿using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneBhv : MonoBehaviour
{
    public bool Paused;
    public PauseMenuBhv PauseMenu;

    protected virtual void SetPrivates()
    {
        Application.targetFrameRate = 60;
        NavigationService.SetCurrentRootScene(SceneManager.GetActiveScene().name);
    }

    public virtual void Pause()
    {
        if (PauseMenu == null)
            return;
        Paused = true;
        PauseMenu.Pause();
    }

    public virtual void Resume()
    {
        if (PauseMenu == null)
            return;
        Paused = false;
        PauseMenu.UnPause();
    }
}
