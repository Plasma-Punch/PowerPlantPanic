using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _stateManager;
    private int _scenes;
    private int _sceneToLoadAtStart = 1;
    private int _selectedTutorialScene = 2;
    private bool _runTutorial;

    private void Start()
    {
        _scenes = SceneManager.sceneCountInBuildSettings;
    }

    public void StartGame() //Next scene is the main game
    {
        if (!_runTutorial)
        {
            GameObject obj = Instantiate(_stateManager);
            obj.GetComponent<GameStateManager>().SpawnedIn = true;
        }
            SceneManager.LoadSceneAsync(_sceneToLoadAtStart, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Credits()
    {
        SceneManager.LoadSceneAsync(_scenes -1, LoadSceneMode.Single);
    }

    public void StartMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public void SetTutorial(int index)
    {
        _selectedTutorialScene = 1 + 1 + index;
        if(_runTutorial) _sceneToLoadAtStart = _selectedTutorialScene;
    }
    public void SetRunTutorial(bool state)
    {
        if (state) _sceneToLoadAtStart = _selectedTutorialScene;
        else _sceneToLoadAtStart = 1;
        _runTutorial = state;
    }
}
