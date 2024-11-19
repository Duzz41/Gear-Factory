using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuUI : MonoBehaviour
{
    // Belirli bir sahneye geçiş yapar
    public void LoadScene(string sceneName)
    {
        // Sahnenin ismini parametre olarak alır ve o sahneye geçer
        SceneManager.LoadScene(sceneName);
    }

    // Oyunu kapatır
    public void QuitGame()
    {
        // Unity editöründe çalışırken Quit çalışmaz, bu yüzden bir mesaj bırakabiliriz.
#if UNITY_EDITOR
        Debug.Log("Game quit! (Bu sadece editörde çalışır)");
        UnityEditor.EditorApplication.isPlaying = false; // Editör modundan çıkış
#else
        Application.Quit(); // Derlenmiş oyunda çıkışı sağlar
#endif
    }


}
