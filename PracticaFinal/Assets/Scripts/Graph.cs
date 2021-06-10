using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Clase auxiliar para añadir nodos y conexiones
    public static class NodeHelper
    {

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

}