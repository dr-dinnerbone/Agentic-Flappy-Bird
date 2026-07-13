using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private float _maxTime = 1.5f;
    [SerializeField] private float _heightRange = 3f;
    [SerializeField] private GameObject _pipe;
    private float _timer;

    void Start()
    {
        SpawnPipe();
    }

    void Update()
    {
        if (_timer > _maxTime)
        {
            SpawnPipe();
            _timer = 0;
        }
        _timer += Time.deltaTime;
    }

    private void SpawnPipe()
    {
        // Spawns 8 units forward to the right
        Vector3 spawnPos = transform.position + new Vector3(8f, Random.Range(-_heightRange, _heightRange));
        GameObject pipe = Instantiate(_pipe, spawnPos, Quaternion.identity);
        Destroy(pipe, 5f); // Increased slightly so they have time to clear the screen
    }

    // ================== ADD THIS RESET FUNCTION ==================
    public void ResetSpawnerTimeline()
    {
        // 1. Wipe out any lingering pipes instantly
        GameObject[] activePipes = GameObject.FindGameObjectsWithTag("Pipe");
        foreach (GameObject pipe in activePipes)
        {
            Destroy(pipe);
        }

        // 2. Reset the cooldown clock back to 0 completely
        _timer = 0f;

        // 3. Force spawn the very first runway pipe at a perfectly predictable distance
        SpawnPipe();

        Debug.Log("Pipe runway timeline has been perfectly synchronized for the new flock!");
    }
}
