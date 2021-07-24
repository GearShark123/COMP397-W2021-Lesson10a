using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BobState
{
    IDLE,
    RUN,
    JUMP,
    KICK
}

public class BobBehaviour : MonoBehaviour
{
    [Header("Line of Sight")]
    public bool HasLOS;
    public GameObject player;

    [Header("Attack")]
    public PlayerBehaviour playerBehaviour;
    public float damageDelay = 1.0f;
    public bool isAttacking = false;
    public float kickForce = 4.0f;
    public float distanceToPlayer;

    private NavMeshAgent agent;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerBehaviour = FindObjectOfType<PlayerBehaviour>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (HasLOS)
        {
            agent.SetDestination(player.transform.position);
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        }

        if (HasLOS && distanceToPlayer < 4 && !isAttacking)
        {
            animator.SetInteger("AnimState", (int)BobState.KICK);
            DoKickDamage();
            isAttacking = true;

            //transform.LookAt(transform.position - player.transform.forward);    
        }
        else if (HasLOS && agent.isOnOffMeshLink == false && distanceToPlayer > 4)
        {
            animator.SetInteger("AnimState", (int)BobState.RUN);
            isAttacking = false;
        }
        else if (HasLOS && agent.isOnOffMeshLink)
        {
            animator.SetInteger("AnimState", (int)BobState.JUMP);
        }
        else
        {
            animator.SetInteger("AnimState", (int)BobState.IDLE);
        }
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    animator.SetInteger("AnimState", (int)BobState.IDLE);
        //}

        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    animator.SetInteger("AnimState", (int)BobState.RUN);
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    animator.SetInteger("AnimState", (int)BobState.JUMP);
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HasLOS = true;
            player = other.transform.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HasLOS = false;
        }
    }

    private void DoKickDamage()
    {
        playerBehaviour.TakeDamage(20);        
        StartCoroutine(KickBack());
    }

    private IEnumerator KickBack()
    {
        yield return new WaitForSeconds(1.0f);        
        var direction = Vector3.Normalize(player.transform.position - transform.position);
        playerBehaviour.controller.SimpleMove(direction * kickForce);
        StopCoroutine(KickBack());
    }
}
