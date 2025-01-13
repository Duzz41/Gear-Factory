using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MiniGame : MonoBehaviour
{

    [SerializeField] private CharacterMovement characterMovement_cs;
    public RectTransform handle; // Birinci UI elementi
    public RectTransform pointer; // İkinci UI elementi

    [SerializeField] private Slider slider;
    [SerializeField] private float travel_speed;

    public bool traveling = true;
    private Vector2 start_pos;



    void Start()
    {
        start_pos = pointer.localPosition;
        EventDispatcher.RegisterFunction("ActivateGame", ActivateGame);
    }
    void Update()
    {

        if (traveling)
        {
            float xPos = Mathf.PingPong(travel_speed * Time.time, 160);
            pointer.localPosition = new Vector2(xPos + start_pos.x, pointer.localPosition.y);
        }

    }
    public void MiniGameForEnergy()
    {
        traveling = false;
        float xPosition = Mathf.Lerp(-80f, 80f, slider.value);

        if (pointer.localPosition.x <= xPosition && pointer.localPosition.x >= xPosition - 13f)
        {
            characterMovement_cs.energy = 20f;
            Debug.Log("Değdi");
            characterMovement_cs.stopCar = false;
            AudioManager.instance.sfxSource.Stop();
        }
        else
        {
            Debug.Log("Değmedi");
            characterMovement_cs.stopCar = false;
        }


        this.gameObject.SetActive(false);
    }

    public void ActivateGame()
    {
        slider.value = (float)Random.Range(0.1f, 1f);
        traveling = true;
    }



}
