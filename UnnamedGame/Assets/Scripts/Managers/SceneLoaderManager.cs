using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager instance;
    [SerializeField] private GameObject loaderCanvas;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loaderCanvas.SetActive(true);

        do {
            await Task.Yield();
        } while (scene.progress < 0.9f);
        loaderCanvas.SetActive(false);
        scene.allowSceneActivation = true;
        await Task.Delay(100);
    }
}
