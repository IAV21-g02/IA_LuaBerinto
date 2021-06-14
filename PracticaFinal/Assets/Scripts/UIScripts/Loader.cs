using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace luaberinto
{
    public static class Loader
    {
        public static string nombreScena;

        public static void cargaNivel(string name)
        {
            nombreScena = name;
            SceneManager.LoadScene("LoadingScene");
        }
    }
}
