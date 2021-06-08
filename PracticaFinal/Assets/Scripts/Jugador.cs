using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        objetosConocidos = new List<ObjetivoBehaviour>();
        misionesActivas = new List<Mision>();
        npcConocidos = new List<NPC>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //  Agrega un objeto a la lista de objetos que conozco
    public void actualizaConocimientos(ObjetivoBehaviour obj)
    {
        if (objetoEnBolsillo == null)
        {
            objetoEnBolsillo = obj;
            obj.objetoRecogido();
        }


        if (!objetosConocidos.Contains(obj))
            objetosConocidos.Add(obj);
    }

    //  Agrega un npc a la lista de npc que conozco
    public void actualizaConocimientos(NPC npc)
    {
        if (!npcConocidos.Contains(npc))
            npcConocidos.Add(npc);
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
    }

    //  Agrega una misión a la lista de misiones 
    public void agregaMision(Mision mision)
    {
        if (!misionesActivas.Contains(mision))
            misionesActivas.Add(mision);
    }   
}
