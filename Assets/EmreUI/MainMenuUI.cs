using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    public Animator anim;
    public GameObject textImage; // Yazıların gösterileceği Image GameObject'i
    public TextMeshProUGUI textMeshPro; // TextMeshPro bileşeni
    public Button nextButton; // Geçiş butonu
    public string[] messages; // Gösterilecek mesaj parçaları
    private int currentMessageIndex = 0; // Şu anki mesajın indeksi
    private Coroutine typingCoroutine;

    void Start()
    {
        Screen.fullScreen = true;
        nextButton.onClick.AddListener(OnNextButtonClicked); // Buton tıklama olayını dinle
        textImage.SetActive(false); // Başlangıçta Image'i gizle
    }


    // Belirli bir sahneye geçiş yapar
    public void LoadScene(string sceneName)
    {
        // Animasyonu başlat
        if (sceneName == "GameScene")
        {
            anim.SetTrigger("StartAnimation");

            // Coroutine başlat
            StartCoroutine(WaitForAnimationAndLoadScene(sceneName));
        }
        else
            SceneManager.LoadScene(sceneName);
    }

    // Animasyon bitene kadar bekler ve sahneyi yükler
    private IEnumerator WaitForAnimationAndLoadScene(string sceneName)
    {
        // Animasyonun süresini bekle
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("GameScene");
        textImage.SetActive(true); // Image'i aktif et
        currentMessageIndex = 0; // İlk mesajı göster
        typingCoroutine = StartCoroutine(TypeText(messages[currentMessageIndex])); // İlk mesajı yazdır
    }
    private void OnNextButtonClicked()
    {
        if (typingCoroutine != null)
        {
            // Eğer yazma coroutine'i çalışıyorsa, durdur ve yazıyı hemen göster
            StopCoroutine(typingCoroutine);
            textMeshPro.text = messages[currentMessageIndex]; // Yazıyı hemen göster
            typingCoroutine = null; // Coroutine'i sıfırla
        }
        else
        {
            currentMessageIndex++; // Sonraki mesajın indeksini artır
            if (currentMessageIndex < messages.Length)
            {
                typingCoroutine = StartCoroutine(TypeText(messages[currentMessageIndex])); // Sonraki mesajı yazdır
            }
            else
            {
                AudioManager.instance.PlayMusic("Theme");
                // Tüm mesajlar gösterildiyse, sahneye geçiş yap
                SceneManager.LoadScene("GameScene");

            }
        }
    }
    // Yazıları harf harf yazdırır
    private IEnumerator TypeText(string text)
    {
        textMeshPro.text = ""; // Önce metni temizle
        foreach (char letter in text.ToCharArray())
        {
            textMeshPro.text += letter; // Harf ekle
            yield return new WaitForSeconds(0.1f); // Her harf arasında bekle
        }
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
