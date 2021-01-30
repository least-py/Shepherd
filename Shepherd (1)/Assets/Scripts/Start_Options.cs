using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Start_Options : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject Player;
    public GameObject Sheep;
    public Text Sheep_counter_text;

    public int start_sheeps = 8; //Number of start sheeps
    public float shepherd_range = 1.5f; //How far away from shepherd to spawn a sheep
    public float start_x = 20;  //Start position x
    public float start_y = 0;   //Start position y
    public float start_z = 20;  //Start position z

    public int number_sheeps = 0;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        // Instantiate at position (0, 0, 0) and zero rotation.
        Instantiate(Player, new Vector3(start_x, start_y, start_z), Quaternion.identity);
        for (int i = 0; i < start_sheeps; i++)
        {
            Instantiate(Sheep, new Vector3(Random.Range(start_x- shepherd_range, start_x+ shepherd_range), start_y, Random.Range(start_z- shepherd_range, start_z+ shepherd_range)), Quaternion.identity);
        }
    }

    void Update()
    {
        number_sheeps = GameObject.FindGameObjectsWithTag("Sheep").Length;
        Sheep_counter_text.text = "Schafe: " + number_sheeps;
    }
}
