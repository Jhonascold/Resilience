using UnityEngine;

public class HealthBar : MonoBehaviour
{
    // Referencia al RectTransform de la imagen Fill.
    public RectTransform fillRectTransform;
    
    // Valor de offset derecho cuando la vida está vacía y cuando está completa.
    // Por ejemplo, 0 cuando está llena y 1.6 cuando está vacía.
    public float fullRightValue = 0f;
    public float emptyRightValue = 1.6f;

    /// <summary>
    /// Actualiza la barra de vida modificando el offset derecho.
    /// healthPercent debe ser un valor entre 0 (sin vida) y 1 (vida completa).
    /// </summary>
    public void SetHealth(float healthPercent)
    {
        // Queremos que cuando healthPercent sea 1, el offset sea fullRightValue, y cuando sea 0, sea emptyRightValue.
        // Si lo pensamos en términos de interpolación, podemos usar:
        float newRightOffset = Mathf.Lerp(emptyRightValue, fullRightValue, healthPercent);
        
        Vector2 currentOffsetMin = fillRectTransform.offsetMin;
        currentOffsetMin.x = newRightOffset;
        fillRectTransform.offsetMin = currentOffsetMin;
        
    }
}


