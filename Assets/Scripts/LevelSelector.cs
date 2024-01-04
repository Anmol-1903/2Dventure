using UnityEngine;
using UnityEngine.UI;
public class LevelSelector : MonoBehaviour
{
    [SerializeField] GameObject MainCam;
    [SerializeField] GameObject scrollBar;
    [SerializeField] float dest;
    [SerializeField] int currentScroll = 0;
    [SerializeField] float scrollPos = 0;
    [SerializeField] float[] pos;
    [SerializeField] float distance;
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
        MainCam.transform.position = new Vector3(Mathf.Lerp(MainCam.transform.position.x, dest, Time.deltaTime * 2), 0, -7.5f);
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