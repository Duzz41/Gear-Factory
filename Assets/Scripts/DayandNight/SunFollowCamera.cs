using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFollowCamera : MonoBehaviour
{
    public Transform centerObject;  // Dairenin merkezinde bulunacak nesne (örneğin kamera)
    public float radius = 15f;      // Dairenin yarıçapı
    public float orbitSpeed = 1f;   // Güneşin dönme hızı
    private float angle = 90f;       // Başlangıç açısı

    void FixedUpdate()
    {
        // Açıyı zamanla artırarak dönüşü sağla
        angle -= orbitSpeed * Time.deltaTime;

        // X ve Y pozisyonlarını hesapla
        float x = centerObject.position.x + Mathf.Cos(angle) * radius;
        float y = centerObject.position.y + Mathf.Sin(angle) * radius;

        // Güneşin yeni pozisyonunu belirle
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
