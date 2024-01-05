using UnityEngine;
using UnityEngine.UI;
public class LevelSelector : MonoBehaviour
{
    [SerializeField] GameObject MainCam;
    [SerializeField] GameObject scrollBar;
    [SerializeField] float distance;
    [SerializeField] GameObject[] leftButtonsArray, rightButtonsArray;
    [SerializeField] int orangeStart, greenStart;
    float dest;
    int currentScroll = 0;
    float scrollPos = 0;
    float[] pos;
    private void Update()
    {
        pos = new float[transform.childCount];
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
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
}