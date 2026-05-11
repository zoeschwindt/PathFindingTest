using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    public Transform player;

    public float roadCost = 10f;
    public float jumpCost = 10f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        int roadArea = NavMesh.GetAreaFromName("Road");
        int jumpArea = NavMesh.GetAreaFromName("Jump");

        agent.SetAreaCost(roadArea, roadCost);
        agent.SetAreaCost(jumpArea, jumpCost);
    }

    void Update()
    {
        agent.SetDestination(player.position);
    }
}