using UnityEngine;
using UnityEngine.AI;

public class CazadorAI : MonoBehaviour
{
    [Header("Objetivo a perseguir")]
    [SerializeField] private Transform jaguar;

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Configuración")]
    [SerializeField] private float distanciaDeteccion = 20f;
    [SerializeField] private float distanciaAtaque = 2f;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (jaguar == null) return;

        float distancia = Vector3.Distance(transform.position, jaguar.position);

        if (distancia <= distanciaDeteccion && distancia > distanciaAtaque)
        {
            agent.isStopped = false;
            agent.SetDestination(jaguar.position);
        }
        else
        {
            agent.isStopped = true;
        }

        float velocidad = agent.velocity.magnitude;
        animator.SetFloat("Speed", velocidad);
    }
}