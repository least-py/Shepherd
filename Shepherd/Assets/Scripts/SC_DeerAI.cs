﻿//You are free to use this script in Free or Commercial projects
//sharpcoderblog.com @2019

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class SC_DeerAI : MonoBehaviour
{
    public enum AIState { Idle, Walking, Eating, Running, Panicing }
    public AIState currentState = AIState.Idle;
    public float awarenessArea = 1.5f; //How far the deer should detect the enemy
    public float walkingSpeed = 0.3f;
    public float runningSpeed = 1f;
    public Animator animator;

    public GameObject babySheep;
    public int pregnantCounter = 0;
    public float trappcounter = 10f;
    public bool trapped = false;
    public int quicktimeevents = 10;
    public int health = 10;



    // PUBLIC FUCKING POSITION UND SCALING CHANGES

    public Vector3 babysize = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 upscaling = new Vector3(0.1f, 0.1f, 0.1f);
    public float upperbound = 0.2f;

    public float yCorrection = 0.1f / 2;



    List<string> quicktimekeys = new List<string> { "w", "a", "s", "d" };

    public List<string> kombi = new List<string>();

    //Trigger collider that represents the awareness area
    SphereCollider c; 
    //NavMesh Agent
    NavMeshAgent agent;

    bool switchAction = false;
    float actionTimer = 0; //Timer duration till the next action
    Transform enemy;
    public float range = 0.8f; //How far the Deer have to run to resume the usual activities
    public float multiplier = 0.5f;
    bool reverseFlee = false; //In case the AI is stuck, send it to one of the original Idle points

    //Detect NavMesh edges to detect whether the AI is stuck
    Vector3 closestEdge;
    float distanceToEdge;
    float distance; //Squared distance to the enemy
    //How long the AI has been near the edge of NavMesh, if too long, send it to one of the random previousIdlePoints
    float timeStuck = 0;
    //Store previous idle points for reference
    List<Vector3> previousIdlePoints = new List<Vector3>();

    public SC_CharacterController player;
    public Traps traps;

    void Awake()
    {
        transform.localScale = babysize;
        //transform.position = new Vector3(transform.position.x, yCorrection , transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.autoBraking = true;

        //Initialize the AI state
        currentState = AIState.Idle;
        actionTimer = Random.Range(0.1f, 2.0f);
        SwitchAnimationState(currentState);

        pregnantCounter = 2;
            //Random.Range(10, 25);
        player = FindObjectOfType<SC_CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(transform.position.x, yCorrection, transform.position.z);
        if (traps == null)
            trapped = false;

        if (trapped) //Schaf in eine Falle gelaufen?
        {
            if (trappcounter > 0)           
                trappcounter -= Time.deltaTime;  
            else
                Destroy(this.gameObject);

        }
        else
        {
            //Give birth to the next sheep generation :)
            if (pregnantCounter == 0)
            {
                pregnantCounter = Random.Range(25, 50);
                yCorrection = babysize.y / 2.0f;
                GiveBirth();
                transform.localScale = babysize;
                //transform.position = new Vector3(transform.position.x, yCorrection, transform.position.z);

            }

            //Wait for the next course of action
            if (actionTimer > 0)
            {
                actionTimer -= Time.deltaTime;
            }
            else
            {
                switchAction = true;
            }

            if (Input.GetMouseButtonDown(1) && (Vector3.Distance(player.transform.position, transform.position) < awarenessArea))
            {
                enemy = player.transform;
                agent.SetDestination(RandomNavSphere(transform.position, Random.Range(1, 2.4f)));
                currentState = AIState.Running;
                SwitchAnimationState(currentState);
            }

            if (currentState == AIState.Idle)
            {
                if (switchAction)
                {
                    //No enemies nearby, start eating
                    actionTimer = Random.Range(3, 9);

                    currentState = AIState.Eating;
                    SwitchAnimationState(currentState);

                    //Keep last 5 Idle positions for future reference
                    previousIdlePoints.Add(transform.position);
                    if (previousIdlePoints.Count > 5)
                    {
                        previousIdlePoints.RemoveAt(0);
                    }
                }
            }else if (currentState == AIState.Panicing)
            {
                    actionTimer = Random.Range(1, 4);

                    currentState = AIState.Eating;
                    SwitchAnimationState(currentState);            
            }
            else if (currentState == AIState.Walking)
            {
                //Set NavMesh Agent Speed
                agent.speed = walkingSpeed;

                // Check if we've reached the destination
                if (DoneReachingDestination())
                {
                    currentState = AIState.Idle;
                }
            }
            else if (currentState == AIState.Eating)
            {

                if (switchAction)
                {
                    //Wait for current animation to finish playing
                    if (!animator || animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f)
                    {
                        if(transform.localScale.x <= upperbound)
                        {
                            transform.localScale += upscaling;
                            yCorrection += (upscaling.y / 2);
                            //transform.position = new Vector3(transform.position.x, yCorrection, transform.position.z);
                        }

                        pregnantCounter--;
                        //Walk to another random destination
                        agent.destination = RandomNavSphere(transform.position, Random.Range(3, 7));
                        currentState = AIState.Walking;
                        SwitchAnimationState(currentState);
                    }
                }
            }
            else if (currentState == AIState.Running)
            {
                //Set NavMesh Agent Speed
                agent.speed = runningSpeed;

                //Check if we've reached the destination then stop running
                if (DoneReachingDestination())
                {
                    actionTimer = Random.Range(1.4f, 3.4f);
                    currentState = AIState.Eating;
                    SwitchAnimationState(AIState.Idle);
                }

                if (reverseFlee)
                {
                    if (DoneReachingDestination() && timeStuck < 0)
                    {
                        reverseFlee = false;
                    }
                    else
                    {
                        timeStuck -= Time.deltaTime;
                    }
                }
                else
                {
                    if (enemy)
                    {
                        Vector3 runTo = transform.position + ((transform.position - enemy.position) * multiplier);
                        distance = (transform.position - enemy.position).sqrMagnitude / 4;

                        //Find the closest NavMesh edge
                        NavMeshHit hit;
                        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
                        {
                            closestEdge = hit.position;
                            distanceToEdge = hit.distance;
                            //Debug.DrawLine(transform.position, closestEdge, Color.red);
                        }

                        if (distanceToEdge < 1f)
                        {
                            if (timeStuck > 1.5f)
                            {
                                if (previousIdlePoints.Count > 0)
                                {
                                    runTo = previousIdlePoints[Random.Range(0, previousIdlePoints.Count - 1)];
                                    reverseFlee = true;
                                }
                            }
                            else
                            {
                                timeStuck += Time.deltaTime;
                            }
                        }

                        if (distance < range)
                        {
                            agent.SetDestination(runTo);
                        }
                        else
                        {
                            enemy = null;
                        }
                    }
                }

                //Temporarily switch to Idle if the Agent stopped
                if (agent.velocity.sqrMagnitude < 0.1f * 0.1f)
                {
                    SwitchAnimationState(AIState.Idle);
                }
                else
                {
                    SwitchAnimationState(AIState.Running);
                }

            }
            switchAction = false;
        }
    }

    void GiveBirth()
    {
        Instantiate(babySheep, transform.position, transform.rotation);
    }

    bool DoneReachingDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    //Done reaching the Destination
                    return true;
                }
            }
        }

        return false;
    }

    void SwitchAnimationState(AIState state)
    {
        //Animation control
        if (animator)
        {
            animator.SetBool("isEating", state == AIState.Eating);
            animator.SetBool("isRunning", state == AIState.Running);
            animator.SetBool("isWalking", state == AIState.Walking);
            animator.SetBool("isPanicing", state == AIState.Panicing);
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);

        return navHit.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Die"))
        {
            kombi = new List<string> {};

            for (int i = 0; i < quicktimeevents; i++)
                kombi.Add(quicktimekeys[Random.Range(0, 3)]);
            

            traps = other.GetComponent<Collider>().gameObject.GetComponent<Traps> ();
            trappcounter = traps.trapptime;
            trapped = true;
            actionTimer = 0;
            agent.destination = RandomNavSphere(transform.position, 0);
            currentState = AIState.Panicing;
            SwitchAnimationState(AIState.Panicing);
        }
    }

    void TakeDamage(int damage)
    {
        health = health - damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}