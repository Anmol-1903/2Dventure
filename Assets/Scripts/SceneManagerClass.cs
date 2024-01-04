using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class SceneManagerClass : MonoBehaviour
{
    public static SceneManagerClass instance;

    [SerializeField] int buildIndex;
    [SerializeField] GameObject _loadingScreen;
    [SerializeField] Animator[] svg;
    [SerializeField] Slider _progressBar;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than 1 SceneManager in the scene");
        }
        instance = this;

        buildIndex = SceneManager.GetActiveScene().buildIndex;
        _loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
        svg = _loadingScreen.transform.GetComponentsInChildren<Animator>();
        for(int i = 0; i < svg.Length; i++)
        {
            svg[i].gameObject.SetActive(false);
        }
    }

    private void LoadNewScene()
    {
        if(buildIndex == 0)
        {
            //main menu
        }
        else
        {
            LoadAsync(0);
            int temp = Random.Range(0, svg.Length);
            svg[temp].gameObject.SetActive(true);
        }
    }
    
    IEnumerator LoadAsync(int lvl)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(lvl);
        while (!operation.isDone)
        {
            _progressBar.value = operation.progress * .9f;
            yield return null;
        }
    }
}