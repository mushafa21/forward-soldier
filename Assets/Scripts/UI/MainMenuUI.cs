using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public UIButton startGameButton, howToPlayButton, closeHowToPlayButton;
    public GameObject howtoPlayPanel;
    public AudioClip mainMenuSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        GameManager.Instance.playerHud.SetActive(false);

        startGameButton.onClick.AddListener(StartGame);
        howToPlayButton.onClick.AddListener(ShowHowToPlay);
        closeHowToPlayButton.onClick.AddListener(HideHowToPlay);
        AudioManager.Instance.PlayMusic(mainMenuSound);
    }

    private void StartGame()
    {
        GameManager.Instance.GoToFirstLevel();
    }

    private void ShowHowToPlay()
    {
        howtoPlayPanel.SetActive(true);
    }

    private void HideHowToPlay()
    {
        howtoPlayPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
