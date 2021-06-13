using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public float nonSeed = -544.6f; 
    public int filas { get; set; }
    public int columnas { get; set; }
    public float seed { get; set; }
    public float velocidad { get; set; }
    public int ent { get; set; }

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            instancia.filas = 15;
            instancia.columnas = 15;
            instancia.velocidad = 1.0f;
            instancia.ent = 3;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    //  Determina si los campos introducidos en los inputsField son correctos para pasar a la scena loading
    public void GoLoadingScene()
    {
        var iF = Canvas.FindObjectsOfType<InputField>();
        bool colOk = false;
        bool filOk = false;
        bool seedOk = false;
        foreach (InputField inF in iF)
        {
            switch (inF.name)
            {
                case "SeedCampo":
                    float num = nonSeed;
                    if (inF.text == "" || float.TryParse(inF.text, out num))
                    {
                        seed = num;
                        seedOk = true;
                    }
                    else
                    {
                        inF.text = "Formato inv�lido";
                    }
                    break;
                case "ColumnasCampo":
                    int col = 15;
                    if (inF.text == "" || int.TryParse(inF.text, out col))
                    {
                        columnas = col;
                        colOk = true;
                    }
                    else
                    {
                        inF.text = "Formato inv�lido";
                    }
                    break;
                case "FilasCampo":
                    int fil = 15;
                    if (inF.text == "" || int.TryParse(inF.text, out fil))
                    {
                        filas = fil;
                        filOk = true;
                    }
                    else
                    {
                        inF.text = "Formato inv�lido";
                    }
                    break;
                default:
                    break;
            }
        }
        if (colOk && filOk && seedOk)
        {
            SceneManager.LoadScene("LoadingScene");
        }
    }

    //  Carga la escena Main cuando todo est� cargado y listo para su uso
    public void GoMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    //  Activa o desactiva el panel de informaci�n 
    public void ActivePanel(GameObject gm)
    {
        if (gm.active)
        {
            gm.SetActive(false);
        }
        else gm.SetActive(true);
    }

    public void actualizaSlider(GameObject gm)
    {
        ent = (int)gm.GetComponent<Slider>().value;
    }

    public void actualizaVelocidad(GameObject gm)
    {
        velocidad = gm.GetComponent<Slider>().value;
        Debug.Log(gm.GetComponent<Slider>().value);
    }

    public void actualizaVentana(GameObject gm)
    {
        if (gm.GetComponent<Toggle>().isOn)
        {
            Screen.SetResolution(1200, 600, true);
        }
        else
        {
            Screen.SetResolution(800, 600, false);
        }
    }

    public void cierraApp()
    {
        Application.Quit();
    }
}
