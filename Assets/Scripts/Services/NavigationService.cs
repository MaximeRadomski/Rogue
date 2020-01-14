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

    public static void LoadNextScene(string name)
    {
        _path += "/" + name;
        SceneManager.LoadScene(name);
    }

    public static void LoadPreviousScene()
    {
        var lastSeparator = _path.LastIndexOf('/');
        if (string.IsNullOrEmpty(_path) || lastSeparator == 0)
        {
            Debug.Log("    [DEBUG]    Root");
            return;
        }
        _path = _path.Substring(0, lastSeparator);
        lastSeparator = _path.LastIndexOf('/');
        var previousScene = _path.Substring(lastSeparator + 1);
        SceneManager.LoadScene(previousScene);
    }

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
