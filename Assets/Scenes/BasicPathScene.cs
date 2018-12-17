using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicPathScene : MonoBehaviour
{
    public  Transform player;
    private NavMeshAgent navAgent;
    public LayerMask layerMask;

    public float sampleRadius = 10;
    public int sampleDensity = 10;

    float timeTillMove = 1f;
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, sampleRadius);
    }

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();        
    }

    void Update()
    {
        //Check line of sight every 4 frames
        if (timeTillMove <= 0 && IsInLineOfSight(transform.position, player.position) && Time.frameCount % 4 == 0)
        {
            navAgent.SetDestination(FindHidingSpot(sampleRadius, sampleDensity));

            timeTillMove = 0.1f;
        }

        timeTillMove -= Time.deltaTime;

    }

    bool IsInLineOfSight(Vector3 origin, Vector3 target)
    {
        Ray ray = new Ray(origin, target - origin);

    // Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(origin, target), Color.red, 10f);

        bool output = !Physics.Raycast(ray, Vector3.Distance(origin, target), layerMask);

        return output; 
    }


    Vector3 FindHidingSpot(float size, int density)
    {
        List<Vector3> viablePositions = new List<Vector3>(density);

        Vector3 bestPosition = transform.position;
        bool bestPosInLineOfSight = IsInLineOfSight(bestPosition, player.position);

        for (int x = 0; x < density; x++)
        {
            for (int y = 0; y < density; y++)
            {
                Vector3 pos = new Vector3(size / density * x - size / 2, 0, size / density * y - size / 2) + transform.position;
                NavMesh.SamplePosition(pos, out NavMeshHit hit, size / density, NavMesh.AllAreas);
                pos = hit.position;
                bool inLineOfSight = IsInLineOfSight(pos, player.position);

                if (!bestPosInLineOfSight)
                {
                    if (!inLineOfSight)
                    {
                        if (Vector3.Distance(transform.position, pos) < Vector3.Distance(transform.position, bestPosition) && Vector3.Distance(player.position, pos) > Vector3.Distance(transform.position, pos))
                        {
                            viablePositions.Add(pos);
                            bestPosition = pos;
                            bestPosInLineOfSight = false;
                        }
                    }
                }
                else
                {
                    Debug.DrawRay(pos, Vector3.up, Color.blue, 5f);

                    if (inLineOfSight)
                    {
                        if (Vector3.Distance(pos, player.position) > Vector3.Distance(bestPosition, player.position))
                        {
                            //bestPosition = pos;
                            //bestPosInLineOfSight = true;
                        }
                    }
                    else
                    {
                        if (bestPosInLineOfSight || Vector3.Distance(transform.position, pos) < Vector3.Distance(transform.position, bestPosition))
                        {
                            Debug.DrawRay(bestPosition, Vector3.up, Color.blue, 5f);

                            bestPosition = pos;
                            bestPosInLineOfSight = false;
                        }
                    }
                }
            }
        }

        return bestPosition;
        //return viablePositions[Random.Range(viablePositions.Count - 2, viablePositions.Count)];
    }
}
