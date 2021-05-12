using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.AI;


public class AIExample : MonoBehaviour
{
    public enum WanderType { Random, Waypoint };

    public FirstPersonController fpsc;
    public WanderType wanderType = WanderType.Random;
    public float wanderSpeed = 3f;
    public float chaseSpeed = 7f;
    public float fov = 120f;
    public float viewDistance = 10f;
    public float wanderRadius = 7f;
    public Transform[] waypoints; //Mảng điểm tham chiếu chỉ được sử dụng khi lang thang điểm tham chiếu được chọn

    private bool isAware = false;
    private Vector3 wanderPoint;
    private NavMeshAgent agent;
    private Renderer renderer;
    private int waypointIndex = 0;
    private Animator animator;


    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponent<Renderer>();
        animator = GetComponentInChildren<Animator>();
        wanderPoint = RandomWanderPoint();
    }

    public void Update()
    {
        if (isAware)
        {
            //chase player ( đuổi player )
            agent.SetDestination(fpsc.transform.position);
            animator.SetBool("Aware", true);
            agent.speed = chaseSpeed;

            //renderer.material.color = Color.red;
        }
        else
        {
            //tìm
            SearchForPlayer();
            //đi lang thang
            Wander();
            animator.SetBool("Aware", false);
            agent.speed = wanderSpeed;

            //renderer.material.color = Color.blue;
        }
    }

    //Tìm player
    public void SearchForPlayer()
    {
        if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f)
        {
            if(Vector3.Distance(fpsc.transform.position, transform.position) < viewDistance)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, fpsc.transform.position, out hit, -1))
                {
                    //xử lý va chạm vs tag : "Player"
                    if (hit.transform.CompareTag("Player"))
                    {
                        OnAware(); 
                    }
                }
                
            }
        }
    }

    public void OnAware()
    {
        isAware = true;
    }

    //Xử lý đi lang thang cho zombies
    public void Wander()
    {
        if(wanderType == WanderType.Random) 
        { 
            if (Vector3.Distance(transform.position, wanderPoint) < 0.7f)
            {
                wanderPoint = RandomWanderPoint();
            }
            else
            {
                agent.SetDestination(wanderPoint);
            }
        }
        else
        {
            //Waypoint wandering (đi lang thang đến các điểm)
            if(waypoints.Length >= 2)
            { 
                if (Vector3.Distance(waypoints[waypointIndex].position, transform.position) < 2f)
                {
                    if (waypointIndex == waypoints.Length - 1)
                    {
                        waypointIndex = 0;
                    }
                    else
                    {
                        waypointIndex++;
                    }
                }
                else
                {
                    agent.SetDestination(waypoints[waypointIndex].position);
                }
            }
            else
            {
                Debug.LogWarning("Please assign more than 1 waypoint to the AI: " + gameObject.name);
            }
        }
    }

    //random điểm đến cho zombies
    public Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, wanderRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }

}
