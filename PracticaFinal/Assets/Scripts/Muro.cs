using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Orientacion : int
{
    NORTE, ESTE, SUR, OESTE , NONE
}

public class Muro : MonoBehaviour
{
    //  Orientación de este muro
    public Orientacion orientacion;

    //  Variable que determina si este muro se puede quitar
    private bool inamovible = false;
    //  Variable que determina si este muro está abierto
    private bool abierto = false;
    //  Casilla a la que este muro da acceso
    public Casilla casillaAcceso;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //  Gira los muros en función de su orientación
    public void Orienta(Orientacion orientacion_)
    {
        orientacion = orientacion_;
        switch (orientacion_)
        {
            case Orientacion.NORTE:
                GetComponent<Renderer>().material.color = Color.red;
                transform.Rotate(Vector3.up, 90);
                transform.position = new Vector3(transform.parent.position.x, transform.position.y,
                    transform.parent.position.z + (LaberintoManager.instance.getProfundidadCasilla() / 2) - transform.localScale.z / 2);
                break;
            case Orientacion.OESTE:
                GetComponent<Renderer>().material.color = Color.blue;
                transform.position = new Vector3(transform.parent.position.x - (LaberintoManager.instance.getProfundidadCasilla() / 2) + (transform.localScale.z / 2),
                    transform.position.y, transform.position.z);
                break;
            case Orientacion.SUR:
                GetComponent<Renderer>().material.color = Color.green;
                transform.Rotate(Vector3.up, -90);
                transform.position = new Vector3(transform.parent.position.x, transform.position.y,
                    transform.parent.position.z - (LaberintoManager.instance.getProfundidadCasilla() / 2) + transform.localScale.z / 2);
                break;
            case Orientacion.ESTE:
                GetComponent<Renderer>().material.color = Color.yellow;
                transform.position = new Vector3(transform.parent.position.x + (LaberintoManager.instance.getProfundidadCasilla() / 2) - transform.localScale.z / 2,
                    transform.position.y, transform.position.z);
                break;
            default:
                break;

        }
    }

    public void quitaMuro()
    {
        if (!inamovible)
        {
            Destroy(this.gameObject);
        }
    }

    public void setMuroExterior()
    {
        inamovible = true;
        GetComponent<Renderer>().material.color = Color.black;
    }

    public Casilla getAcceso()
    {
        return casillaAcceso;
    }

    public void setCasillaAcesso(Casilla newAcceso)
    {
        if (newAcceso == null)
        {
            inamovible = true;
        }
        casillaAcceso = newAcceso;
    }

    public void coloreaAcceso()
    {
        if (!inamovible) 
            casillaAcceso.GetComponent<Renderer>().material.color = Color.magenta;
    }

    public void decoloreaAcceso()
    {
        if (!inamovible)
            casillaAcceso.GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnMouseDown()
    {
        coloreaAcceso();
        Invoke("decoloreaAcceso",3);
    }

    public bool estaAbierto()
    {
        return abierto;
    }
}
