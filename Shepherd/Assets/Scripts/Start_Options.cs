﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Start_Options : MonoBehaviour
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject Player;
    public GameObject Sheep;
    public GameObject wolf;
    public GameObject Traps;
    public Text Sheep_counter_text;

    public int start_sheeps = 8; //Number of start sheeps
    public float shepherd_range = 1.5f; //How far away from shepherd to spawn a sheep
    public float start_x = 20;  //Start position x
    public float start_y = 3;   //Start position y
    public float start_z = 20;  //Start position z

    public int number_sheeps = 0;

    public float SpawnWolf = 30f;
    public float SpawnTraps = 30f;

    // This script will simply instantiate the Prefab when the game starts.
    private void Start()
    {
        // Instantiate at position (0, 0, 0) and zero rotation.
        Instantiate(Player, new Vector3(start_x, start_y, start_z), Quaternion.identity);
        for (int i = 0; i < start_sheeps; i++)
        {
            Instantiate(Sheep, new Vector3(UnityEngine.Random.Range(start_x - shepherd_range, start_x + shepherd_range), start_y, UnityEngine.Random.Range(start_z - shepherd_range, start_z + shepherd_range)), Quaternion.identity);
        }
    }

    void OnEnable()
    {

    }

    void Update()
    {
        if (SpawnWolf > 0)
        {
            SpawnWolf -= Time.deltaTime;
        }
        else
        {
            Instantiate(wolf, new Vector3(UnityEngine.Random.Range(20f, 120f), start_y, UnityEngine.Random.Range(20f, 120f)), Quaternion.identity);
            SpawnWolf = UnityEngine.Random.Range(30f, 45f);
        }
        if (SpawnTraps > 0)
        {
            SpawnTraps -= Time.deltaTime;
        }
        else
        {
            Instantiate(Traps, new Vector3(UnityEngine.Random.Range(20f, 120f), 0.001f, UnityEngine.Random.Range(20f, 120f)), Quaternion.identity);
            SpawnTraps = UnityEngine.Random.Range(30f, 45f);
        }

        number_sheeps = GameObject.FindGameObjectsWithTag("Sheep").Length;
        Sheep_counter_text.text = "Schafe: " + number_sheeps;
        if(number_sheeps <= 0)
        {
            //end game
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
