using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CazadorAI : MonoBehaviour
{
    public enum TipoAtaque
    {
        Disparo,
        Lanza,
        Patadas
    }

    [Header("Objetivo")]
    [SerializeField] private Transform jaguar;

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Tipo de cazador")]
    [SerializeField] private TipoAtaque tipoAtaque;

    [Header("Detección")]
    [SerializeField] private float distanciaDeteccion = 40f;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float velocidadGiroAtaque = 8f;

    [Header("Daño")]
    [SerializeField] private int daño = 10;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private float tiempoAplicarDaño = 0.5f;

    [Header("Animación")]
    [SerializeField] private string parametroVelocidad = "Speed";
    [SerializeField] private string triggerDisparo = "AttackShoot";
    [SerializeField] private string triggerLanza = "AttackSpear";
    [SerializeField] private string triggerPatada1 = "AttackKick1";
    [SerializeField] private string triggerPatada2 = "AttackKick2";

    [Header("Arma de disparo")]
    [SerializeField] private ArmaCazador armaCazador;

    private bool atacando;
    private float tiempoProximoAtaque;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        BuscarJaguar();

        ConfigurarSegunTipo();
    }

    private void Update()
    {
        if (jaguar == null)
        {
            BuscarJaguar();
            return;
        }

        float distancia = Vector3.Distance(transform.position, jaguar.position);

        if (distancia <= distanciaDeteccion)
        {
            if (distancia <= distanciaAtaque)
            {
                DetenerYAtacar();
            }
            else
            {
                PerseguirJaguar();
            }
        }
        else
        {
            Detener();
        }

        ActualizarAnimacionMovimiento();
    }

    private void BuscarJaguar()
    {
        GameObject jaguarEncontrado = GameObject.FindGameObjectWithTag("Jaguar");

        if (jaguarEncontrado != null)
        {
            jaguar = jaguarEncontrado.transform;
        }
    }

    private void ConfigurarSegunTipo()
    {
        switch (tipoAtaque)
        {
            case TipoAtaque.Disparo:
                distanciaAtaque = 15f;
                daño = 10;
                tiempoEntreAtaques = 2f;
                tiempoAplicarDaño = 0.4f;
                break;

            case TipoAtaque.Lanza:
                distanciaAtaque = 2.8f;
                daño = 18;
                tiempoEntreAtaques = 1.8f;
                tiempoAplicarDaño = 0.45f;
                break;

            case TipoAtaque.Patadas:
                distanciaAtaque = 2.2f;
                daño = 14;
                tiempoEntreAtaques = 1.4f;
                tiempoAplicarDaño = 0.35f;
                break;
        }
    }

    private void PerseguirJaguar()
    {
        if (atacando) return;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(jaguar.position);
        }
    }

    private void DetenerYAtacar()
    {
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        MirarAlJaguar();

        if (!atacando && Time.time >= tiempoProximoAtaque)
        {
            StartCoroutine(Atacar());
        }
    }

    private void Detener()
    {
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
        }
    }

    private void MirarAlJaguar()
    {
        Vector3 direccion = jaguar.position - transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.01f)
            return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivo,
            velocidadGiroAtaque * Time.deltaTime
        );
    }

    private IEnumerator Atacar()
    {
        atacando = true;
        tiempoProximoAtaque = Time.time + tiempoEntreAtaques;

        LanzarAnimacionAtaque();

        yield return new WaitForSeconds(tiempoAplicarDaño);

        AplicarDañoSiCorresponde();

        yield return new WaitForSeconds(0.3f);

        atacando = false;
    }

    private void LanzarAnimacionAtaque()
    {
        if (animator == null) return;

        switch (tipoAtaque)
        {
            case TipoAtaque.Disparo:
                animator.SetTrigger(triggerDisparo);
                break;

            case TipoAtaque.Lanza:
                animator.SetTrigger(triggerLanza);
                break;

            case TipoAtaque.Patadas:
                if (Random.value < 0.5f)
                    animator.SetTrigger(triggerPatada1);
                else
                    animator.SetTrigger(triggerPatada2);
                break;
        }
    }

    private void AplicarDañoSiCorresponde()
    {
        if (jaguar == null) return;

        float distancia = Vector3.Distance(transform.position, jaguar.position);

        if (distancia > distanciaAtaque + 0.5f)
            return;

        if (tipoAtaque == TipoAtaque.Disparo)
        {
            if (armaCazador != null)
            {
                armaCazador.Disparar(jaguar);
            }

            return;
        }

        JaguarVida vidaJaguar = jaguar.GetComponent<JaguarVida>();

        if (vidaJaguar != null)
        {
            vidaJaguar.RecibirDaño(daño);
        }
    }

    private void ActualizarAnimacionMovimiento()
    {
        if (animator == null || string.IsNullOrEmpty(parametroVelocidad))
            return;

        float velocidad = 0f;

        if (agent != null && agent.enabled)
        {
            velocidad = agent.velocity.magnitude;
        }

        animator.SetFloat(parametroVelocidad, velocidad);
    }
}