using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace luaberinto
{
    public class initGameButton : MonoBehaviour
    {
        public void playGame()
        {
            SceneManager.LoadScene("LoadingScene");
        }
    }
}
