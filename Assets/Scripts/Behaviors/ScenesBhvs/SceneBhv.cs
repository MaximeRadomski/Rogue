using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneBhv : MonoBehaviour
{
    public Soul Soul;
    public Journey Journey;
    public bool Paused;
    public PauseMenuBhv PauseMenu;
    public Instantiator Instantiator;
    public string OnRootPreviousScene = null;
    public bool CanGoPreviousScene = true;

    protected virtual void SetPrivates()
    {
        Application.targetFrameRate = 60;
        NavigationService.TrySetCurrentRootScene(SceneManager.GetActiveScene().name);
        Instantiator = GetComponent<Instantiator>();
        Soul = PlayerPrefsHelper.GetSoul();
        Journey = PlayerPrefsHelper.GetJourney();
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

    protected void GiveUp()
    {
        Instantiator.NewPopupYesNo(Constants.YesNoTitle,
            "You wont be able to recover your progress if you give up now!"
            , Constants.Cancel, Constants.Proceed, OnAcceptGiveUp);

        object OnAcceptGiveUp(bool result)
        {
            if (result)
            {
                Camera.main.gameObject.GetComponent<CameraBhv>().Unfocus();
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "GAME OVER", 10.0f, TransitionGiveUp, reverse: true);
                object TransitionGiveUp(bool transResult)
                {
                    NavigationService.NewRootScene(Constants.SoulTreeScene);
                    return transResult;
                }
            }
            return result;
        }
    }

    protected virtual void Settings()
    {
        
    }

    protected virtual void Exit()
    {
        Instantiator.NewPopupYesNo(Constants.YesNoTitle,
            "Are you sure you want to quit the game?"
            , Constants.Cancel, Constants.Proceed, OnAcceptExit);

        object OnAcceptExit(bool result)
        {
            if (result)
            {
                PlayerPrefsHelper.SaveSoul(Soul);
                Application.Quit();
            }
            return result;
        }
    }

    public virtual void OnPlayerDeath(CharacterBhv playerBhv)
    {
        playerBhv.Character.IsDead = true;
        StartCoroutine(Helper.ExecuteAfterDelay(1.0f, () =>
        {
            Defeat(playerBhv.Character);
            return true;
        }));
    }

    private void Defeat(Character character)
    {
        Constants.InputLocked = true;
        Instantiator.NewOverTitle(string.Empty, "Sprites/MapTitle_3", AfterDefeat, Direction.Down);
        object AfterDefeat(bool result)
        {
            StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
            {
                Soul.Xp = character.TotalExperience + Soul.XpKept;
                Soul.XpKept = 0;
                PlayerPrefsHelper.SaveSoul(Soul);
                Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "SOUL TREE", 4.0f, TransitionDefeat, reverse: true);
                object TransitionDefeat(bool transResult)
                {
                    NavigationService.NewRootScene(Constants.SoulTreeScene);
                    Constants.InputLocked = false;
                    return transResult;
                }
                return true;
            }));
            return result;
        }
    }
}
