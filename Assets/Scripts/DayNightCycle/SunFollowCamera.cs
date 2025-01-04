using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFollowCamera : MonoBehaviour
{
    public DayNightCycle dayNightCycle;
    public Transform centerObject;  // Dairenin merkezinde bulunacak nesne (örneğin kamera)
    public float radiusX = 10f;     // X ekseni için yarıçap
    public float radiusY = 5f;      // Y ekseni için yarıçap
    public float orbitSpeed = 1f;   // Güneşin dönme hızı
    [SerializeField] private float offsett;

    void FixedUpdate()
    {
        Cycle();
    }

    void Cycle()
    {
        // Günün zamanını al
        float timeOfDay = dayNightCycle.timeOfDay;

        // Güneşin açısını hesapla (0.0 - 1.0 arası değerleri 0 - 360 dereceye çevir)
        float angle = -timeOfDay * 360f;

        // X ve Y pozisyonlarını hesapla (elips için farklı yarıçaplar kullanarak)
        float x = (centerObject.position.x + offsett) + Mathf.Cos(Mathf.Deg2Rad * angle) * radiusX;
        float y = centerObject.position.y + Mathf.Sin(Mathf.Deg2Rad * angle) * radiusY;

        // Güneşin yeni pozisyonunu belirle
        transform.position = new Vector3(x, y, transform.position.z);
    }
}