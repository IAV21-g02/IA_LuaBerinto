using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public struct Index
{
    public int x;
    public int y;

    public Index(int x_, int y_)
    {
        x = x_;
        y = y_;
    }

}

public class LaberintoManager : MonoBehaviour
{
    //  Instancia de este singleton
    public static LaberintoManager instance;
    //  Matriz de casillas del laberinto
    private Casilla[,] casillas;
    //  Jugador
    private Jugador jugador;



    [SerializeField]
    private List<NavMeshSurface> surfaces;

    private List<NavMeshObstacle> obstacles;

    private List<ObjetivoBehaviour> objetivos;

    //
    private float limX;
    private float limZ;


    [Tooltip("Filas que componen este laberinto")]
    public int filas;
    [Tooltip("Columnas que componen este laberinto")]
    public int columnas;
    [Tooltip("Prefab usado para generar las casillas")]
    public Casilla casillaPrefab;
    [Tooltip("Prefab usado para generar muros")]
    public Muro muroPrefab;
    [Tooltip("Prefab usado para generar al jugador")]
    public Jugador jugadorPrefab;
    [Tooltip("Prefab usado para generar objetivos sobre el laberinto")]
    public ObjetivoBehaviour objetivoPrefab;
    [Tooltip("Coprimo Halton X")]
    public float baseX = 2;
    [Tooltip("Coprimo Halton Y")]
    public float baseY = 3;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        casillas = new Casilla[filas, columnas];
        surfaces = new List<NavMeshSurface>();
        obstacles = new List<NavMeshObstacle>();
        objetivos = new List<ObjetivoBehaviour>();
        ConstruyeLaberinto();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //  Devuelve la instancia de este singleton
    LaberintoManager GetInstance()
    {
        return instance;
    }

    //  Construye las casillas y los muros del laberinto
    private void ConstruyeLaberinto()
    {
        Transform actCasillaTr = casillaPrefab.transform;
        Vector3 initPos = Vector3.zero;
        actCasillaTr.position = initPos;
        for (int actFila = 0; actFila < filas; actFila++)
        {
            for (int actCol = 0; actCol < columnas; actCol++)
            {
                Casilla actCasilla = Instantiate(casillaPrefab, LaberintoManager.instance.transform);
                Index newIndex = new Index(actFila, actCol);
                actCasilla.Init(muroPrefab, newIndex);

                casillas[actFila, actCol] = actCasilla;

                actCasilla.transform.position = new Vector3(actCasillaTr.localScale.x * actCol + initPos.x, 0, actCasillaTr.localScale.z * actFila + initPos.z);

                if (actCol == 0)
                {
                    actCasilla.muroOeste.setMuroExterior();
                    if (actFila == 0)
                    {
                        actCasilla.muroSur.setMuroExterior();
                    }
                    else if (actFila == filas - 1)
                    {
                        actCasilla.muroNorte.setMuroExterior();
                    }
                }
                else if (actCol == columnas - 1)
                {
                    actCasilla.muroEste.setMuroExterior();
                    if (actFila == 0)
                    {
                        actCasilla.muroSur.setMuroExterior();
                    }
                    else if (actFila == filas - 1)
                    {
                        actCasilla.muroNorte.setMuroExterior();
                    }
                }

                if (actFila == 0)
                {
                    actCasilla.muroSur.setMuroExterior();

                }
                else if (actFila == filas - 1)
                {
                    actCasilla.muroNorte.setMuroExterior();
                }
            }
        }
        asignaAccesos();
        GeneraLaberinto();
    }

    // Asigna los obstaculos y suelos para su bake en runtime
    private void bakeInRunTime()
    {
        foreach (Casilla casilla in casillas)
        {
            surfaces.Add(casilla.GetComponent<NavMeshSurface>());

            Muro currMuro = casilla.muroNorte;
            if (currMuro != null && !currMuro.estaAbierto())
            {
                obstacles.Add(currMuro.GetComponent<NavMeshObstacle>());
            }
            currMuro = casilla.muroEste;
            if (currMuro != null && !currMuro.estaAbierto())
            {
                obstacles.Add(currMuro.GetComponent<NavMeshObstacle>());
            }
            currMuro = casilla.muroSur;
            if (currMuro != null && !currMuro.estaAbierto())
            {
                obstacles.Add(currMuro.GetComponent<NavMeshObstacle>());
            }
            currMuro = casilla.muroOeste;
            if (currMuro != null && !currMuro.estaAbierto())
            {
                obstacles.Add(currMuro.GetComponent<NavMeshObstacle>());
            }
        }

        foreach (NavMeshObstacle ob in obstacles)
        {
            ob.enabled = true;
        }


        foreach (NavMeshSurface s in surfaces)
        {
            s.BuildNavMesh();
        }

        //foreach (NavMeshObstacle ob in obstacles)
        //{
        //    //ob.
        //}
    }

    //Asigna los accesos de los muros 
    private void asignaAccesos()
    {
        foreach (Casilla casilla in casillas)
        {
            casilla.asignaAccesosMuros();
        }
    }

