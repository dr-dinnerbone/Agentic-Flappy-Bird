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
        Vector3 spawnPos = transform.position + new Vector3(8f, Random.Range(-_heightRange, _heightRange));
        GameObject pipe = Instantiate(_pipe, spawnPos, Quaternion.identity);
        Destroy(pipe, 5f);
    }

    public void ResetSpawnerTimeline()
    {
        GameObject[] activePipes = GameObject.FindGameObjectsWithTag("Pipe");
        foreach (GameObject pipe in activePipes)
        {
            Destroy(pipe);
        }

        _timer = 0f;

        SpawnPipe();
    }
}
