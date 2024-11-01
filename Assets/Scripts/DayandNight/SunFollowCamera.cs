using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFollowCamera : MonoBehaviour
{public Transform centerObject;  // Dairenin merkezinde bulunacak nesne (örneğin kamera)
    public float radiusX = 10f;     // X ekseni için yarıçap
    public float radiusY = 5f;     // Y ekseni için yarıçap
    public float orbitSpeed = 0.03f;   // Güneşin dönme hızı
    [SerializeField] private float angle = -3;       // Başlangıç açısı

    void FixedUpdate()
    {
        // Açıyı zamanla artırarak dönüşü sağla
        angle -= orbitSpeed * Time.deltaTime;

        // X ve Y pozisyonlarını hesapla (elips için farklı yarıçaplar kullanarak)
        float x = centerObject.position.x + Mathf.Cos(angle) * radiusX;
        float y = centerObject.position.y + Mathf.Sin(angle) * radiusY;

        // Güneşin yeni pozisyonunu belirle
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
