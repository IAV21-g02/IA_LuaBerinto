using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace luaberinto
{


    public class PanelFinal : MonoBehaviour
    {
        public static PanelFinal instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        public void activaPanelFinal()
        {
            GameObject gm = transform.GetChild(1).gameObject;
            gm.SetActive(true);
            gm.transform.GetChild(2).GetComponent<Text>().text = "Duración de la prueba : " + 
                LaberintoManager.instance.getTotalTiempo() + " segundos." ;
            LaberintoManager.instance.getJugador().GetComponent<AudioSource>().Stop();
        }
    }
}
