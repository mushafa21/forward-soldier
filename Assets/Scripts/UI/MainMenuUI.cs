using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public UIButton startGameButton, howToPlayButton, closeHowToPlayButton;
    public GameObject howtoPlayPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        howToPlayButton.onClick.AddListener(ShowHowToPlay);
        closeHowToPlayButton.onClick.AddListener(HideHowToPlay);
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
