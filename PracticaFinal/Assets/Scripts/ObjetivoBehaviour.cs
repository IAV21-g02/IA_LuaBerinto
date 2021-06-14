using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace luaberinto
{
    public class ObjetivoBehaviour : MonoBehaviour
    {
        private bool enBolsillo = false;
        private Index miPos;

        void Start()
        {

        }

        void Update()
        {
            if (!enBolsillo)
                transform.Rotate(Vector3.up, 0.5f);
            else
                transform.RotateAround(LaberintoManager.instance.getPosJugador(), Vector3.up, 1);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Jugador"))
            {
                other.GetComponent<Jugador>().actualizaConocimientos(this);
            }
            if (other.CompareTag("Casilla"))
            {
                miPos = other.GetComponent<Casilla>().getIndex();
                other.GetComponentInChildren<Light>().color = Color.green;
            }
        }

        public void objetoRecogido()
        {
            transform.position = LaberintoManager.instance.getPosJugador() + new Vector3(0.25f, 0, 0);
            enBolsillo = true;
        }

        public Index GetPos() { return miPos; }

    }

}