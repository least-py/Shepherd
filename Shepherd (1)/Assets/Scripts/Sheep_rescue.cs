using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sheep_rescue : MonoBehaviour
{
    SC_DeerAI ai;
    public Sprite qte_links;
    public Sprite qte_rechts;
    public Sprite qte_oben;
    public Sprite qte_unten;
    public Sprite qte_blank;


    // Start is called before the first frame update
    void Start()
    {
        ai = gameObject.GetComponent<SC_DeerAI>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("space"))
        {
            if (Vector3.Distance(ai.player.transform.position, transform.position) < ai.awarenessArea)
            {
                if (ai.kombi.Count != 0)
                {
                    Debug.Log(ai.kombi[0]);
                    string todo = ai.kombi[0];
                    if (todo == "w")
                    {
                        GameObject.Find("QuickTimeEvent").GetComponent<Image>().sprite = qte_oben;
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            ai.kombi.RemoveAt(0);
                        }
                    }
                    else if (todo == "a")
                    {
                        GameObject.Find("QuickTimeEvent").GetComponent<Image>().sprite = qte_links;
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            ai.kombi.RemoveAt(0);
                        }
                    }
                    else if (todo == "s")
                    {
                        GameObject.Find("QuickTimeEvent").GetComponent<Image>().sprite = qte_unten;
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            ai.kombi.RemoveAt(0);
                        }
                    }
                    else if (todo == "d")
                    {
                        GameObject.Find("QuickTimeEvent").GetComponent<Image>().sprite = qte_rechts;
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            ai.kombi.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    GameObject.Find("QuickTimeEvent").GetComponent<Image>().sprite = qte_blank;
                    if (ai.traps != null)
                        ai.traps.destroy = true;
                        ai.traps = null;
                    ai.trapped = false;
                }

            }




        }
        else
            GameObject.Find("QuickTimeEvent").GetComponent<Image>().sprite = qte_blank;
    }

    void OnTriggerEnter(Collider other)
    {
        
    }

}
