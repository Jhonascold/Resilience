using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//Este script ira ligado a un canvas que contenga los textos de las vidas, puntuacion, estrellas y tiempo
public class UIManager : MonoBehaviour
{
    //public TMP_Text textoVidas;
    public TMP_Text textoPuntuacion;
    //public TMP_Text textoMonedas;
    //public TMP_Text textoTiempo;

    void Update()
    {
        // Si el GameManager existe, mostrar sus valores
        if (ScoreManager.Instance != null)
        {
            //Con esto controlo los textos que quiero mostrar y los que no en distintas escenas. En la de GameOver p.e no quiero mostrar ni vidas ni tiempo.
            
            if(SceneManager.GetActiveScene().name == "Win" || SceneManager.GetActiveScene().name == "GameOver")
            {
                textoPuntuacion.text = "Puntos: " + ScoreManager.Instance.score;
                
            }else{
                textoPuntuacion.text = ScoreManager.Instance.score.ToString();
            }
            
            //textoMonedas.text = "Monedas: " + GameManager.instance.monedas;
            
        }
    }
}
