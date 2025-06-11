using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform leftDoor, rightDoor;
    public Vector3 leftClosedPos, leftOpenPos, rightClosedPos, rightOpenPos;
    public float speed = 2f;
    private bool isOpen = false;
    
    void Start() {
        // Guardamos la posición local inicial como “cerrada”
        leftClosedPos  = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;
        // Desplazamientos exactos que quieres
        leftOpenPos    = leftClosedPos  + new Vector3(-7.29f, 0f, 0f);
        rightOpenPos   = rightClosedPos + new Vector3(7.7f,  0f, 0f);
    }

    void Update() {
        // Interpolamos suavemente entre cerrado y abierto
        Vector3 targetL = isOpen ? leftOpenPos  : leftClosedPos;
        Vector3 targetR = isOpen ? rightOpenPos : rightClosedPos;
        leftDoor.localPosition  = Vector3.Lerp(leftDoor.localPosition,  targetL, Time.deltaTime * speed);
        rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, targetR, Time.deltaTime * speed);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            isOpen = true;
            Debug.Log("Abriendo puertas");
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isOpen = false;
            Debug.Log("Cerrando puertas");
        }
    }
}