    //  Usando 
    private void GeneraLaberinto()
    {
        //  Array bidimensional para determinar si esta casilla ha sido visitada
        bool[,] visitado = new bool[filas, columnas];
        //  TO DO Elegir una casilla externa como inicial
        Casilla actCasilla = casillas[0, 0];
        CreaCamino(casillas[0, 0], visitado);
        instanciaJugador(actCasilla.transform);
        instanciaObjetivos();
        bakeInRunTime();
        jugador.GetComponent<NavMeshAgent>().SetDestination(casillas[filas - 1, columnas - 1].transform.position);
    }

    //  TODO
    //  Instancia los objetivos sobre el laberinto usando el algoritmo de Halton
    private void instanciaObjetivos()
    {
        int numPremios = filas / 3;
        int cont = 0;

        while (cont < numPremios)
        {
            int x = Random.Range(0, columnas);
            int y = Random.Range(0, filas);
            if (casillas[x, y].getObjetivo() == null)
            {
                Vector3 pos = casillas[x, y].transform.position;
                pos.y = 0.5f;
                casillas[x, y].setCasillaConObjetivo(Instantiate(objetivoPrefab, pos, Quaternion.identity));
                cont++;
            }
        }



        //limX = (filas * getAnchuraCasilla() + casillas[0, 0].transform.position.x) /*/ 10*/;
        //limZ = (columnas * getProfundidadCasilla() + casillas[0, casillas.GetLength(0) - 1].transform.position.z) /*/ 10*/;
        //Debug.Log(casillas[0, 0].transform.position.x);
        //Debug.Log(casillas[casillas.GetLength(0) - 1, 0].transform.position.z);
        //while (cont < numPremios)
        //{
        //    Halton2d(baseX, baseY, cont + 1);
        //    cont++;
        //}
    }

    //  Devuelve los index dentro de casillas de una casilla
    public Index getIndexByCasilla(Casilla actCasilla)
    {
        bool encontrado = false;
        int indx = 0;
        int indy = 0;
        do
        {
            if (casillas[indx, indy].Equals(actCasilla))
                encontrado = true;
            else if (indx < columnas)
            {
                indx++;
            }
            else if (indy < filas)
            {
                indy++;
            }
        }
        while (!encontrado && (indx < columnas || indy < filas));
        if (!encontrado) return new Index(-1, -1);
        return new Index(indx, indy);
    }


    //  Determina si las coordenadas dadas son válidas para casillas
    private bool casillaValida(int indX, int indY)
    {
        if (indX >= 0 && indY >= 0 && indX < columnas && indY < filas)
            return true;
        else return false;
    }

    //  Determina si las coordenadas dadas son válidas para casillas
    private bool casillaValida(Index index)
    {
        if (index.x >= 0 && index.y >= 0 && index.x < columnas && index.y < filas)
            return true;
        else return false;
    }

    //  Devuelve las casillas adyacentes y asequibles desde una casilla
    private List<Casilla> getCasillasAdyacentes(Casilla casilla)
    {
        List<Casilla> orientaciones = new List<Casilla>();
        Casilla newCasilla = getCasillaByOrientacion(casilla, Orientacion.NORTE);
        if (newCasilla != null)
            orientaciones.Add(newCasilla);
        newCasilla = getCasillaByOrientacion(casilla, Orientacion.ESTE);
        if (newCasilla != null)
            orientaciones.Add(newCasilla);
        newCasilla = getCasillaByOrientacion(casilla, Orientacion.SUR);
        if (newCasilla != null)
            orientaciones.Add(newCasilla);
        newCasilla = getCasillaByOrientacion(casilla, Orientacion.OESTE);
        if (newCasilla != null)
            orientaciones.Add(newCasilla);

        return orientaciones;
    }

    //  Devuelve la orientación entre dos casillas adyacentes
    private Orientacion getOrientacionEntreCasillas(Casilla a, Casilla b)
    {
        Index aIndex = a.getIndex();
        Index bIndex = b.getIndex();
        if (aIndex.x < bIndex.x)
        {
            return Orientacion.NORTE;
        }
        else if (aIndex.x > bIndex.x)
        {
            return Orientacion.SUR;
        }
        else if (aIndex.y < bIndex.y)
        {
            return Orientacion.OESTE;
        }
        else if (aIndex.y > bIndex.y)
        {
            return Orientacion.ESTE;
        }
        return Orientacion.NONE;
    }

