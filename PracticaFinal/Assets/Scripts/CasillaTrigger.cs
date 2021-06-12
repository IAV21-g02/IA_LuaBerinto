using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace luaberinto
{
    public class CasillaTrigger : MonoBehaviour
    {
        public Casilla parent;

        private void Start()
        {
            if (parent == null) Debug.Log("ES NULL");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Jugador"))
            {
                LaberintoManager.instance.getCasillaByIndex(parent.getIndex().x, parent.getIndex().y).gameObject.GetComponent<Renderer>().material.color = Color.white;
                //Debug.Log("Llego a casilla: " + parent.myIndex.x + ", " + parent.myIndex.y);
                //cogemos el player
                Jugador player = other.gameObject.GetComponent<Jugador>();
                //actualizamos la casilla
                player.casillaActual = parent.myIndex;
                //cambiamos la direccion del jugador
                if (other.gameObject.GetComponent<Jugador>().estado_ == estados.Explorando)
                {
                    player.mueveSiguienteCasilla();
                }
                else
                {
                    player.sigueCamino();
                }
                //marcamos la casilla como visitada
                LaberintoManager.instance.getGrafoLaberinto().nodes[parent.myIndex].visited = true;
            }
        }
    }
}
