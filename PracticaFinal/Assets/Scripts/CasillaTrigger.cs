using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace luaberinto
{
    public class CasillaTrigger : MonoBehaviour
    {
        public Casilla parent;
        private Renderer render;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Jugador"))
            {
                if(render)
                    render.material.color = Color.white;
                else
                {
                    render = LaberintoManager.instance.getCasillaByIndex(parent.getIndex().x, parent.getIndex().y).gameObject.GetComponent<Renderer>();
                    render.material.color = Color.white;
                }
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
                //  Si esta casilla es la final, se notifica al player
                if (LaberintoManager.instance.casillaFinal.Equals(parent))
                {
                    player.setCasillaFinal(parent);
                    if (player.misionesCompletadas == GameManager.instancia.ent)
                    {
                        //player.estado_ = estados.Terminado;
                        PanelFinal.instance.activaPanelFinal();
                    }
                }
            }
        }
    }
}
