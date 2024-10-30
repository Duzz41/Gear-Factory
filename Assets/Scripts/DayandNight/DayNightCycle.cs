using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    //public Volume ppv;
    public Light2D _light;
    [SerializeField] Color _gradient;

    public float tick;
    public float seconds;
    public int mins;
    public int hours;
    public int days = 1;

    public bool activateLights;
    public GameObject[] lights;
    public SpriteRenderer[] stars;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
        //ppv = gameObject.GetComponent<Volume>();
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
    }

    void ControlPPV()
    {
        if (hours >= 19 && hours < 21)
        {
            if (_light.intensity >= 0.1f)
            {
                _light.intensity -= Time.fixedDeltaTime / 10;
                _light.color = Color.Lerp(_light.color, _gradient, Time.fixedDeltaTime);
            }


            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].color = new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, Time.fixedDeltaTime / 10);
            }
            if (activateLights == false)
            {
                if (mins > 45)
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].SetActive(true);

                    }
                    activateLights = true;
                }
            }
        }

        if (hours >= 5 && hours < 7)
        {
            if (_light.intensity <= 1)
            {
                _light.intensity += Time.fixedDeltaTime / 10;
                _light.color = Color.Lerp(_light.color, Color.white, Time.fixedDeltaTime);
            }
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].color = new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, Time.fixedDeltaTime / 10);
            }
            if (activateLights == true)
            {
                if (mins > 45)
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

    public void DisplayTime()
    {

    }

}
