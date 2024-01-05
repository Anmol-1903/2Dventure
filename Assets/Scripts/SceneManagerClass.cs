using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class SceneManagerClass : MonoBehaviour
{
    public static SceneManagerClass instance;

    GameObject _loadingScreen;
    Animator[] svg;
    Slider _progressBar;
    int buildIndex;
    bool pressedAnyButton;
    float progress;


    [SerializeField] TextMeshProUGUI _loadingText;
    [SerializeField] float _loadTime = 5f;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than 1 SceneManager in the scene");
        }
        instance = this;

        _loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
        buildIndex = SceneManager.GetActiveScene().buildIndex;
        _loadingText = _loadingScreen.GetComponentInChildren<TextMeshProUGUI>();
        svg = _loadingScreen.GetComponentsInChildren<Animator>();
        _progressBar = _loadingScreen.transform.GetComponentInChildren<Slider>();
        pressedAnyButton = false;
        for (int i = 0; i < svg.Length; i++)
        {
            svg[i].gameObject.SetActive(false);
        }
        _loadingScreen.SetActive(false);
    }
    public void ButtonPressed()
    {
        pressedAnyButton = true;
    }
    public bool IsLoading()
    {
        if (progress > .9f)
        {
            return true;
        }
        return false;
    }

    public void LoadNewScene()
    {
        if (buildIndex == 0)
        {
            //From Main menu to ___ level
        }
        else
        {
            int temp = Random.Range(0, svg.Length);
            _loadingScreen.SetActive(true);
            svg[temp].gameObject.SetActive(true);
            StartCoroutine(LoadAsync(0));
        }
    }
    IEnumerator LoadAsync(int lvl)
    {
        float startTime = Time.time;
        /*yield return new WaitForSeconds(_loadTime);*/

        AsyncOperation operation = SceneManager.LoadSceneAsync(lvl);
        operation.allowSceneActivation = false;
        // Ensure that the progress bar is not null
        if (_progressBar != null)
        {
            while (!operation.isDone)
            {
                progress = Mathf.Clamp01(operation.progress / 0.9f);
                _progressBar.value = progress;
                if (operation.progress >= .9f)
                {
                    _loadingText.text = "Shoot To Continue ...";
                    if (pressedAnyButton)
                    {
                        operation.allowSceneActivation = true;
                    }
                }
                yield return null;
            }
        }
    }
}