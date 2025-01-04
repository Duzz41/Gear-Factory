using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] Color _color;
    public Light2D _light;
    public Light2D sun;
    public Light2D moon;
    [SerializeField] Gradient _gradient;

    public float dayLength = 180f; // Gün uzunluğu (saniye cinsinden)
    public float timeOfDay; // Gün içindeki zaman (0.0 - 1.0 arası)

    public bool activateLights;
    public GameObject[] lights;

    public int days; // Gün sayacı
    public GameObject dayImage; // Gün değiştiğinde aktif olacak görüntü
    public TextMeshProUGUI dayText; // Gün sayısını gösterecek metin
    


    // Start is called before the first frame update
    void Start()
    {
        days = 1;
        ShowDayText();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        ControlLighting();
        CheckDayNight();
    }

    void UpdateTimeOfDay()
    {
        timeOfDay += Time.deltaTime / dayLength; // Zamanı gün uzunluğuna göre güncelle
        if (timeOfDay >= 1f) // Gün tamamlandığında sıfırla
        {
            timeOfDay = 0f;
            days++;

        }
    }
    void ShowDayText()
    {
        AudioManager.instance.PlaySfx("TextType");
        dayText.gameObject.SetActive(true); // Yazıyı aktif et
        StartCoroutine(TypeText("DAY: " + days)); // Gün sayısını yazdır
    }

    IEnumerator TypeText(string text)
    {
        dayText.text = ""; // Önce metni temizle
        dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, 1f); // Opaklığı 1 yap

        foreach (char letter in text.ToCharArray())
        {
            dayText.text += letter; // Harf ekle

            yield return new WaitForSeconds(0.5f); // Her harf arasında bekle (daha kısa süre)
        }

        // Yazıyı belirli bir süre açık tut
        yield return new WaitForSeconds(2f);

        // Yazıyı yavaşça kapat
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float duration = 1f; // Kapanma süresi
        float time = 0f;

        // Opaklığı yavaşça azalt
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            yield return null;
        }

        dayText.gameObject.SetActive(false); // Yazıyı kapat
    }

    void ControlLighting()
    {
        float timePercent = timeOfDay; // 0.0 - 1.0 arası

        // Renk ve yoğunluk ayarları
        _color = _gradient.Evaluate(timePercent);
        _light.color = _color;

        // Güneş ve ayın yoğunluğunu ayarla
        sun.intensity = Mathf.Clamp01(1 - Mathf.Abs(2 * timePercent - 1)); // Gündüz yoğunluğu
        moon.intensity = Mathf.Clamp01(2 * timePercent); // Gece yoğunluğu

        // Işıkları aç/kapa
        if (timePercent >= 0.75f || timePercent < 0.2f) // Gece
        {
            if (!activateLights)
            {
                sun.intensity = 0f;
                foreach (var light in lights)
                {
                    light.SetActive(true);
                }
                activateLights = true;
            }
        }
        else // Gündüz
        {
            if (activateLights)
            {
                ShowDayText();
                sun.intensity = 1f;

                foreach (var light in lights)
                {
                    light.SetActive(false);
                }
                activateLights = false;
            }
        }
    }

    void CheckDayNight()
    {
        if (timeOfDay >= 0.2f && timeOfDay < 0.85f) // Gece veya gün kontrolü
        {
            GameManager.instance._isDay = true;
        }
        else if (timeOfDay >= 0.85f || timeOfDay < 0.2f)
        {
            GameManager.instance._isDay = false;
        }

    }
}