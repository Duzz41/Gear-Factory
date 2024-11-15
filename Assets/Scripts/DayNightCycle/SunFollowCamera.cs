using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFollowCamera : MonoBehaviour
{
    public DayNightCycle dayNightCycle;
    public Transform centerObject;  // Dairenin merkezinde bulunacak nesne (örneğin kamera)
    public float radiusX = 10f;     // X ekseni için yarıçap
    public float radiusY = 5f;     // Y ekseni için yarıçap
    public float orbitSpeed = 0.03f;   // Güneşin dönme hızı
    [SerializeField] private float angle = -3;       // Başlangıç açısı
    [SerializeField] private float offsett;

    void FixedUpdate()
    {
        Cycle();
    }

    void Cycle()
    {
        if (GameManager.instance._isDay == true)
            angle -= orbitSpeed * Time.deltaTime;
        else
            angle = -2.5f;

        // X ve Y pozisyonlarını hesapla (elips için farklı yarıçaplar kullanarak)
        float x = (centerObject.position.x + offsett) + Mathf.Cos(angle) * radiusX;
        float y = centerObject.position.y + Mathf.Sin(angle) * radiusY;

        // Güneşin yeni pozisyonunu belirle
        transform.position = new Vector3(x, y, transform.position.z);

    }

}
