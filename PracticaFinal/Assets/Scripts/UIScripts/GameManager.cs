using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace luaberinto
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instancia;

        public float nonSeed { get; set; }
        public int filas { get; set; }
        public int columnas { get; set; }
        public float seed { get; set; }
        public float velocidad { get; set; }
        //  Número de NPC y objetos
        public int ent { get; set; }

        private void Awake()
        {
            if (instancia == null)
            {
                instancia = this;
                instancia.filas = 10;
                instancia.columnas = 10;
                instancia.velocidad = 1.0f;
                instancia.ent = 3;
                instancia.nonSeed = -544.6f;
            }
            else
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
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
                            instancia.seed = num;
                            seedOk = true;
                        }
                        else
                        {
                            inF.text = "Formato inválido";
                        }
                        break;
                    case "ColumnasCampo":
                        int col = 15;
                        if ((inF.text == "" || int.TryParse(inF.text, out col)) && col >= 2)
                        {
                            instancia.columnas = col;
                            colOk = true;
                        }
                        else
                        {
                            if (col < 2)
                                inF.text = "Columnas > a 2";
                            else
                                inF.text = "Formato inválido";
                        }
                        break;
                    case "FilasCampo":
                        int fil = 15;
                        if ((inF.text == "" || int.TryParse(inF.text, out fil)) && fil >= 2)
                        {
                            instancia.filas = fil;
                            filOk = true;
                        }
                        else
                        {
                            if (fil < 2)
                                inF.text = "Filas > a 2";
                            else
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

        //  Actualiza los valores del número total de npc-objetivos
        public void actualizaSlider(GameObject gm)
        {
            ent = (int)gm.GetComponent<Slider>().value;
        }

        //  Cambia la velocidad del jugador
        public void actualizaVelocidad(GameObject gm)
        {
            instancia.velocidad = gm.GetComponent<Slider>().value;
        }

        //  Activa/desactiva el full windows
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

        //  Cierra el juego
        public void cierraApp()
        {
            Application.Quit();
        }

        public void goMenu()
        {
            SceneManager.LoadScene("InitScene");
        }
    }
}
