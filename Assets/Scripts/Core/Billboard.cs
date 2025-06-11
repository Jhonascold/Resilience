using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Hace que el objeto mire a la cámara
        transform.LookAt(Camera.main.transform);
        // Opcionalmente, invierte la rotación para que se vea correctamente
        transform.Rotate(0, 180, 0);
    }
}

