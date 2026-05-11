using UnityEngine;

public class AguaCocodrilo : MonoBehaviour
{
    [Header("Cocodrilos disponibles")]
    [SerializeField] private CocodriloAtaque[] cocodrilos;

    [Header("Tags")]
    [SerializeField] private string tagCazador = "Cazador";
    [SerializeField] private string tagJaguar = "Jaguar";

    private GameObject ultimoObjetivoAvisado;

    private void Start()
    {
        if (cocodrilos == null || cocodrilos.Length == 0)
        {
            cocodrilos = FindObjectsByType<CocodriloAtaque>(FindObjectsSortMode.None);
        }

        Debug.Log("Cocodrilos encontrados: " + cocodrilos.Length);
    }

    private void OnTriggerEnter(Collider other)
    {
        DetectarObjetivo(other);
    }

    private void OnTriggerStay(Collider other)
    {
        DetectarObjetivo(other);
    }

    private void DetectarObjetivo(Collider other)
    {
        GameObject objetivo = BuscarObjetoConTag(other.transform);

        if (objetivo == null)
            return;

        if (objetivo == ultimoObjetivoAvisado)
            return;

        ultimoObjetivoAvisado = objetivo;

        Debug.Log("Alguien cayó al agua: " + objetivo.name);

        AvisarATodosLosCocodrilos(objetivo);
    }

    private GameObject BuscarObjetoConTag(Transform t)
    {
        Transform actual = t;

        while (actual != null)
        {
            if (actual.CompareTag(tagCazador) || actual.CompareTag(tagJaguar))
            {
                return actual.gameObject;
            }

            actual = actual.parent;
        }

        return null;
    }

    private void AvisarATodosLosCocodrilos(GameObject objetivo)
    {
        foreach (CocodriloAtaque cocodrilo in cocodrilos)
        {
            if (cocodrilo == null) continue;

            cocodrilo.AtacarObjetivo(objetivo);
        }
    }
}