using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace luaberinto
{

    public class Mision : MonoBehaviour
    {
        //  Npc que encarga esta misión
        private NPC npc;

        //  Objeto que pide esta mision
        private ObjetivoBehaviour objeto;

        //
        public bool misionCompleta = false;

        public Mision(NPC npc_, ObjetivoBehaviour obj_)
        {
            npc = npc_;
            objeto = obj_;
        }

        public ObjetivoBehaviour getObjeto()
        {
            return objeto;
        }

        public  NPC getNPC()
        {
            return npc;
        }
    }

}