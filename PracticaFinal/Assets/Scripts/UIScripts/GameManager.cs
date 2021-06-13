using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public int filas { get; set; }
    public int columnas { get; set; }
    public float seed { get; set; }
    public int ent { get; set; }

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            instancia.filas = 15;
            instancia.columnas = 15;
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
                    float num = -544.6f;
                    if (inF.text == "" || float.TryParse(inF.text, out num))
                    {
                        seed = num;
                        seedOk = true;
                    }
                    else
                    {
                        inF.text = "Formato inválido";
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
                        inF.text = "Formato inválido";
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
                        inF.text = "Formato inválido";
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

    //  Carga la escena Main cuando todo está cargado y listo para su uso
    public void GoMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    //  Activa o desactiva el panel de información 
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
}
