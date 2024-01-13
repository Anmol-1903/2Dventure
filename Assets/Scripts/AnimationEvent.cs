using UnityEngine;
using System.Collections;
public class AnimationEvent : MonoBehaviour
{
    public void LoadingAnimationEnd()
    {
        StartCoroutine(LoadAsync());
        gameObject.SetActive(false);
    }
    IEnumerator LoadAsync()
    {
        yield return new WaitForSeconds(1);
        SceneManagerClass.instance.AllowLoading();
    }
}