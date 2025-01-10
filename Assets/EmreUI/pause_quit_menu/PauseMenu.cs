using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Animator pauseAnimator;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        //pauseMenuUI.SetActive(true);
        pauseAnimator.SetTrigger("Show");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ResumeGame()
    {
        pauseAnimator.ResetTrigger("Show");
        isPaused = false;
        Time.timeScale = 1f;
        pauseAnimator.SetTrigger("Hide");
        StartCoroutine(HidePauseMenuAfterAnimation());
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Quit işlemi çalıştırıldı!");
    }

    private System.Collections.IEnumerator HidePauseMenuAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (!isPaused)
        {
            pauseAnimator.ResetTrigger("Hide");
            pauseMenuUI.SetActive(false);
        }
    }
}