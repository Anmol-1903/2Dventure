using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class SceneManagerClass : MonoBehaviour
{
    public static SceneManagerClass instance;

    AsyncOperation operation;
    Animator animator;
    GameObject _loadingScreen;
    Animator[] svg;
    Slider _progressBar;
    float progress;

    int _waitTime = 6;
    float _counter;


    [SerializeField] TextMeshProUGUI _loadingText;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 SceneManager in the scene");
        }
        instance = this;
        _loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
        animator = _loadingScreen.GetComponent<Animator>();
        _loadingText = _loadingScreen.GetComponentInChildren<TextMeshProUGUI>();
        svg = _loadingScreen.GetComponentsInChildren<Animator>();
        _progressBar = _loadingScreen.transform.GetComponentInChildren<Slider>();
        for (int i = 1; i < svg.Length; i++)
        {
            svg[i].gameObject.SetActive(false);
        }
        DontDestroyOnLoad(gameObject);
    }
    public bool IsLoading()
    {
        if (progress > .9f)
        {
            return true;
        }
        return false;
    }
    public void AllowLoading()
    {
        if (operation != null)
        {
            operation.allowSceneActivation = true;
        }
    }
    public void LoadNewScene(int _level)
    {
        int temp = Random.Range(1, svg.Length);
        _loadingScreen.SetActive(true);
        svg[temp].gameObject.SetActive(true);
        _counter = _waitTime;
        StartCoroutine(LoadAsync(_level));
    }
    IEnumerator LoadAsync(int lvl)
    {
        _loadingScreen.SetActive(true);
        animator?.SetTrigger("StartLoading");
        yield return new WaitForSeconds(1);
        operation = SceneManager.LoadSceneAsync(lvl);
        operation.allowSceneActivation = false;
        if (_progressBar != null)
        {
            while (!operation.isDone)
            {
                progress = Mathf.Clamp01(operation.progress / 0.9f);
                _progressBar.value = progress;
                if (operation.progress >= .9f)
                {
                    _loadingText.text = "Loading in ("+ ((int)_counter).ToString() +")";
                    _counter -= Time.deltaTime;
                    if (_counter <= 0)
                    {
                        animator?.SetTrigger("NewScene");
                        yield return new WaitForSeconds(2);
                        AllowLoading();
                    }
                }
                yield return null;
            }
        }
    }
}