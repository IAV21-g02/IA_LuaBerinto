using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace luaberinto
{
    public class LoadingCube : MonoBehaviour
    {
        void Start()
        {
            string mainSceneName = "MainScene";
            SceneManager.LoadSceneAsync(mainSceneName);
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 0f), Random.Range(0.0f, 1.0f)),
                Time.deltaTime * Random.Range(50f, 100.0f));
        }
    }
}
