using UnityEngine;

public class ArmaCazador : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private int daño = 10;

    [Header("Precisión")]
    [SerializeField] private float alturaObjetivo = 0.8f;

    public void Disparar(Transform objetivo)
    {
        if (puntoDisparo == null || prefabBala == null || objetivo == null)
            return;

        Vector3 posicionObjetivo = objetivo.position + Vector3.up * alturaObjetivo;
        Vector3 direccion = posicionObjetivo - puntoDisparo.position;

        GameObject bala = Instantiate(
            prefabBala,
            puntoDisparo.position,
            Quaternion.LookRotation(direccion.normalized)
        );

        BalaNPC balaScript = bala.GetComponent<BalaNPC>();

        if (balaScript != null)
        {
            balaScript.Inicializar(direccion, daño);
        }
    }
}