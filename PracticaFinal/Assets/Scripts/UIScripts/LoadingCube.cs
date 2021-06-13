using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingCube : MonoBehaviour
{
    // Start is called before the first frame update
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
