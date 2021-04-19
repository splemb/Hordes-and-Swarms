using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public Animator animator;
    public AudioSource audioSource;
    public AudioSource walkAudio;
    GameObject[] allWaypoints;

    public Vector3 home;

    public Transform player;

    public float angle = 120f;
    public float radius = 20f;

    public float runSpeed;
    public float walkSpeed;
    public int damage = 10;

    const float timeToStop = 10f;
    float stopDelay;

    public int sightState = 0;
    public int attacking = 0;

    int health = 10;

    bool dead = false;
    bool patrol = true;

    public AudioClip[] aggro;
    public AudioClip[] attack;

    public GameObject explodeParticle;

    [SerializeField] public LayerMask playerMask;
    [SerializeField] public LayerMask groundMask;

    float basePitch;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        basePitch = audioSource.pitch;
    }

    private void OnEnable()
    {
        target = null;
        home = transform.position;
        allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (!dead)
        {

            if (target == player) agent.SetDestination(target.position);

            if (sightState == 2) CheckEnemySphere(5f);
            CheckPlayerSphere();
            if (patrol && agent.remainingDistance <= 1f)
            {
                if (allWaypoints.Length == 0)
                {
                    target = null;
                }
                else
                {
                    int index = Random.Range(0, allWaypoints.Length);
                    target = allWaypoints[index].transform;
                    agent.SetDestination(target.position);
                }
            }


            else if (attacking <= 0) { Sight(); animator.SetBool("Attack", false); }
            else
            {
                attacking--;
                agent.speed = 0.1f;
                animator.SetBool("Attack", true);

                if (attacking == 45) { audioSource.pitch = basePitch + Random.Range(-0.05f, 0.05f); audioSource.PlayOneShot(attack[Random.Range(0, attack.Length)]); }

                if (attacking == 30)
                {
                    RaycastHit hit;
                    if (Physics.Raycast((transform.position + Vector3.up * 0.8f), transform.forward, out hit, 1.5f * transform.localScale.z, playerMask))
                    {
                        hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -damage);
                    }
                }
            }


            animator.SetFloat("VelocityX", Vector3.Dot(agent.transform.right, agent.velocity) / runSpeed);
            animator.SetFloat("VelocityZ", Vector3.Dot(agent.transform.forward, agent.velocity) / runSpeed);
            animator.SetBool("Grounded", CheckGrounded(0.5f));

            if (agent.currentOffMeshLinkData.linkType.ToString() == "LinkTypeJumpAcross")
            {
                animator.SetTrigger("Jump");
            }

            if (agent.velocity.magnitude > 0 && agent.speed > walkSpeed && CheckGrounded(0.5f) && !walkAudio.isPlaying)
            {
                walkAudio.loop = true;
                walkAudio.Play();
                
            }
            else if (agent.velocity.magnitude == 0 || !CheckGrounded(0.5f))
            {
                walkAudio.Stop();
                walkAudio.loop = false;
            }
        }
        
    }

    void Sight()
    {
        Vector3 playerCenterPosition = player.GetComponent<Collider>().bounds.center + (0.5f * player.GetComponent<Collider>().bounds.extents.y * Vector3.up);
        Vector3 directionVector = -(playerCenterPosition - (transform.position + Vector3.up * 0.8f)).normalized;
        float distance = Vector3.Distance(transform.position + Vector3.up * 1.8f, playerCenterPosition);
        //Debug.Log("Distance: " + distance);

        float dotProduct = Vector3.Dot(directionVector, transform.forward);
        Debug.DrawRay((transform.position + Vector3.up * 0.8f), -directionVector * radius, Color.magenta);
        RaycastHit hit;
        if (Physics.Raycast((transform.position + Vector3.up * 0.8f), -directionVector, out hit, radius, playerMask))
        {
            if (dotProduct < -0.5f && distance <= radius && (hit.transform.tag == "Player"))
            {
                if (sightState != 2) { audioSource.pitch = basePitch + Random.Range(-0.05f, 0.05f); audioSource.PlayOneShot(aggro[Random.Range(0, aggro.Length)]); }
                target = player;
                patrol = false;
                sightState = 2;
                agent.speed = runSpeed;
                //Debug.Log(dotProduct + ": IN FIELD OF VIEW. Distance: " + distance);
            }
            else if (sightState == 2)
            {
                sightState = 1;
                stopDelay = Time.time + timeToStop;

                //Debug.Log(dotProduct + ": OUT OF SIGHT. Distance: " + distance);
            }
        }
        if (Time.time > stopDelay && sightState == 1)
        {
            patrol = true;
            agent.speed = walkSpeed;
            sightState = 0;
        }
    }

    public void ApplyDamage(int amount = 0)
    {
        health -= amount;
        if (health <= 0 && dead == false) Death();
    }

    void Death()
    {
        walkAudio.Stop();
        dead = true;
        Destroy(this.gameObject, 2.8f);
        animator.SetTrigger("Death");
        CheckEnemySphere(10f);
        audioSource.Play();
        agent.speed = 0f;
        Instantiate(explodeParticle, transform.position, Quaternion.identity);
    }

    void CheckEnemySphere(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up, radius, playerMask);

        foreach (Collider c in colliders)
        {
            if (c.tag == "Enemy" && c.gameObject != this.gameObject && (sightState == 2 || dead))
            {
                c.transform.LookAt(player);
                Debug.Log("ENEMEMEY");
            }
        }
    }

    void CheckPlayerSphere()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up, 1.5f * transform.localScale.z, playerMask);
        bool playerInRange = false;

        foreach (Collider c in colliders)
        {
            if (c.tag == "Player")
            {
                playerInRange = true;
                transform.LookAt(player.position);
            }
        }

        if (!playerInRange) attacking = 0;
        else if (attacking == 0) attacking = 50;
    }

    private bool CheckGrounded(float reach = 0.001f)
    {
        //return if the player is on the ground
        //reach variable (default 0.05) is how far the 
        if (Physics.BoxCast(transform.position + (transform.up * 0.5f * transform.localScale.y), new Vector3(0.25f, 0.1f, 0.25f), -transform.up, Quaternion.identity, (0.5f + reach) * transform.localScale.y, groundMask)) return true;
        else return false;
    }

}
