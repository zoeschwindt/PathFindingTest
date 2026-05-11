using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class AutoJumpLinks : MonoBehaviour
{
    [Header("Puntos de salto")]
    [SerializeField] private Transform contenedorPuntos;

    [Header("Configuración")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private float distanciaMaximaSalto = 12f;
    [SerializeField] private float diferenciaAlturaMaxima = 3f;
    [SerializeField] private float anchoLink = 1f;
    [SerializeField] private bool bidireccional = true;

    [Header("Validación")]
    [SerializeField] private float distanciaBusquedaNavMesh = 2f;

    private void Start()
    {
        CrearLinksAutomaticos();
    }

    [ContextMenu("Crear Links Automaticos")]
    public void CrearLinksAutomaticos()
    {
        if (contenedorPuntos == null)
        {
            Debug.LogWarning("No asignaste el contenedor de puntos de salto.");
            return;
        }

        Transform[] puntos = new Transform[contenedorPuntos.childCount];

        for (int i = 0; i < contenedorPuntos.childCount; i++)
        {
            puntos[i] = contenedorPuntos.GetChild(i);
        }

        for (int i = 0; i < puntos.Length; i++)
        {
            for (int j = i + 1; j < puntos.Length; j++)
            {
                Transform puntoA = puntos[i];
                Transform puntoB = puntos[j];

                if (puntoA == null || puntoB == null)
                    continue;

                float distancia = Vector3.Distance(puntoA.position, puntoB.position);
                float diferenciaAltura = Mathf.Abs(puntoA.position.y - puntoB.position.y);

                if (distancia > distanciaMaximaSalto)
                    continue;

                if (diferenciaAltura > diferenciaAlturaMaxima)
                    continue;

                if (!PuntoEstaEnNavMesh(puntoA.position))
                    continue;

                if (!PuntoEstaEnNavMesh(puntoB.position))
                    continue;

                CrearLink(puntoA.position, puntoB.position, puntoA.name + "_to_" + puntoB.name);
            }
        }

        Debug.Log("Links de salto generados automáticamente.");
    }

    private bool PuntoEstaEnNavMesh(Vector3 punto)
    {
        return NavMesh.SamplePosition(
            punto,
            out NavMeshHit hit,
            distanciaBusquedaNavMesh,
            NavMesh.AllAreas
        );
    }

    private void CrearLink(Vector3 inicio, Vector3 fin, string nombre)
    {
        GameObject linkObject = new GameObject("JumpLink_" + nombre);
        linkObject.transform.SetParent(transform);
        linkObject.transform.position = inicio;

        NavMeshLink link = linkObject.AddComponent<NavMeshLink>();

        if (navMeshSurface != null)
        {
            link.agentTypeID = navMeshSurface.agentTypeID;
        }

        link.startPoint = Vector3.zero;
        link.endPoint = fin - inicio;
        link.width = anchoLink;
        link.bidirectional = bidireccional;
        link.costModifier = 1f;
    }
}