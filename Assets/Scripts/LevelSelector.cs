using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class LevelSelector : MonoBehaviour
{
    [SerializeField] GameObject MainCam;
    [SerializeField] GameObject scrollBar;
    [SerializeField] GameObject[] leftButtonsArray, rightButtonsArray;
    [SerializeField] Button[] levelButtons;
    [SerializeField] float distance;
    [SerializeField] int orangeStart, greenStart;

    PlayerControl playerControl;

    float dest;
    int currentScroll = 0;
    float scrollPos = 0;
    float[] pos;

    int progress;

    private void Awake()
    {
        playerControl = new PlayerControl();
    }
    private void OnEnable()
    {
        playerControl.Enable();
        playerControl.LevelSelector.Pan.performed += Pan_performed;
        playerControl.LevelSelector.Enter.performed += Enter_performed;
        levelButtons = GetComponentsInChildren<Button>();
        progress = PlayerPrefs.GetInt("Level", 0);
        for(int i = 0; i < levelButtons.Length; i++)
        {
            if(i <= progress)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
        pos = new float[transform.childCount];
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
        currentScroll = progress; 
        dest = pos[currentScroll];
    }
    private void OnDisable()
    {
        playerControl.Disable();
        playerControl.LevelSelector.Pan.performed -= Pan_performed;
        playerControl.LevelSelector.Enter.performed -= Enter_performed;
    }
    private void Pan_performed(InputAction.CallbackContext val)
    {
        if(val.ReadValue<float>() > 0.2f)
        {
            NextLevelScroll();
        }
        else if(val.ReadValue<float>() < -0.2f)
        {
            PrevLevelScroll();
        }
    }

    private void Enter_performed(InputAction.CallbackContext val)
    {
        if(currentScroll <= progress)
        {
            PlayLevel(currentScroll+1);
        }
    }

    private void Update()
    {
        for (int i = 0; i < pos.Length; i++)
        {
            if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            {
                scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, dest, 0.1f);
            }
        }
        MainCam.transform.position = new Vector3(Mathf.Lerp(MainCam.transform.position.x, dest, Time.deltaTime * 5), 0, -7.5f);
        #region ButtonColors
        if (currentScroll + 1 >= greenStart)
        {
            rightButtonsArray[2].SetActive(true);
            rightButtonsArray[1].SetActive(false);
            rightButtonsArray[0].SetActive(false);
        }
        else if(currentScroll + 1 >= orangeStart)
        {
            rightButtonsArray[2].SetActive(false);
            rightButtonsArray[1].SetActive(true);
            rightButtonsArray[0].SetActive(false);
        }
        else
        {
            rightButtonsArray[2].SetActive(false);
            rightButtonsArray[1].SetActive(false);
            rightButtonsArray[0].SetActive(true);
        }
        if(currentScroll - 1 >= greenStart)
        {
            leftButtonsArray[2].SetActive(true);
            leftButtonsArray[1].SetActive(false);
            leftButtonsArray[0].SetActive(false);
        }
        else if(currentScroll - 1 >= orangeStart)
        {
            leftButtonsArray[2].SetActive(false);
            leftButtonsArray[1].SetActive(true);
            leftButtonsArray[0].SetActive(false);
        }
        else
        {
            leftButtonsArray[2].SetActive(false);
            leftButtonsArray[1].SetActive(false);
            leftButtonsArray[0].SetActive(true);
        }
        #endregion
    }
    public void NextLevelScroll()
    {
        if (currentScroll < transform.childCount - 1)
            currentScroll++;
        dest = pos[currentScroll];
    }
    public void PrevLevelScroll()
    {
        if (currentScroll > 0)
            currentScroll--;
        dest = pos[currentScroll];
    }
    public void PlayLevel(int _level)
    {
        SceneManagerClass.instance.LoadNewScene(_level);
    }
}