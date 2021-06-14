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
        private Index misionObjIndex = new Index(-1, -1);
        // Cámara asociada al jugador
        private Camera jugadorCam;
        //  Cuerpo del modelo del jugador
        private GameObject cuerpo;

        private Text textNPC;
        private Text textObj;
        private Text textActOj;
        private Text textTotalMisiones;


        private Casilla casillaFinal = null;

        public int misionesCompletadas = 0;

        //Velocidad del jugador al recorrer el laberinto
        private float velocidad;

        //  Tiempo que tarda el jugador en recorrer el laberinto
        public float totalTiempo = 0.0f;

        private float initTiempo = 0.0f;

        private void Awake()
        {
            caminoRecorrido = new Stack<Index>();
            Cruces = new Stack<Index>();
            dir = new Vector3(0, 0, 0);
        }

        // Start is called before the first frame update
        void Start()
        {
            initTiempo = Time.realtimeSinceStartup;
            objetosConocidos = new List<ObjetivoBehaviour>();
            misionesActivas = new List<Mision>();
            npcConocidos = new List<NPC>();
            casillaActual = new Index(0, 0);
            jugadorCam = GetComponentInChildren<Camera>();
            Camera.main.transform.parent = transform;
            jugadorCam.enabled = false;
            cuerpo = gameObject.transform.GetChild(1).gameObject;
            velocidad = GameManager.instancia.velocidad;

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
                    case "TotalMisionesComp":
                        textTotalMisiones = txt;
                        break;
                }
            }
        }

        public void Update()
        {
            //movemos al jugador
            if (estado_ != estados.Terminado)
            {
                transform.Translate(dir * Time.deltaTime * velocidad);
            }


            //UI
            if (Input.GetKeyDown(KeyCode.Space))
                jugadorCam.enabled = !jugadorCam.enabled;

            textNPC.text = "NPCs :" + npcConocidos.Count + "/" + GameManager.instancia.ent;
            textObj.text = "objetivos :" + objetosConocidos.Count + "/" + GameManager.instancia.ent;
            textActOj.text = "estado actual:" + estado_.ToString();
            textTotalMisiones.text = "Misiones completadas :" + misionesCompletadas + "/" + GameManager.instancia.ent;

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
                objetosConocidos.Add(obj);

                if (misionActual == null)
                {
                    ActualizaMision();
                }
            }
            // si tengo una mision actual y se trata del objeto que estoy buscando
            if (misionActual != null && misionActual.getObjeto() == obj)
            {
                agregaObjetoAlBolsillo(obj);

                //Usamos dijkstra para volver al npc por el camino mas corto
                dijkstra a = new dijkstra(LaberintoManager.instance.getGrafoLaberinto(), casillaActual, misionActual.getNPC().GetPos());
                camino = a.devuelveCamino();

                foreach (Index c in camino)
                {
                    LaberintoManager.instance.getCasillaByIndex(c.x, c.y).gameObject.GetComponent<Renderer>().material.color = Color.cyan;
                }
            }
        }
        //  Agrega un npc a la lista de npc que conozco
        public void actualizaConocimientos(NPC npc)
        {
            //si no esta en la lista(aun no lo conocemos)
            if (!npcConocidos.Contains(npc)) npcConocidos.Add(npc);
            else//si ya lo conociamos
            {
                //si es nuestra mision actual y tenemos el objeto se lo damos
                if (misionActual != null && misionActual.getNPC() == npc && objetoEnBolsillo == misionActual.getObjeto())
                {
                    //Entregamos objeto
                    npc.darObjeto();
                    //Marcamos la mision como completada y nos quedamos sin mision activa
                    misionActual.misionCompleta = true;
                    //
                    misionesCompletadas++;

                    //Cogemos el indice de la casilla del player
                    casillaActual = npc.GetPos();
                    if (misionesCompletadas < GameManager.instancia.ent && !misionObjIndex.Equals(casillaActual)) //Si no nos encontramos en la ultima casilla explorada
                    {
                        //Hacemos dijkstra para calcular el camino mas corto a dicha casilla
                        dijkstra a = new dijkstra(LaberintoManager.instance.getGrafoLaberinto(), casillaActual, misionObjIndex);
                        camino = a.devuelveCamino();
                        //Pintamos el camino
                        foreach (Index c in camino)
                        {
                            LaberintoManager.instance.getCasillaByIndex(c.x, c.y).gameObject.GetComponent<Renderer>().material.color = Color.cyan;
                        }
                        //Empezamos a movernos segun dicho camino que hemos calculado
                        sigueCamino();
                    }
                    else if (casillaFinal != null && misionesCompletadas == GameManager.instancia.ent)
                    {
                        dijkstra a = new dijkstra(LaberintoManager.instance.getGrafoLaberinto(), casillaActual, casillaFinal.getIndex());
                        camino = a.devuelveCamino();
                        foreach (Index c in camino)
                        {
                            LaberintoManager.instance.getCasillaByIndex(c.x, c.y).gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                        }
                        sigueCamino();
                    }
                }
            }
            //Para que si colisionamos con otros NPCs y no estamos haciendo una mision
            //Actualicemos a ver si podemos hacer alguna otra.
            if (misionActual == null) ActualizaMision();
        }

        //actualizamos la misionActual y buscamos el objeto correspondiente a esa mision
        private void ActualizaMision()
        {
            misionObjIndex = new Index(-1, -1);
            foreach (NPC npc in npcConocidos)
            {
                //si la mision no esta ya completa
                if (!npc.getMision().misionCompleta)
                {
                    //conozco el objeto de la mision
                    if (objetosConocidos.Contains(npc.getMision().getObjeto()) && misionActual == null)
                    {
                        //Actualizo mi misión actual
                        misionActual = npc.getMision();
                        //calculo el camino desde mi casilla actual al objeto que tengo que recoger para hacer la mision
                        dijkstra a = new dijkstra(LaberintoManager.instance.getGrafoLaberinto(), casillaActual, npc.getMision().getObjeto().GetPos());
                        camino = a.devuelveCamino();

                        //Pinto dicho camino
                        foreach (Index c in camino)
                        {
                            LaberintoManager.instance.getCasillaByIndex(c.x, c.y).gameObject.GetComponent<Renderer>().material.color = Color.cyan;
                        }

                        //Cambio mi estado a en mision
                        estado_ = estados.EnMision;

                        //Cogemos la lista de casillas adyacentes a la del NPC puesto que colisionamos antes con el
                        //que con el trigger del centro de la casilla
                        List<Casilla> adyNPC = LaberintoManager.instance.
                            getCasillasAdyacentes(LaberintoManager.instance.getCasillaByIndex(npc.GetPos().x, npc.GetPos().y));

                        //Compruebo cual es la casilla a la que tengo que volver cuando acabe la mision
                        //es decir si tengo que volver a la casilla del npc o a la del objeto cuando acabe
                        //esta mision para seguir explorando
                        if (adyNPC.Contains(LaberintoManager.instance.getCasillaByIndex(casillaActual.x, casillaActual.y)))
                        {
                            misionObjIndex = npc.GetPos();
                        }
                        else misionObjIndex = npc.getMision().getObjeto().GetPos();
                        return;
                    }
                }
            }
            if (estado_ != estados.Explorando)
            {
                mueveSiguienteCasilla();
                estado_ = estados.Explorando;
            }
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
                //no esta visitada todavia
                if (!grafo.nodes[casillaActual].visited)
                {
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
                        eligeDireccionMover(grafo, i);
                        break;
                    }
                    else adyacentesVisitadas++;
                }
                //si el ultimo ha sido visitado quiere decir que todos han sido visitados
                if (adyacentesVisitadas == grafo.nodes[casillaActual].Successors.Count)
                {
                    //Quitamos esa casilla del camino
                    quitaCasillaCamino();
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
                    mueveDesdeCasillaNormal(grafo);
                }
                else//si la casilla si esta visitada
                {
                    //quitamos esta casilla del camino
                    quitaCasillaCamino();
                }

            }
        }

        private void eligeDireccionMover(Graph grafo, int i)
        {
            //cogemos la nueva casilla a la que se tiene que mover
            Index ind = grafo.nodes[casillaActual].Successors[i].id;
            Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);
            //calculamos la direccion en la que se tiene que mover
            Vector3 aux = new Vector3(casilla.gameObject.transform.position.x, transform.position.y, casilla.gameObject.transform.position.z);

            dir = (aux - transform.position).normalized;
        }

        private void quitaCasillaCamino()
        {
            //quitamos esta casilla del camino
            if (caminoRecorrido.Count > 0)
            {
                caminoRecorrido.Pop();
                //movemos al jugador a la casilla anterior
                if (caminoRecorrido.Count > 0)
                {
                    Index ind = caminoRecorrido.Peek();
                    Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);
                    //calculamos la direccion en la que se tiene que mover
                    Vector3 aux = new Vector3(casilla.gameObject.transform.position.x, transform.position.y, casilla.gameObject.transform.position.z);
                    dir = (aux - transform.position).normalized;
                }
                else dir = new Vector3(0, 0, 0);
            }
            else dir = new Vector3(0, 0, 0);
        }

        private void mueveDesdeCasillaNormal(Graph grafo)
        {
            //avanzamos hacia delante
            int adyacentesVisitadas = 0;
            for (int i = 0; i < grafo.nodes[casillaActual].Successors.Count; i++)
            {
                //elegimos el camino no visitado
                if (!grafo.nodes[casillaActual].Successors[i].visited)
                {
                    eligeDireccionMover(grafo, i);
                    break;
                }
                else adyacentesVisitadas++;
            }

            //Si no hemos entrado en ninguna de los adyacentes quiere decir que nos encontramos en un callejon
            //sin salida
            if (adyacentesVisitadas == grafo.nodes[casillaActual].Successors.Count)
            {
                //quitamos esta casilla del camino
                quitaCasillaCamino();
            }
        }

        //sigue el camino para hacer una mision
        public void sigueCamino()
        {
            if (camino != null && camino.Count > 0)
            {
                //calculamos la direccion en la que se tiene que mover
                Index obj = camino.Pop();
                Vector3 aux = new Vector3(LaberintoManager.instance.getCasillaByIndex(obj.x, obj.y).transform.position.x, transform.position.y, LaberintoManager.instance.getCasillaByIndex(obj.x, obj.y).transform.position.z);
                dir = (aux - transform.position).normalized;
            }
            else
            {
                //Cambiamos de estado a explorando si no hay mas camino
                //Reseteamos a valores por defecto
                if (misionesCompletadas < GameManager.instancia.ent)
                {
                    camino = null;
                    misionActual = null;
                    estado_ = estados.Explorando;

                    //Comprobamos si podemos realizar alguna mision
                    ActualizaMision();

                    //Continuamos explorando el laberinto
                    LaberintoManager.instance.getGrafoLaberinto().nodes[casillaActual].visited = false;
                    mueveSiguienteCasilla();
                }
                else
                {
                    estado_ = estados.Terminado;
                    PanelFinal.instance.activaPanelFinal();
                }
            }
        }

        public void setCasillaFinal(Casilla casilla)
        {
            casillaFinal = casilla;
        }

        public float getTotalTiempo()
        {
            return Time.realtimeSinceStartup - initTiempo;
        }
    }
}