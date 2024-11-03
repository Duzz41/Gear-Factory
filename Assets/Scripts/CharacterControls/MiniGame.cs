using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MiniGame : MonoBehaviour
{

    [SerializeField] private CharacterMovement characterMovement_cs;
    public RectTransform uiElement1; // Birinci UI elementi
    public RectTransform uiElement2; // İkinci UI elementi

    [SerializeField] private Slider slider;
    [SerializeField] private float travel_speed;
    private bool traveling = true;
    private Vector2 start_pos;



    void Start()
    {
        start_pos = uiElement2.localPosition;
        EventDispatcher.RegisterFunction("ActivateGame", ActivateGame);
    }
    void Update()
    {

        if (traveling)
        {
            float xPos = Mathf.PingPong(travel_speed * Time.time, 140);
            uiElement2.localPosition = new Vector2(xPos + start_pos.x, uiElement2.localPosition.y);
        }


    }

    void StopGame()
    {
        if (AreUIElementsOverlapping(uiElement1, uiElement2))
        {
            characterMovement_cs.energy = 20f;

            Debug.Log("UI elementleri carpisiyor!");
        }
        else
        {
            Debug.Log("UI elementleri carpisymior.");
        }
        EventDispatcher.SummonEvent("MiniGameForEnergy");
        traveling = false;
    }

    public void ActivateGame()
    {
        slider.value = (float)Random.Range(0, 1f);
        traveling = true;
    }

    // İki UI elementinin çarpışıp çarpışmadığını kontrol eden fonksiyon
    public bool AreUIElementsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        // UI elementlerinin dünya koordinatlarındaki dikdörtgen sınırlarını alıyoruz
        Rect rect1World = GetWorldRect(rect1);
        Rect rect2World = GetWorldRect(rect2);

        // Dikdörtgenlerin çarpışıp çarpışmadığını kontrol ediyoruz
        return rect1World.Overlaps(rect2World);
    }

    // Verilen RectTransform'un dünya koordinatlarında dikdörtgensel sınırlarını alır
    public Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // Dünya koordinatlarıyla Rect oluşturuyoruz
        Vector2 size = new Vector2(corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        return new Rect(corners[0], size);
    }



    #region Inputs
    public void EnergyButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StopGame();
        }
        #endregion
    }
}
