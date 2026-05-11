using UnityEngine;

public class BalaNPC : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidad = 25f;
    [SerializeField] private int daño = 10;
    [SerializeField] private float tiempoVida = 4f;

    private Vector3 direccion;

    public void Inicializar(Vector3 nuevaDireccion, int nuevoDaño)
    {
        direccion = nuevaDireccion.normalized;
        daño = nuevoDaño;

        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        transform.position += direccion * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        JaguarVida vidaJaguar = other.GetComponentInParent<JaguarVida>();

        if (vidaJaguar != null)
        {
            vidaJaguar.RecibirDaño(daño);
            Destroy(gameObject);
        }
    }
}