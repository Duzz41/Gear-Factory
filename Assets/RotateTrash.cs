using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateTrash : MonoBehaviour
{
    // Sağ hareket için ayarlar
    public float moveDistance = 5f; // Sağ hareket mesafesi
    public float moveDuration = 2f; // Sağ hareket süresi

    // Dönme için ayarlar
    public Vector3 rotationAngle = new Vector3(0, 0, 30); // Z ekseninde dönecek açı
    public float rotationDuration = 1f; // Dönme süresi

    void Start()
    {
        // Sağ hareket ve dönüşü başlat
        MoveAndRotate();
    }

    void MoveAndRotate()
    {
        // Sağ hareket
        transform.DOMoveX(transform.position.x + moveDistance, moveDuration)
            .SetEase(Ease.Linear) // Düz bir hareket
            .OnComplete(() => 
            {
                // Hareket tamamlandığında tekrar hareket et
                MoveAndRotate();
            });

        // Sürekli dönüş
        transform.DORotate(-rotationAngle, rotationDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental); // Sonsuz döngü
    }
}