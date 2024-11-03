using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    //public Volume ppv;
    public Light2D _light;
    public Light2D sun;
    public Light2D moon;
    [SerializeField] Color _color;
    [SerializeField] Gradient _gradient;

    public bool isDay;
    public float tick;
    public float seconds;
    public int mins;
    public int hours;
    public int days = 1;

    public bool activateLights;
    public GameObject[] lights;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalcTime();
        ControlPPV();
    }
    public void CalcTime()
    {
        seconds += Time.fixedDeltaTime * tick;
        if (seconds >= 60)
        {
            seconds = 0;
            mins += 1;

        }
        if (mins >= 60)
        {
            mins = 0;
            hours += 1;

        }
        if (hours >= 24)
        {
            hours = 0;
            days += 1;
        }
        ControlPPV();
        CheckDay();
    }

    void ControlPPV()
    {
        float timePercent = (hours * 60 + mins) / 1440f;

        // Gradiente göre rengi ayarla
        _color = _gradient.Evaluate(timePercent);
        _light.color = _color;
        if (hours >= 18 && hours < 22)
        {

            if (_light.intensity > 0.01f)
            {
                _light.intensity -= Time.fixedDeltaTime / 50;
                sun.intensity = _light.intensity - 0.04f;

            }
            if (moon.intensity <= 0.6)
                moon.intensity += Time.fixedDeltaTime / 50;
            if (activateLights == false)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].SetActive(true);
                }
                activateLights = true;
            }
        }
        if (hours >= 3 && hours < 7)
        {
            if (_light.intensity <= 1)
            {
                _light.intensity += Time.fixedDeltaTime / 50;
                sun.intensity = _light.intensity;
            }
            if (moon.intensity > 0.01f)
                moon.intensity -= Time.fixedDeltaTime / 50;

        }
    }

    void CheckDay()
    {
        if (hours >= 22 && hours < 23)
        {
            isDay = false;
        }
        else if (hours >= 6 && hours < 7)
        {
            isDay = true;
            if (activateLights == true)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].SetActive(false);
                }
                activateLights = false;
            }
        }
    }
}
