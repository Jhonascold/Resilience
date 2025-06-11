using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Este script ira ligado a un objeto vacio llamado MenuManagerObj que se encargara de gestionar los botones del menu. Tambien se encuentra asignado al canvas de la escena main
public class MenuManager : MonoBehaviour
{
    
    public void IniciarJuego()
    {   
        if(ScoreManager.Instance != null)
            ScoreManager.Instance.ResetValues();
        SceneManager.LoadScene("Estación(Escena1)");
    }

    public void SalirJuego()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ReiniciarJuego()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetValues();
        SceneManager.LoadScene("Estación(Escena1)");
    }

    public void IrAMenu()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetValues();
        SceneManager.LoadScene("MenuPrincipal");
    }


    
}
