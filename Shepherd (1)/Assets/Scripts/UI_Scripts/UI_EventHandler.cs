using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_EventHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("EventHandler is alive");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartButtonClicked()
    {
        Debug.Log("StartButton clicked");
    }
}
