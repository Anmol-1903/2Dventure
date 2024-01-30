using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] string[] tips, dialogues;
    [SerializeField] AudioClip[] clip;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Image muteSprite;
    [SerializeField] Image unMuteSprite;

    private bool isPaused;
    private bool isMute;
    TextMeshProUGUI tipText, dialogueText;
    AudioSource audioSource;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;


        tipText = pauseMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dialogueText = pauseMenu.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        audioSource = pauseMenu.GetComponent<AudioSource>();

        audioMixer.GetFloat("Volume", out float vol);
        if (vol < 0)
        {
            isMute = true;
            unMuteSprite.gameObject.SetActive(false);
            muteSprite.gameObject.SetActive(true);
        }
        else
        {
            isMute = false;
            unMuteSprite.gameObject.SetActive(true);
            muteSprite.gameObject.SetActive(false);
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenu.SetActive(true);
        tipText.text = tips[Random.Range(0, tips.Length)];
        int temp = Random.Range(0, dialogues.Length);
        dialogueText.text = dialogues[temp];
        audioSource.clip = clip[temp];
        audioSource.Play();
    }
    public void Resume()
    {
        if (CanResume())
        {
            Time.timeScale = 1f;
            isPaused = false;
            pauseMenu.SetActive(false);
        }
    }
    public bool CanResume()
    {
        return !audioSource.isPlaying;
    }
    public bool IsPaused()
    {
        return isPaused;
    }
    public void Restart()
    {
        if (CanResume())
        {
            Resume();
            SceneManagerClass.instance.LoadNewScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void Mute()
    {
        if(isMute)
        {
            audioMixer.SetFloat("Volume", 0);
            unMuteSprite.gameObject.SetActive(true);
            muteSprite.gameObject.SetActive(false);
        }
        else
        {
            audioMixer.SetFloat("Volume", -80);
            unMuteSprite.gameObject.SetActive(false);
            muteSprite.gameObject.SetActive(true);
        }
        isMute = !isMute;
    }
    public void MainMenu()
    {
        if (CanResume())
        {
            Resume();
            SceneManagerClass.instance.LoadNewScene(0);
        }
    }
}