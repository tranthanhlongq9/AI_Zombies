using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.AI;


public class AIExample : MonoBehaviour
{
    public FirstPersonController fpsc;
    
    public float fov = 120f;
    public float viewDistance = 10f;
    public float wanderRadius = 7f;

    private bool isAware = false;
    private Vector3 wanderPoint;
    private NavMeshAgent agent;
    private Renderer renderer;


    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponent<Renderer>();
        wanderPoint = RandomWanderPoint();
    }

    public void Update()
    {
        if (isAware)
        {
            //chase player ( đuổi player )
            agent.SetDestination(fpsc.transform.position);
            renderer.material.color = Color.red;
        }
        else
        {
            //tìm
            SearchForPlayer();
            //đi lang thang
            Wander();
            renderer.material.color = Color.blue;
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
        if (Vector3.Distance(transform.position, wanderPoint) < 1f)
        {
            wanderPoint = RandomWanderPoint();
        }
        else
        {
            agent.SetDestination(wanderPoint);
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
