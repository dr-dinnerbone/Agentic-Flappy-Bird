using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _bird;
    [SerializeField] private int _birdsToSpawn;
    void Start()
    {
        SpawnBird();
    }
    private void SpawnBird()
    {
        Vector3 spawnPos = new Vector3(0f, 5f, 0f);
        Instantiate(_bird, spawnPos, Quaternion.identity);
    }
}
