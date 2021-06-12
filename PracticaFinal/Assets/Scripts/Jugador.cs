using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace luaberinto
{
    public enum estados { EnMision, Explorando, Terminado }

    public class Jugador : MonoBehaviour
    {
        //  Actual objeto que lleva el jugador en el bolsillo
        private ObjetivoBehaviour objetoEnBolsillo;
        //  Lista de objetos que conozco del laberinto
        private List<ObjetivoBehaviour> objetosConocidos;
        //  Lista de misiones que tiene actualmente el jugador
        private List<Mision> misionesActivas;
        //mision que estoy realizando
        private Mision misionActual;
        //  Lista de NPC que conozco
        private List<NPC> npcConocidos;
        //camino actual a recorrer
        Stack<Index> camino = null;
        //estado del jugador
        public estados estado_ = estados.Explorando;

        //para el movimiento del jugador
        //Vector2 derecha, izquierda, frente, atras, miDireccion;

        //lista para guardar el camino que ya he recorrido
        Stack<Index> caminoRecorrido;
        //Pila de los cruces por lo que he pasado
        Stack<Index> Cruces;

        Vector3 dir;

        public Index casillaActual { get; set; }

        // Cámara asociada al jugador
        private Camera jugadorCam;
        //  Cuerpo del modelo del jugador
        private GameObject cuerpo;

        private Text textNPC;
        private Text textObj;
        private Text textActOj;

        private void Awake()
        {
            caminoRecorrido = new Stack<Index>();
            Cruces = new Stack<Index>();
            dir = new Vector3(0, 0, 0);
        }

        // Start is called before the first frame update
        void Start()
        {
            objetosConocidos = new List<ObjetivoBehaviour>();
            misionesActivas = new List<Mision>();
            npcConocidos = new List<NPC>();
            casillaActual = new Index(0, 0);
            jugadorCam = GetComponentInChildren<Camera>();
            Camera.main.transform.parent = transform;
            cuerpo = gameObject.transform.GetChild(1).gameObject;

            var t = Canvas.FindObjectsOfType<Text>();
            foreach (Text txt in t)
            {
                switch (txt.name)
                {
                    case "Objetos":
                        textObj = txt;
                        break;
                    case "Npc":
                        textNPC = txt;
                        break;
                    case "ActualObjeto":
                        textActOj = txt;
                        break;
                }
            }
        }

        public void Update()
        {

            //movemos al jugador
            transform.Translate(dir * Time.deltaTime);

            //UI
            if (Input.GetKeyDown(KeyCode.Space))
                jugadorCam.enabled = false;
            else if (Input.GetKeyUp(KeyCode.Space))
                jugadorCam.enabled = true;
            textNPC.text = "NPCs :" + npcConocidos.Count + "/3";
            textObj.text = "objetivos :" + objetosConocidos.Count + "/3";
            textActOj.text = "estado actual:" + estado_.ToString();

            //  giros muy hardos TODO suavizar?
            cuerpo.transform.forward = dir;
        }

        //  Agrega un objeto a la lista de objetos que conozco
        public void actualizaConocimientos(ObjetivoBehaviour obj)
        {
            //si no conocemos el objeto
            if (!objetosConocidos.Contains(obj))
            {
                //lo añadimos a la lista
                Debug.Log("Objeto");
                objetosConocidos.Add(obj);
                ActualizaMision();
            }
            // si tengo una mision actual y se trata del objeto que estoy buscando
            if (misionActual != null && misionActual.getObjeto() == obj)
            {
                Debug.Log("Volvemos al NPC");
                agregaObjetoAlBolsillo(obj);
                dijkstra a = new dijkstra(LaberintoManager.instance.getGrafoLaberinto(), casillaActual, misionActual.getNPC().GetPos());
                camino = a.devuelveCamino();

                foreach( Index c in camino)
                {
                    LaberintoManager.instance.getCasillaByIndex(c.x, c.y).gameObject.GetComponent<Renderer>().material.color = Color.magenta;
                }

            }


        }


        //  Agrega un npc a la lista de npc que conozco
        public void actualizaConocimientos(NPC npc)
        {
            //si no esta en la lista(aun no lo conocemos)
            if (!npcConocidos.Contains(npc))
            {
                //lo añadimos a la lista
                npcConocidos.Add(npc);
            }
            else//si ya lo conociamos
            {
                //si es nuestra mision actual y tenemos el objeto se lo damos
                if (misionActual != null && misionActual.getNPC() == npc && objetoEnBolsillo == misionActual.getObjeto())
                {
                    npc.darObjeto();
                    misionActual.misionCompleta = true;
                    misionActual = null;
                    mueveSiguienteCasilla();

                }

            }
            ActualizaMision();
        }

        //actualizamos la misionActual y buscamos el objeto correspondiente a esa mision
        private void ActualizaMision()
        {
            foreach (NPC npc in npcConocidos)
            {
                //si la mision no esta ya completa
                if (!npc.getMision().misionCompleta)
                {
                    //conozco el objeto de la mision
                    if (objetosConocidos.Contains(npc.getMision().getObjeto()) && misionActual == null)
                    {
                        misionActual = npc.getMision();
                        dijkstra a = new dijkstra(LaberintoManager.instance.getGrafoLaberinto(), casillaActual, npc.getMision().getObjeto().GetPos());
                        camino = a.devuelveCamino();
                        foreach (Index c in camino)
                        {
                            LaberintoManager.instance.getCasillaByIndex(c.x, c.y).gameObject.GetComponent<Renderer>().material.color = Color.magenta;
                        }
                        estado_ = estados.EnMision;
                        return;
                    }


                }
            }
            if (estado_ != estados.Explorando)
            {
                mueveSiguienteCasilla();

            }
            estado_ = estados.Explorando;
        }

        //  Agrega un objeto al bolsillo
        public void agregaObjetoAlBolsillo(ObjetivoBehaviour obj)
        {
            objetoEnBolsillo = obj;
            obj.gameObject.SetActive(false);
        }

        //  Elimina el objeto que lleva el jugador en el bolsillo
        public void quitaObjetoDelBolsillo()
        {
            objetoEnBolsillo = null;
        }

        //  Intercambia un objeto del laberinto por el que lleva en el bolsillo
        public void cambiaObjetoDelBolsillo(ObjetivoBehaviour obj)
        {
            Instantiate(objetoEnBolsillo, obj.transform.position, Quaternion.identity);
            objetoEnBolsillo = obj;
            obj.objetoRecogido();
        }

        //  Agrega una misión a la lista de misiones 
        public void agregaMision(Mision mision)
        {
            if (!misionesActivas.Contains(mision))
                misionesActivas.Add(mision);
        }

        public void mueveSiguienteCasilla()
        {
            Graph grafo = LaberintoManager.instance.getGrafoLaberinto();
            //comprobamos si estamos en una interseccion
            if (grafo.nodes[casillaActual].Successors.Count > 2)//estamos en una interseccion
            {
                //Debug.Log("Interseccion");
                //no esta visitada todavia
                if (!grafo.nodes[casillaActual].visited)
                {
                    //Debug.Log("Primera vez que llegamos a la intersección: " + casillaActual.x + " , " + casillaActual.y);
                    //metemos esa casilla en la pila
                    Cruces.Push(casillaActual);
                    caminoRecorrido.Push(casillaActual);
                }

                int adyacentesVisitadas = 0;
                for (int i = 0; i < grafo.nodes[casillaActual].Successors.Count; i++)
                {
                    //elegimos el primer camino no visitado
                    if (!grafo.nodes[casillaActual].Successors[i].visited)
                    {
                        //cogemos la nueva casilla a la que se tiene que mover
                        Index ind = grafo.nodes[casillaActual].Successors[i].id;
                        Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);
                        //Debug.Log("Siguiente Casilla: " + ind.x + " , " + ind.y);

                        //calculamos la direccion en la que se tiene que mover
                        Vector3 aux = new Vector3(casilla.gameObject.transform.position.x, transform.position.y, casilla.gameObject.transform.position.z);

                        dir = (aux - transform.position).normalized;
                        break;

                    }
                    else adyacentesVisitadas++;
                }
                //si el ultimo ha sido visitado quiere decir que todos han sido visitados
                if (adyacentesVisitadas == grafo.nodes[casillaActual].Successors.Count)
                {
                    //Debug.Log("Todas las bifurcaciones visitadas");
                    //quitamos esta casilla del camino
                    caminoRecorrido.Pop();
                    //movemos al jugador a la casilla anterior
                    Index ind = caminoRecorrido.Peek();
                    //Debug.Log("Siguiente Casilla: " + ind.x + " , " + ind.y);
                    Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);
                    //calculamos la direccion en la que se tiene que mover
                    Vector3 aux = new Vector3(casilla.gameObject.transform.position.x, transform.position.y, casilla.gameObject.transform.position.z);

                    dir = (aux - transform.position).normalized;

                    //eliminamos el cruce de la lista
                    Cruces.Pop();
                }
            }
            else//no estamos en un cruce
            {
                //si la casilla no esta visitada
                if (!grafo.nodes[casillaActual].visited)
                {
                    //la añadimos al camino
                    caminoRecorrido.Push(casillaActual);
                    //avanzamos hacia delante
                    int adyacentesVisitadas = 0;
                    for (int i = 0; i < grafo.nodes[casillaActual].Successors.Count; i++)
                    {
                        //elegimos el camino no visitado
                        if (!grafo.nodes[casillaActual].Successors[i].visited)
                        {
                            //cogemos la nueva casilla a la que se tiene que mover
                            Index ind = grafo.nodes[casillaActual].Successors[i].id;
                            Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);

                            //calculamos la direccion en la que se tiene que mover
                            Vector3 aux = new Vector3(casilla.gameObject.transform.position.x, transform.position.y, casilla.gameObject.transform.position.z);

                            dir = (aux - transform.position).normalized;
                            break;
                        }
                        else adyacentesVisitadas++;
                    }

                    //Si no hemos entrado en ninguna de los adyacentes quiere decir que nos encontramos en un callejon
                    //sin salida
                    if (adyacentesVisitadas == grafo.nodes[casillaActual].Successors.Count)
                    {
                        caminoRecorrido.Pop();
                        //movemos al jugador a la casilla anterior
                        Index ind = caminoRecorrido.Peek();
                        Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);

                        //calculamos la direccion en la que se tiene que mover
                        Vector3 aux = new Vector3(casilla.gameObject.transform.position.x, transform.position.y, casilla.gameObject.transform.position.z);

                        dir = (aux - transform.position).normalized;
                    }
                }
                else//si la casilla si esta visitada
                {
                    //quitamos esta casilla del camino
                    caminoRecorrido.Pop();
                    //movemos al jugador a la casilla anterior
                    Index ind = caminoRecorrido.Peek();
                    Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);

                    //calculamos la direccion en la que se tiene que mover
                    Vector3 aux = new Vector3(casilla.gameObject.transform.position.x, transform.position.y, casilla.gameObject.transform.position.z);

                    dir = (aux - transform.position).normalized;
                }

            }
        }

        //sigue el camino para hacer una mision
        public void sigueCamino()
        {
            if (camino != null && camino.Count > 0)
            {
                //calculamos la direccion en la que se tiene que mover
                Index obj = camino.Pop();
                Debug.Log("Siguiente Casilla camino: " + obj.x + " , " + obj.y);
                Vector3 aux = new Vector3(LaberintoManager.instance.getCasillaByIndex(obj.x, obj.y).transform.position.x, transform.position.y, LaberintoManager.instance.getCasillaByIndex(obj.x, obj.y).transform.position.z);
                dir = (aux - transform.position).normalized;
            }
            else
            {
                estado_ = estados.Explorando;
            }
        }
    }
}