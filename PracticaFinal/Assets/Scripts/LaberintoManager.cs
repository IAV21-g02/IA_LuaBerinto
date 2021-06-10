using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace luaberinto
{
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
        //  Caras superiores de los objetos que funcionaran como navMeshArea
        private List<NavMeshSurface> surfaces;
        //  Obstáculos (aka muros)
        private List<NavMeshObstacle> obstacles;
        //  Objetos que hay que recoger con la IA
        public List<ObjetivoBehaviour> objetivos;
        //  Ancho de la superficie de navegacion
        private float limX;
        //  Largo de la superficie de navegacion
        private float limZ;
        //  Posicion inicial desde la que se empieza a crear el laberinto
        private Vector3 initPos = Vector3.zero;
        //  Grafo que representa el laberinto para mover al player
        private Graph grafoLaberinto;
        //  Lista de npcs disponibles 
        public List<GameObject> npcs;


      

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
        [Tooltip("Número de objetos a obtener y llevar a su respectivo NPC")]
        public int numPremios = 10;

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
            //Inicializacion de matrices y listas
            casillas = new Casilla[filas, columnas];
            surfaces = new List<NavMeshSurface>();
            obstacles = new List<NavMeshObstacle>();
            //objetivos = new List<ObjetivoBehaviour>();

            //Inicializacion de variables necesarias para la secuencia de Halton
            limX = (filas * getAnchuraCasilla());
            limZ = (columnas * getProfundidadCasilla());

            ConstruyeLaberinto();
        }

        //  Devuelve la instancia de este singleton
        public LaberintoManager GetInstance()
        {
            return instance;
        }

        //  Construye las casillas y los muros del laberinto
        private void ConstruyeLaberinto()
        {
            Transform actCasillaTr = casillaPrefab.transform;

            actCasillaTr.position = initPos;
            for (int actFila = 0; actFila < filas; actFila++)
            {
                for (int actCol = 0; actCol < columnas; actCol++)
                {
                    Casilla actCasilla = Instantiate(casillaPrefab, LaberintoManager.instance.transform);
                    luaberinto.Index newIndex = new luaberinto.Index(actFila, actCol);
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
            creaMisiones();

            grafoLaberinto = new Graph(casillas);
   
           
        }

        //  Instancia los objetivos sobre el laberinto usando el algoritmo de Halton
        private void instanciaObjetivos()
        {
            int cont = 0;

            while (cont < numPremios)
            {
                //Lo añadimos a la lista de objetivos
                objetivos.Add(Halton2d(baseX, baseY, cont + 1, objetivoPrefab.gameObject).GetComponent<ObjetivoBehaviour>());
                cont++;
            }
        }

        //  Devuelve los index dentro de casillas de una casilla
        public luaberinto.Index getIndexByCasilla(Casilla actCasilla)
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
        private Orientacion getOrientacionEntreCasillas(Casilla origen, Casilla destino)
        {
            Index oriIndex = origen.getIndex();
            Index destIndex = destino.getIndex();
            if (oriIndex.x < destIndex.x)
            {
                return Orientacion.NORTE;
            }
            else if (oriIndex.x > destIndex.x)
            {
                return Orientacion.SUR;
            }
            else if (oriIndex.y < destIndex.y)
            {
                return Orientacion.OESTE;
            }
            else if (oriIndex.y > destIndex.y)
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
                    //nextCasilla.GetComponent<Renderer>().material.color = Color.magenta;
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
            jugador = Instantiate(jugadorPrefab, jugadorPos, Quaternion.identity);
        }

        /// <summary>
        /// Generador del mapa de Halton
        /// </summary>
        private GameObject Halton2d(float baseX, float baseY, float index, GameObject prefab)
        {
            //Ajuste para pasar del rango [0,1] a las coordenadas reales del suelo
            float posX = initPos.x + (adjustHaltonToGrid(Halton(baseX, index), columnas) * limX);
            float posZ = initPos.z + (adjustHaltonToGrid(Halton(baseY, index), filas) * limZ);

            //Creamos el objeto que nos servirá como prefab
            return Instantiate(prefab, new Vector3(posX, 0.5f, posZ), Quaternion.identity, transform).gameObject;
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

        /// <summary>
        /// Metodo para centrar en las casillas los objetos pickables que creemos
        /// </summary>
        private float adjustHaltonToGrid(float haltonResult, int maxDivisions)
        {
            float adj = 0.0f;
            float divPercentage = 1.0f / maxDivisions;

            while (divPercentage / 2 < haltonResult - adj)
            {
                adj += divPercentage;
            }

            return adj;
        }

        public Vector3 getPosJugador()
        {
            return jugador.transform.position;
        }

        public Graph getGrafoLaberinto()
        {
            return grafoLaberinto;
        }

        private void creaMisiones()
        {
            int num = npcs.Count;
            for (int i = 0; i < num; i++)
            {
                int aux = Random.Range(0, npcs.Count);
                NPC npc = Halton2d(baseX, baseY, i + 1, npcs[aux].gameObject).GetComponent<NPC>();
                npc.transform.position = new Vector3(npc.transform.position.x, 0.25f, npc.transform.position.z);

                int aux2 = Random.Range(0, objetivos.Count);
                ObjetivoBehaviour obj = Halton2d(baseX, baseY, i + num, objetivos[aux].gameObject).GetComponent<ObjetivoBehaviour>();

                npc.setMision(new Mision(npc, obj));

                npcs.Remove(npcs[aux]);
                objetivos.Remove(objetivos[aux2]);
            }
        }
    }

}