    //  Crea los caminos del laberinto
    /*
 *      I = casilla inicial
 *      S = siguiente casilla
        _________________
        |_I___S_|___|___|
        |___|___|___|___|
        |___|___|___|___|

 */
    private void CreaCamino(Casilla casilla, bool[,] visitado)
    {
        int rndElec;
        Index ind = casilla.getIndex(); //  Sabemos que existe la casilla previamente

        //  Lista con todas las posibles salidas desde esta casilla
        List<Casilla> salidas = getCasillasAdyacentes(casilla);

        visitado[ind.x, ind.y] = true;
        //  Buscamos a que casilla ir desde esta casilla
        while (salidas.Count > 0)
        {
            //  Eligimos una casilla aleatoria de las posible salidas
            //Random.seed = Random.Range(0,100);
            //Random.seed = (int)Time.time;
            rndElec = Random.Range(0, salidas.Count);
            Casilla nextCasilla = salidas[rndElec];
            Index nextCasillaInd = nextCasilla.getIndex();
            if (!visitado[nextCasillaInd.x, nextCasillaInd.y])
            {
                //Debug.Log("[ " + nextCasillaInd.x + "," + nextCasillaInd.y + "]");
                nextCasilla.GetComponent<Renderer>().material.color = Color.magenta;
                quitaMuro(casilla, nextCasilla, getOrientacionEntreCasillas(casilla, nextCasilla));
                CreaCamino(nextCasilla, visitado);
            }
            else
            {
                salidas.Remove(salidas[rndElec]);
            }
        }
    }

    public float getAnchuraCasilla()
    {
        return casillaPrefab.GetComponent<Renderer>().bounds.size.x;
    }

    public float getProfundidadCasilla()
    {
        return casillaPrefab.GetComponent<Renderer>().bounds.size.z;
    }

    public Casilla getCasillaByIndex(int indx, int indy)
    {
        if (casillaValida(indx, indy))
        {
            return casillas[indx, indy];
        }
        else return null;
    }

    public Casilla getCasillaByOrientacion(Casilla casilla, Orientacion orientacion)
    {
        Casilla newCasilla = null;
        Index index = casilla.getIndex();

        switch (orientacion)
        {
            case Orientacion.NORTE:
                index.x++;
                if (casillaValida(index))
                {
                    newCasilla = casillas[index.x, index.y];
                }
                break;
            case Orientacion.ESTE:
                index.y++;
                if (casillaValida(index))
                {
                    newCasilla = casillas[index.x, index.y];
                }
                break;
            case Orientacion.SUR:
                index.x--;
                if (casillaValida(index))
                {
                    newCasilla = casillas[index.x, index.y];
                }
                break;
            case Orientacion.OESTE:
                index.y--;
                if (casillaValida(index))
                {
                    newCasilla = casillas[index.x, index.y];
                }
                break;
        }

        return newCasilla;
    }

    public Orientacion getOrientacionByNum(int num)
    {
        Orientacion currOrientacion = Orientacion.NONE;
        switch (num)
        {
            case 0:
                currOrientacion = Orientacion.NORTE;
                break;
            case 1:
                currOrientacion = Orientacion.ESTE;
                break;
            case 2:
                currOrientacion = Orientacion.SUR;
                break;
            case 3:
                currOrientacion = Orientacion.OESTE;
                break;
            default:
                currOrientacion = Orientacion.NONE;
                break;
        }
        return currOrientacion;
    }

    private void quitaMuro(Casilla a, Casilla b, Orientacion orientacion)
    {
        switch (orientacion)
        {
            case Orientacion.NORTE:
                a.muroNorte.quitaMuro();
                b.muroSur.quitaMuro();
                break;
            case Orientacion.ESTE:
                a.muroOeste.quitaMuro();
                b.muroEste.quitaMuro();
                break;
            case Orientacion.SUR:
                a.muroSur.quitaMuro();
                b.muroNorte.quitaMuro();
                break;
            case Orientacion.OESTE:
                a.muroEste.quitaMuro();
                b.muroOeste.quitaMuro();
                break;

        }
    }

    private void instanciaJugador(Transform tr)
    {
        Vector3 jugadorPos;
        jugadorPos.x = tr.transform.position.x;
        jugadorPos.y = 0.5f;
        jugadorPos.z = tr.transform.position.z;
        //jugadorPos.y += 0.5;
        jugador = Instantiate(jugadorPrefab, jugadorPos, Quaternion.identity);
    }

    /// <summary>
    /// Generador del mapa de Halton
    /// </summary>
    private void Halton2d(float baseX, float baseY, float index)
    {
        float x = Halton(baseX, index);
        float z = Halton(baseY, index);
        //Ajuste para pasar del rango [0,1] a las coordenadas reales del suelo
        float posX = -limX + (x * limX * 2);
        float posZ = -limZ + (z * limZ * 2);
        GameObject objeto = Instantiate(objetivoPrefab, new Vector3(posX, 0.5f, posZ), Quaternion.identity, transform).gameObject;
        objetivos.Add(objeto.GetComponent<ObjetivoBehaviour>());
        //tamaño random del objeto
        //float sizeX = Random.Range(0.5f, 2.5f);
        //float sizeZ = Random.Range(0.5f, 2.5f);
        //objeto.transform.localScale = new Vector3(sizeX, 3, sizeZ);
    }


    /// <summary>
    /// Metodo que dada una base y el indice del objeto
    /// en la secuencia de halton devuelve su valor correspondiente entr 0 y 1
    /// </summary>
    private float Halton(float b, float index)
    {
        float result = 0;
        float denominator = 1;

        while (index > 0)
        {
            denominator *= b;
            result += (index % b) / denominator;
            index = Mathf.Floor(index / b);
        }
        return result;
    }


}
