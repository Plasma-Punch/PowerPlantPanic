using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    [HideInInspector]
    public bool SpawnedIn;
    private void Start()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0) && !SpawnedIn || SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(SceneManager.sceneCountInBuildSettings  -2) && !SpawnedIn)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        SpawnedIn = false;
    }

    public void GameLost(Component sender, object obj)
    {
        SceneManager.LoadSceneAsync(4, LoadSceneMode.Single);
    }

    public void LoadGameplayscene(Component sender, object obj)
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}
