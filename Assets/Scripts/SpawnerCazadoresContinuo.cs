using UnityEngine;

public class SpawnerCazadoresContinuo : MonoBehaviour
{
    [Header("Prefabs de cazadores")]
    [SerializeField] private GameObject[] prefabsCazadores;

    [Header("Puntos de spawn")]
    [SerializeField] private Transform[] puntosSpawn;

    [Header("Configuración")]
    [SerializeField] private float tiempoEntreSpawns = 10f;
    [SerializeField] private bool spawnearAlIniciar = true;

    [Header("Límite opcional")]
    [SerializeField] private bool usarLimite = true;
    [SerializeField] private int maximoCazadores = 15;

    private float temporizador;

    private void Start()
    {
        temporizador = tiempoEntreSpawns;

        if (spawnearAlIniciar)
        {
            SpawnearGrupo();
        }
    }

    private void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            if (!usarLimite || ContarCazadores() < maximoCazadores)
            {
                SpawnearGrupo();
            }

            temporizador = tiempoEntreSpawns;
        }
    }

    private void SpawnearGrupo()
    {
        for (int i = 0; i < prefabsCazadores.Length; i++)
        {
            if (prefabsCazadores[i] == null)
                continue;

            if (puntosSpawn.Length == 0)
                return;

            Transform punto = puntosSpawn[i % puntosSpawn.Length];

            GameObject nuevoCazador = Instantiate(
                prefabsCazadores[i],
                punto.position,
                punto.rotation
            );

            nuevoCazador.tag = "Cazador";
        }
    }

    private int ContarCazadores()
    {
        return GameObject.FindGameObjectsWithTag("Cazador").Length;
    }
}