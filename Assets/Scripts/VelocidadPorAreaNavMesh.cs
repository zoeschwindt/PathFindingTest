using UnityEngine;
using UnityEngine.AI;

public class VelocidadPorAreaNavMesh : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Velocidades")]
    [SerializeField] private float velocidadNormal = 3.5f;
    [SerializeField] private float velocidadAgua = 1.2f;

    [Header("Nombre del área de agua")]
    [SerializeField] private string nombreAreaAgua = "Agua";

    private int areaAgua;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        areaAgua = NavMesh.GetAreaFromName(nombreAreaAgua);
    }

    private void Update()
    {
        if (agent == null) return;

        if (EstaEnAgua())
        {
            agent.speed = velocidadAgua;
        }
        else
        {
            agent.speed = velocidadNormal;
        }
    }

    private bool EstaEnAgua()
    {
        if (areaAgua < 0) return false;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            int mascaraAgua = 1 << areaAgua;

            return (hit.mask & mascaraAgua) != 0;
        }

        return false;
    }
}