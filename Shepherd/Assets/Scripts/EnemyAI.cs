using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float distanz = 0.4f;
    public Animator animator;
    public enum PState { Running, Attacking}
    public PState currentStates = PState.Running;
    SpriteRenderer m_spriteRenderer;

    private GameObject closestSheep { get; set; }

    private float? distanceToClosestSheep { get; set; } = null;

    private GameObject[] sheepCache { get; set; }

    public float RunningSpeed { get; set; } = 3f;

    private NavMeshAgent agent { get; set; }

    float damageTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.autoBraking = true;
    }

    // Update is called once per frame
    void Update()
    {

        updateDestination();
        if (distanceToClosestSheep < distanz)
        {
            agent.speed = 0f;
            currentStates = PState.Attacking;
            SwitchAnimationStates(currentStates);
        }
    }

    public void flipX(Vector3 movementDirection)
    {
        // get camera direction
        Vector3 cameraDirection = Camera.main.transform.forward;

        // compute orthogonal vector in xz plane
        Vector3 orthogonal = new Vector3(cameraDirection.z, 0, -cameraDirection.x);

        //multiply
        float product = Vector3.Dot(orthogonal, movementDirection);
        if (product < -0.1f)
            m_spriteRenderer.flipX = true;
        else if (product > 0.1f)
            m_spriteRenderer.flipX = false;
    }

    private void updateDestination()
    {
        closestSheep = null;
        distanceToClosestSheep = null;
        sheepCache = GameObject.FindGameObjectsWithTag("Sheep");
        if (sheepCache == null || sheepCache.Length == 0)
        {
            //no target, no destination and set speed to 0
            closestSheep = null;
            agent.speed = 0f;
            return;
        }
        float currentDistance = 0.0f;

        for (int sheepBatch = 0; sheepBatch < sheepCache.Length; sheepBatch++)
        {
            //is sheep closer than current closest sheep?
            currentDistance = Vector3.Distance(sheepCache[sheepBatch].transform.position, this.transform.position);

            if (distanceToClosestSheep == null || distanceToClosestSheep > currentDistance)
            {
                closestSheep = sheepCache[sheepBatch];
                distanceToClosestSheep = currentDistance;
            }
        }

        //find closest sheep on the nav mesh and walk towards it
        if (closestSheep != null)
        {

            NavMeshHit hit;
            NavMesh.SamplePosition(closestSheep.transform.position, out hit, 1.0f, NavMesh.AllAreas);
            agent.destination = hit.position;
            agent.speed = this.RunningSpeed;
            currentStates = PState.Running;
            SwitchAnimationStates(currentStates);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sheep")
        {
            agent.speed = 0f;
            currentStates = PState.Attacking;
            SwitchAnimationStates(currentStates);
            other.gameObject.SendMessage("TakeDamage", 1.0);
        }
    }

    void OnTriggerStay(Collider other)
    {
        damageTimer -= Time.deltaTime;
        if(damageTimer <= 0)
        {

            if (other.gameObject.tag == "Sheep")
            {
                agent.speed = 0f;
                other.gameObject.SendMessage("TakeDamage", 1.0);
                currentStates = PState.Attacking;
                SwitchAnimationStates(currentStates);
        }
            damageTimer = 1f;
        }
    }

    void SwitchAnimationStates(PState state)
    {
        //Animation control
        if (animator)
        {
            animator.SetBool("isAttacking", state == PState.Attacking);
            animator.SetBool("isRunning", state == PState.Running);
        }
    }

}
