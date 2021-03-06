using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace luaberinto
{
    public class Casilla : MonoBehaviour
    {
        //  Index de esta casilla en las casillas del manager
        public Index myIndex { get; set; }

        //  Muro norte de esta casilla
        public Muro muroNorte;
        //  Muro este de esta casilla
        public Muro muroEste;
        //  Muro sur de esta casilla
        public Muro muroSur;
        //  Muro oeste de esta casilla
        public Muro muroOeste;

        private List<Muro> murosActivos;

        //  Objetivo que puede tener esta casilla
        private ObjetivoBehaviour objetivo;

        public Casilla()
        {
            muroNorte = new Muro();
            muroEste = new Muro();
            muroSur = new Muro();
            muroOeste = new Muro();
        }

        public void Init(Muro prefab, Index myIndex_)
        {
            myIndex = myIndex_;
            GetComponentInChildren<TextMesh>().text = "[" + myIndex.x + " , " + myIndex.y + "]";

            muroNorte = Instantiate(prefab, transform);
            muroNorte.Orienta(Orientacion.NORTE);

            muroEste = Instantiate(prefab, transform);
            muroEste.Orienta(Orientacion.ESTE);

            muroSur = Instantiate(prefab, transform);
            muroSur.Orienta(Orientacion.SUR);

            muroOeste = Instantiate(prefab, transform);
            muroOeste.Orienta(Orientacion.OESTE);
        }

        void Start()
        {
            murosActivos = new List<Muro>();
            objetivo = null;

        }

        public Muro getMuroPorOrientacion(int m)
        {
            Muro currMuro = null;
            switch (m)
            {
                case 0:
                    currMuro = muroNorte;
                    break;
                case 1:
                    currMuro = muroEste;
                    break;
                case 2:
                    currMuro = muroSur;
                    break;
                case 3:
                    currMuro = muroOeste;
                    break;
                default:
                    break;
            }
            return currMuro;
        }

        public Index getIndex()
        {
            return myIndex;
        }

        public void asignaAccesosMuros()
        {
            luaberinto.Casilla actCasilla = LaberintoManager.instance.getCasillaByOrientacion(this, Orientacion.NORTE);
            if (actCasilla)
            {
                muroNorte.setCasillaAcesso(actCasilla);
            }
            else muroNorte.setCasillaAcesso(null);

            actCasilla = LaberintoManager.instance.getCasillaByOrientacion(this, Orientacion.ESTE);
            if (actCasilla)
            {
                muroEste.setCasillaAcesso(actCasilla);
            }
            else muroEste.setCasillaAcesso(null);

            actCasilla = LaberintoManager.instance.getCasillaByOrientacion(this, Orientacion.SUR);
            if (actCasilla)
            {
                muroSur.setCasillaAcesso(actCasilla);
            }
            else muroSur.setCasillaAcesso(null);

            actCasilla = LaberintoManager.instance.getCasillaByOrientacion(this, Orientacion.OESTE);
            if (actCasilla)
            {
                muroOeste.setCasillaAcesso(actCasilla);
            }
            else muroOeste.setCasillaAcesso(null);
        }

        public void setCasillaConObjetivo(ObjetivoBehaviour obj)
        {
            objetivo = obj;
        }

        public ObjetivoBehaviour getObjetivo()
        {
            return objetivo;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Jugador"))
            {
                GetComponentInChildren<Light>().enabled = true;  
            }
        }


    }

}