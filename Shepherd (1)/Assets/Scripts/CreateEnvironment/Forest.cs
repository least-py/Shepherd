using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{

    public GameObject firA;
    public GameObject firB;
    public GameObject firC;


    public string direction; //gets initialized by getBiggestScale()
    public float length;

    public int mirror;

    // Start is called before the first frame update
    void Start()
    {
        createTreeWall();

    }

    

    void createTreeWall()
    {
        float rndPos = Random.Range(-1.0f, 1.0f);
        float dirPos = 0.0f;

        float backgroundoffset = Random.Range(-3.5f, -2.0f) * mirror;

        if (direction.Equals("X"))
        {
            dirPos = transform.position.x - (length/2);
        }
        else
        {
            dirPos = transform.position.z - (length/2);
        }

        for (int metre = 0; metre <= length ; metre++){

            rndPos = Random.Range(-1.0f, 1.0f);
            float offset = Random.Range(0.0f, 0.5f);

            //Background trees
            if(metre % 2 == 0)
            {
                createATree(firA, dirPos, backgroundoffset, 7.0f);
                createATree(firB, dirPos + 0.5f, backgroundoffset, 4.75f);
            }
            else
            {
                createATree(firA, dirPos, backgroundoffset, 4.0f);
                createATree(firC, dirPos + 0.5f, backgroundoffset, 6.0f);
                
            }

            dirPos += offset;
            dirPos++;

            int tree = Random.Range(0, 3);
            if (tree == 0)
            {
                createATree(firA, dirPos, rndPos, 7.0f);
            }
            else if (tree == 1)
            {
                createATree(firB, dirPos, rndPos, 4.75f);
            }
            else //tree == 2
            {
                createATree(firC, dirPos, rndPos, 6.0f);
            }

            dirPos -= offset;
        }
    }
    //dirPos is the position on the "direction"-axis
    void createATree(GameObject tree, float dirPos, float rndPos, float size)
    {   
        if (direction.Equals("X"))
        {
            Instantiate(tree, new Vector3(dirPos, transform.position.y + size, transform.position.z + rndPos), Quaternion.Euler(new Vector3 (transform.localRotation.x, transform.localRotation.y, transform.localRotation.z)));
        }
        else //Z
        {
            Instantiate(tree, new Vector3(transform.position.x + rndPos, transform.position.y + size, dirPos), Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.y + 90, transform.localRotation.z)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
