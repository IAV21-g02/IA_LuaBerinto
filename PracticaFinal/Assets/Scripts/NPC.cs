using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    //  Misión que este NPC ofrece
    private Mision miMision;

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
    }
}
