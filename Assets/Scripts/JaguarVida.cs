using UnityEngine;
using UnityEngine.AI;

public class JaguarVida : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int vidaActual;

    [Header("Respawn")]
    [SerializeField] private Transform puntoRespawn;

    private NavMeshAgent agent;
    private CharacterController characterController;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        agent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
    }

    public void RecibirDaño(int daño)
    {
        vidaActual -= daño;

        Debug.Log("Jaguar recibió daño. Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("El jaguar murió.");

        vidaActual = vidaMaxima;

        if (puntoRespawn == null)
            return;

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.Warp(puntoRespawn.position);
        }
        else if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = puntoRespawn.position;
            characterController.enabled = true;
        }
        else
        {
            transform.position = puntoRespawn.position;
        }
    }
}