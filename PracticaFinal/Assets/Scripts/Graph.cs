using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//LINK AL FORO DE STACK OVERFLOW:  stackoverflow.com/questions/21685552/graph-like-implementation-in-c-sharp
//COPYRIGHT BY LUABERINTO :D

//Clase grafo para representar las conexiones que hay entre las casillas de nuestro laberinto
public class Graph
{
    //Dicionario que dado el indice de la casilla devuelve el nodo asociado a la misma
    public Dictionary<Index, Node> nodes;

    //Constructora
    public Graph(Casilla[,] laberinto)
    {
        InitGraph(laberinto);
    }

    //Inicializaci�n del grafo segun el laberinto generado proceduralmente
    public void InitGraph(Casilla[,] casillas)
    {
        nodes = new Dictionary<Index, Node>();

        //Creacion de los nodos que conforman el grafo
        foreach (Casilla c in casillas)
        {
            nodes.Add(c.getIndex());
        }

        //A�adimos las conexiones necesarias
        foreach (Casilla c in casillas)
        {
            if (c.muroNorte.estaAbierto())
            {
                nodes.Connect(c.getIndex(), c.muroNorte.casillaAcceso.getIndex());
            }
            if (c.muroSur.estaAbierto())
            {
                nodes.Connect(c.getIndex(), c.muroSur.casillaAcceso.getIndex());
            }
            if (c.muroEste.estaAbierto())
            {
                nodes.Connect(c.getIndex(), c.muroEste.casillaAcceso.getIndex());
            }
            if (c.muroOeste.estaAbierto())
            {
                nodes.Connect(c.getIndex(), c.muroOeste.casillaAcceso.getIndex());
            }
        }
    }

    // Metodo que devuelve el diccionario como una lista por si fuese necesario en alg�n caso
    public List<Node> getGraphAsList()
    {
        return new List<Node>(nodes.Values);
    }
}

//Clase auxiliar para a�adir nodos y conexiones
public static class NodeHelper
{
    public static void Add(this Dictionary<Index, Node> dict, Index nodename)
    {
        dict.Add(nodename, new Node(nodename));
    }
    public static void Connect(this Dictionary<Index, Node> dict, Index from, Index to)
    {
        dict[from].Successors.Add(dict[to]);
        dict[to].Predecessors.Add(dict[from]);
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
    public List<Node> Predecessors { get; set; }
    public List<Node> Successors { get; set; }

    //  Constructora
    public Node()
    {
        visited = false;
    }

    // Constructora mediante indice
    public Node(Index name) : this()
    {
        this.id = name;
    }
}
