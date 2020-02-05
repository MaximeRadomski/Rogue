using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NavigationService
{
    private static string _path;

    public static void SetCurrentRootScene(string name)
    {
        if (string.IsNullOrEmpty(_path))
            _path = "/" + name;
    }

    public static void NewRootScene(string name)
    {
        _path = "";
        LoadNextScene(name);
    }

    public static void LoadNextScene(string name)
    {
        _path += "/" + name;
        Debug.Log("    [DEBUG]    Path = " + _path);
        SceneManager.LoadScene(name);
    }

    public static void LoadPreviousScene(string onRootPreviousScene = null)
    {
        var lastSeparator = _path.LastIndexOf('/');
        if (string.IsNullOrEmpty(_path) || lastSeparator == 0)
        {
            if (onRootPreviousScene != null)
                NewRootScene(onRootPreviousScene);
            else
                Debug.Log("    [DEBUG]    Root");
            return;
        }
        _path = _path.Substring(0, lastSeparator);
        lastSeparator = _path.LastIndexOf('/');
        var previousScene = _path.Substring(lastSeparator + 1);
        Debug.Log("    [DEBUG]    Path = " + _path);
        SceneManager.LoadScene(previousScene);
    }

    public static void ReloadScene()
    {
        Debug.Log("    [DEBUG]    Path = " + _path);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
