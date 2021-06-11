using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//LINK AL FORO DE STACK OVERFLOW:  stackoverflow.com/questions/21685552/graph-like-implementation-in-c-sharp
//COPYRIGHT BY LUABERINTO :D

//Clase grafo para representar las conexiones que hay entre las casillas de nuestro laberinto
namespace luaberinto
{

    public class Graph
    {
        //Dicionario que dado el indice de la casilla devuelve el nodo asociado a la misma
        public Dictionary<luaberinto.Index, Node> nodes;

        //Constructora
        public Graph(Casilla[,] laberinto)
        {
            InitGraph(laberinto);
            dijkstra a = new dijkstra(this, new Index(0, 0), new Index(0, 0));
        }

        //Inicialización del grafo segun el laberinto generado proceduralmente
        public void InitGraph(Casilla[,] casillas)
        {
            nodes = new Dictionary<Index, Node>();

            //Creacion de los nodos que conforman el grafo
            foreach (Casilla c in casillas)
            {
               Add(c.getIndex());
            }

            //Añadimos las conexiones necesarias
            foreach (Casilla c in casillas)
            {

                if (c.muroNorte.estaAbierto() && c.muroNorte.casillaAcceso)
                {
                    Connect(c.getIndex(), c.muroNorte.casillaAcceso.getIndex());
                }
                if (c.muroSur.estaAbierto() && c.muroSur.casillaAcceso)
                {
                    Connect(c.getIndex(), c.muroSur.casillaAcceso.getIndex());
                }
                if (c.muroEste.estaAbierto() && c.muroEste.casillaAcceso)
                {
                    Connect(c.getIndex(), c.muroEste.casillaAcceso.getIndex());
                }
                if (c.muroOeste.estaAbierto() && c.muroOeste.casillaAcceso)
                {
                    Connect(c.getIndex(), c.muroOeste.casillaAcceso.getIndex());
                }

            }
        }
        public void Add(Index nodename)
        {
            nodes.Add(nodename, new Node(nodename));
        }
        public void Connect( Index from, Index to)
        {
            nodes[from].Successors.Add(nodes[to]);
        }

        // Metodo que devuelve el diccionario como una lista por si fuese necesario en algún caso
        public List<Node> getGraphAsList()
        {
            return new List<Node>(nodes.Values);
        }
    }



    //  Clase nodo que representa de forma abstracta una casilla
    public class Node
    {
        //  Representa el indice de la casilla a la que representa en la clase LaberintoManager
        public Index id { get; set; }

        //  Si el nodo ha sido visitado alguna vez o no
        public bool visited { get; set; }

        //  Listas con los vecinos a este nodo
        public List<Node> Successors { get; set; }

        //  Constructora
        public Node()
        {
            visited = false;
            Successors = new List<Node>();
        }

        // Constructora mediante indice
        public Node(Index name) : this()
        {
            this.id = name;
        }


    }

    public class Pair : IComparable<Pair>
    {
       public Index index;
        public int valor;
       public Pair(Index i, int v)
        {
            index = i;
            valor = v;
        }

        public int CompareTo(Pair obj)
        {
           // if (obj == null) return 1;

            Pair otherPair = obj as Pair;
            if (otherPair != null)
                return this.valor.CompareTo(otherPair.valor);
            else
                throw new ArgumentException("Object is not a Temperature");
        }

        public bool Equals(Pair other)
        {
            return (other.index.x == this.index.x) && (other.index.y == this.index.y);
        }
    }

        

    public class dijkstra
    {
        
        private Graph grafo;
        Index ini;
        Index fin;
        int[] distancias;
        bool[] visitados;
        Node[] ulti;
        Priority_Queue<Pair> pq;
        public dijkstra(Graph graf, Index x, Index f)
        {
            grafo = graf;
            ini = x;
            fin = f;
            distancias = new int[grafo.nodes.Count];
            visitados = new bool[grafo.nodes.Count];
            ulti = new Node[grafo.nodes.Count];
            pq = new Priority_Queue<Pair>();

            //inicializamos los arrays
            for ( int i = 0; i< grafo.nodes.Count; i++)
            {
                distancias[i] = int.MaxValue;
                visitados[i] = false;
            }

            distancias[ini.x * LaberintoManager.instance.columnas + ini.y] = 0;
            pq.Add(new Pair(ini,0));
            while (pq.Count != 0)
            {
                Pair v = pq.Remove();
                foreach (Node a in grafo.nodes[v.index].Successors)
                    relajar(a, grafo.nodes[v.index]);
            }

        }

        private void relajar(Node a, Node b)
        {

            int destino = a.id.x * LaberintoManager.instance.columnas + a.id.y;
            int origen = b.id.x * LaberintoManager.instance.columnas + b.id.y;

            if (distancias[destino] > distancias[origen] + 1)
            {
                distancias[destino] = distancias[origen] +1;
                ulti[destino] = a;
               bool aux = pq.Contains(new Pair(a.id,0));
                if (aux)
                {
                    pq.Remove(new Pair(a.id, 0));
                }
                pq.Add(new Pair(a.id, distancias[destino]));
            }
        }
    }

}