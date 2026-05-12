using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OIL : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float slowMultiplier = 0.5f;

    private Dictionary<NavMeshAgent, float> velocidadesOriginales = new Dictionary<NavMeshAgent, float>();

    private void OnTriggerEnter(Collider other)
    {
        // IA con NavMeshAgent
        NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();

        if (agent != null && !velocidadesOriginales.ContainsKey(agent))
        {
            velocidadesOriginales.Add(agent, agent.speed);
            agent.speed = agent.speed * slowMultiplier;
        }

        // Jugador
        HomerControl player = other.GetComponentInParent<HomerControl>();

        if (player != null)
        {
            player.SetSpeedMultiplier(slowMultiplier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // IA con NavMeshAgent
        NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();

        if (agent != null && velocidadesOriginales.ContainsKey(agent))
        {
            agent.speed = velocidadesOriginales[agent];
            velocidadesOriginales.Remove(agent);
        }

        // Jugador
        HomerControl player = other.GetComponentInParent<HomerControl>();

        if (player != null)
        {
            player.SetSpeedMultiplier(1f);
        }
    }
}