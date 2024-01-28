using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class SceneManagerClass : MonoBehaviour
{
    public static SceneManagerClass instance;
    Animator animator;
    GameObject _loadingScreen;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 SceneManager in the scene");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        _loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
        animator = GetComponent<Animator>();
    }
    IEnumerator StartLoading(int lvl)
    {
        _loadingScreen.SetActive(true);
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(.5f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(lvl);
        while(!operation.isDone)
        {
            yield return null;
        }
        animator.SetTrigger("Stop");
    }
    public void LoadNewScene(int _level)
    {
        StartCoroutine(StartLoading(_level));
    }
    public void Deactivate()
    {
        _loadingScreen.SetActive(false);
    }
}