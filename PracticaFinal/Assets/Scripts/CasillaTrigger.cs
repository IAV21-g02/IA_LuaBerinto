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
                Debug.Log("Llego a casilla: " + parent.myIndex.x + ", " + parent.myIndex.y);
                //cogemos el player
                Jugador player = other.gameObject.GetComponent<Jugador>();
                //actualizamos la casilla
                player.casillaActual = parent.myIndex;
                //cambiamos la direccion del jugador
                player.mueveSiguienteCasilla();
                //marcamos la casilla como visitada
                LaberintoManager.instance.getGrafoLaberinto().nodes[parent.myIndex].visited = true;
            }
        }
    }
}
