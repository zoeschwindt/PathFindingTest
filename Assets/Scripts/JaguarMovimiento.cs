using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class JaguarMovimiento : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Camara")]
    [SerializeField] private Camera camara;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (camara == null)
            camara = Camera.main;
    }

    private void Update()
    {
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {

            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray ray = camara.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    agent.isStopped = false;
                    agent.SetDestination(navHit.position);

                    Debug.Log("Jaguar moviéndose a: " + navHit.position);
                }
                else
                {
                    Debug.Log("El punto clickeado no está cerca del NavMesh");
                }
            }
            else
            {
                Debug.Log("El click no tocó ningún collider");
            }
        }

        if (animator != null && agent != null)
        {
            float velocidad = agent.velocity.magnitude;
            animator.SetFloat("Speed", velocidad);
        }
    }
}