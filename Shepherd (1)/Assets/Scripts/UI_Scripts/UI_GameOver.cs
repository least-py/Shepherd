using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Restarting");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene("HermannTest");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {

    }
}
