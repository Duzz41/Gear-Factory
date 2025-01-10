using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; 
    public Animator pauseAnimator; 
    public Animator quitButtonAnimator; 
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
        pauseMenuUI.SetActive(true); 
        pauseAnimator.SetTrigger("Show"); 
        quitButtonAnimator.SetTrigger("Show"); 
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; 
        pauseAnimator.SetTrigger("Hide"); 
        quitButtonAnimator.SetTrigger("Hide"); 
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
            pauseMenuUI.SetActive(false); 
        }
    }
}