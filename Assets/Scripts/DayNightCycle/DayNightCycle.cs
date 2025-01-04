using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] Color _color;
    public Light2D _light;
    public Light2D sun;
    public Light2D moon;
    [SerializeField] Gradient _gradient;

    public float dayLength = 120f; // Gün uzunluğu (saniye cinsinden)
    public float timeOfDay; // Gün içindeki zaman (0.0 - 1.0 arası)

    public bool activateLights;
    public GameObject[] lights;

    // Start is called before the first frame update
    void Start()
    {
        timeOfDay = 0.25f; // Başlangıçta 120 derece konumunda
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
        }
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
        if (timePercent >= 0.75f || timePercent < 0.25f) // Gece
        {
            if (!activateLights)
            {
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
        GameManager.instance._isDay = timeOfDay >= 0.2f || timeOfDay < 0.8f; // Gündüz ve gece kontrolü
    }
}