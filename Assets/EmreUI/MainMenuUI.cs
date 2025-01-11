using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] RotateWheel rotateWheel;
    public bool isMenu = false;
    [SerializeField] private string[] typeNames;
    public Animator anim;
    public Animator settingAnim;
    public GameObject textImage; // Yazıların gösterileceği Image GameObject'i
    public TextMeshProUGUI textMeshPro; // TextMeshPro bileşeni
    public string[] messages; // Gösterilecek mesaj parçaları
    private int currentMessageIndex = 0; // Şu anki mesajın indeksi
    private Coroutine typingCoroutine;
    [SerializeField] private Animator anim2;

    void Start()
    {

        Screen.fullScreen = true;
        textImage.SetActive(false); // Başlangıçta Image'i gizle
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            OnNextButtonClicked();
        }


    }
    public void StartSettingsAnim()
    {
        isMenu = true;
        settingAnim.SetTrigger("StartSetting");

    }
    public void StopSettingsAnim()
    {
        rotateWheel.UpdateButtonStates();
        isMenu = false;
        settingAnim.ResetTrigger("StartSetting");
        settingAnim.SetTrigger("ExitSetting");

    }
    // Belirli bir sahneye geçiş yapar
    public void LoadScene(string sceneName)
    {
        // Animasyonu başlat
        if (sceneName == "GameScene")
        {
            anim.SetTrigger("StartAnimation");
            anim2.SetTrigger("FALL");
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

        //SceneManager.LoadScene("GameScene");
        textImage.SetActive(true); // Image'i aktif et
        currentMessageIndex = 0; // İlk mesajı göster
        typingCoroutine = StartCoroutine(TypeText(messages[currentMessageIndex])); // İlk mesajı yazdır
    }
    public void OnNextButtonClicked()
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
    private int currentIndex = 0;
    string TypeSound()
    {
        if (currentIndex >= typeNames.Length)
        {
            currentIndex = 0; // Başlangıca dön
        }

        // Mevcut taneyi al
        string typeName = typeNames[currentIndex];

        // İndeksi bir artır
        currentIndex++;

        return typeName; // Taneyi döndür
    }
    // Yazıları harf harf yazdırır
    private IEnumerator TypeText(string text)
    {
        textMeshPro.text = ""; // Önce metni temizle
        foreach (char letter in text.ToCharArray())
        {
            textMeshPro.text += letter; // Harf ekle
            //AudioManager.instance.sfxSource.pitch=0.8f;
            AudioManager.instance.PlaySfx(TypeSound());

            yield return new WaitForSeconds(0.08f); // Her harf arasında bekle
        }
        OnNextButtonClicked();
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
