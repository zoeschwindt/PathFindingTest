using UnityEngine;
using UnityEngine.AI;

public class OIL : MonoBehaviour
{
    public float slowMultiplier = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        // IA
        NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed *= slowMultiplier;
        }

        // JUGADOR
        HomerControl player = other.GetComponentInParent<HomerControl>();
        if (player != null)
        {
            player.SetSpeedMultiplier(slowMultiplier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed /= slowMultiplier;
        }

        HomerControl player = other.GetComponentInParent<HomerControl>();
        if (player != null)
        {
            player.SetSpeedMultiplier(1f);
        }
    }
}