using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace luaberinto
{

    public class NPC : MonoBehaviour
    {
        //  Misión que este NPC ofrece
        private Mision miMision;
        private Index miPos;

        private void Start()
        {
            //miMision = new Mision(this, null);
        }
        public void setMision(Mision mision_)
        {
            miMision = mision_;
        }

        public Mision getMision()
        {
            return miMision;
        }

        public void darObjeto()
        {
            miMision.misionCompleta = true;
            // TODO gestionar las misiones desde manager
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Jugador"))
            {
                Debug.Log("NPC");
                other.GetComponent<Jugador>().actualizaConocimientos(this);
            }
            if (other.CompareTag("Casilla"))
            {
                miPos = other.GetComponent<Casilla>().getIndex();
                
            }
        }

        public Index GetPos() { return miPos; }
    }

}