using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace luaberinto
{

    public class Jugador : MonoBehaviour
    {
        //  Actual objeto que lleva el jugador en el bolsillo
        private ObjetivoBehaviour objetoEnBolsillo;
        //  Lista de objetos que conozco del laberinto
        private List<ObjetivoBehaviour> objetosConocidos;
        //  Lista de misiones que tiene actualmente el jugador
        private List<Mision> misionesActivas;
        //  Lista de NPC que conozco
        private List<NPC> npcConocidos;

        //para el movimiento del jugador
        //Vector2 derecha, izquierda, frente, atras, miDireccion;

        //lista para guardar el camino que ya he recorrido
        Stack<Index> caminoRecorrido;
        //Pila de los cruces por lo que he pasado
        Stack<Index> Cruces;

        Vector3 dir;

        public Index casillaActual { get; set; }

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
           

        }

        public void Update()
        {
            //movemos al jugador
            Debug.Log(dir);
            transform.Translate(dir);
        }

        //  Agrega un objeto a la lista de objetos que conozco
        public void actualizaConocimientos(ObjetivoBehaviour obj)
        {
            if (!objetosConocidos.Contains(obj))
                objetosConocidos.Add(obj);

            bool encontrado = false;
            int cont = 0;
            while (npcConocidos.Count > 0 && !encontrado && cont < npcConocidos.Count)
            {
                if (npcConocidos[cont].getMision().getObjeto().Equals(obj))
                {
                    if (objetoEnBolsillo == null)
                    {
                        objetoEnBolsillo = obj;
                        obj.objetoRecogido();
                    }
                    else
                    {
                        cambiaObjetoDelBolsillo(obj);
                    }
                    //TO ERASE
                    GetComponent<NavMeshAgent>().SetDestination(npcConocidos[cont].transform.position);
                    encontrado = true;
                }
                else cont++;
            }

            if (objetoEnBolsillo == null)
            {
                objetoEnBolsillo = obj;
                obj.objetoRecogido();
            }
        }

        //  Agrega un npc a la lista de npc que conozco
        public void actualizaConocimientos(NPC npc)
        {
            if (!npcConocidos.Contains(npc))
            {
                npcConocidos.Add(npc);
                if (objetoEnBolsillo != null && npc.getMision().getObjeto().Equals(objetoEnBolsillo))
                {
                    npc.darObjeto();
                    //TO DO mover hacia el objetivo

                }
                else if (objetosConocidos.Contains(npc.getMision().getObjeto()))
                {
                    //GetComponent<NavMeshAgent>().SetDestination(npc.getMision().getObjeto().transform.position);
                    //TO DO mover hacia el objetivo
                }
            }
        }

        //  Agrega un objeto al bolsillo
        public void agregaObjetoAlBolsillo(ObjetivoBehaviour obj)
        {
            objetoEnBolsillo = obj;
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

        //  Agrega una misi�n a la lista de misiones 
        public void agregaMision(Mision mision)
        {
            if (!misionesActivas.Contains(mision))
                misionesActivas.Add(mision);
        }

        //public void actualizaDirecciones(Orientacion dir)
        //{
        //    switch (dir)
        //    {
        //        case Orientacion.NORTE:
        //            derecha = new Vector2(0,1);
        //            izquierda = new Vector2(0,-1);
        //            frente = new Vector2(1,0);
        //            atras = new Vector2(-1,0);

        //            break;
        //        case Orientacion.SUR:
        //            derecha = new Vector2(0,-1);
        //            izquierda = new Vector2(0,1);
        //            frente = new Vector2(-1,0);
        //            atras = new Vector2(1,0);
        //            break;
        //        case Orientacion.ESTE:
        //            derecha = new Vector2(-1,0);
        //            izquierda = new Vector2(1,0);
        //            frente = new Vector2(0,1);
        //            atras = new Vector2(0,-1);
        //            break;
        //        case Orientacion.OESTE:
        //            derecha = new Vector2(1,0);
        //            izquierda = new Vector2(-1,0);
        //            frente = new Vector2(0,-1);
        //            atras = new Vector2(0,1);
        //            break;
        //    }
        //    miDireccion = frente;
        //}

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
                for (int i = 0; i < grafo.nodes[casillaActual].Successors.Count; i++)
                {
                    //elegimos el primer camino no visitado
                    if (!grafo.nodes[casillaActual].Successors[i].visited)
                    {
                        //cogemos la nueva casilla a la que se tiene que mover
                        Index ind = grafo.nodes[casillaActual].Successors[i].id;
                        Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);
                        //calculamos la direccion en la que se tiene que mover
                        dir = (casilla.gameObject.transform.position - transform.position).normalized;
                        break;

                    }
                }
                //si el ultimo ha sido visitado quiere decir que todos han sido visitados
                if (grafo.nodes[casillaActual].Successors[grafo.nodes[casillaActual].Successors.Count - 1].visited)
                {
                    //quitamos esta casilla del camino
                    caminoRecorrido.Pop();
                    //movemos al jugador a la casilla anterior
                    Index ind = caminoRecorrido.Peek();
                    Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);
                    //calculamos la direccion en la que se tiene que mover
                    dir = (casilla.gameObject.transform.position - transform.position).normalized;

                    //eliminamos el cruce de la lista
                    Cruces.Pop();

                }



            }
            else//no estamos en un cruce
            {
                //si la casilla no esta visitada
                if (!grafo.nodes[casillaActual].visited)
                {
                    //la a�adimos al camino
                    caminoRecorrido.Push(casillaActual);
                    //avanzamos hacia delante
                    for (int i = 0; i < grafo.nodes[casillaActual].Successors.Count; i++)
                    {
                        //elegimos el camino no visitado
                        if (!grafo.nodes[casillaActual].Successors[i].visited)
                        {
                            //cogemos la nueva casilla a la que se tiene que mover
                            Index ind = grafo.nodes[casillaActual].Successors[i].id;
                            Casilla casilla = LaberintoManager.instance.getCasillaByIndex(ind.x, ind.y);
                            //calculamos la direccion en la que se tiene que mover
                            dir = (casilla.gameObject.transform.position - transform.position).normalized;
                            break;
                        }
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
                    dir = (casilla.gameObject.transform.position - transform.position).normalized;


                }

            }
        }


    }

}