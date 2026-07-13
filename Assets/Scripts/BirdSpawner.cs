using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _bird;
    [SerializeField] private int _birdsToSpawn = 250;
    private List<GameObject> _birds;
    private int _aliveBirds;
    private System.Random rng;
    private PipeSpawner _pipeSpawnerScript;

    void Start()
    {
        _birds = new List<GameObject>();
        _aliveBirds = _birdsToSpawn;
        rng = new System.Random();
        _pipeSpawnerScript = Object.FindFirstObjectByType<PipeSpawner>();

        SpawnAllBirds();
    }

    void Update()
    {
        int newAliveCount = 0;
        for (int i = 0; i < _birds.Count; i++)
        {
            if (_birds[i] != null && _birds[i].GetComponentInChildren<FlyBehaviour>()._alive)
            {
                newAliveCount++;
            }
        }
        _aliveBirds = newAliveCount;

        if (_aliveBirds < 1)
        {
            ExecuteAdvancedSelection();
            if (_pipeSpawnerScript != null)
            {
                _pipeSpawnerScript.ResetSpawnerTimeline();
            }
            else
            {
                // Fallback catch if the script lookup failed
                ClearAllPipes();
            }

        }
    }

    private void ExecuteAdvancedSelection()
    {
        // 1. Gather and sort all scripts by performance
        List<FlyBehaviour> sortedBirds = _birds
            .Select(b => b.GetComponentInChildren<FlyBehaviour>())
            .Where(script => script is not null)
            .OrderByDescending(bird => bird._timer)
            .ToList();

        Debug.Log($"Gen Winner lasted: {sortedBirds[0]._timer:F2}s. Top Elite: {sortedBirds[4]._timer:F2}s");

        // 2. ISOLATE THE ELITE POOL (Save the top 15 performing birds to breed from)
        List<Brain> elitePool = new List<Brain>();
        int eliteCount = 15;
        for (int i = 0; i < eliteCount; i++)
        {
            elitePool.Add(sortedBirds[i].brain);
        }

        // 3. Clear out all the old physical GameObjects completely
        foreach (GameObject bird in _birds) Destroy(bird);
        _birds.Clear();

        // 4. Instantiate 250 fresh bird bodies
        SpawnAllBirds();

        // 5. CLONE THE ELITES CRUCIAL BLUEPRINTS BACK EXACTLY (No Mutations for Top 5)
        for (int i = 0; i < 5; i++)
        {
            FlyBehaviour script = _birds[i].GetComponentInChildren<FlyBehaviour>();
            script.SetNewWeightsAndBias(elitePool[i].hiddenWeights, elitePool[i].hiddenBiases, elitePool[i].outputWeights, elitePool[i].outputBias);
        }

        // 6. BREED THE REMAINING FLOCK USING CROSSOVER & MUTATION
        float mutationRate = 0.10f;  // 10% chance per weight link
        float mutationAmount = 0.02f; // Gentle micro-adjustments prevent memory ruin

        for (int i = 5; i < _birds.Count; i++)
        {
            FlyBehaviour childScript = _birds[i].GetComponentInChildren<FlyBehaviour>();

            // Pick two random distinct parents from your top elite stock pool
            Brain parentA = elitePool[rng.Next(0, eliteCount)];
            Brain parentB = elitePool[rng.Next(0, eliteCount)];

            // Prepare memory structural maps for child brain allocation arrays
            float[,] childHWeights = new float[parentA.hiddenWeights.GetLength(0), parentA.hiddenWeights.GetLength(1)];
            float[] childHBiases = new float[parentA.hiddenBiases.Length];
            float[] childOWeights = new float[parentA.outputWeights.Length];
            float childOBias = (rng.NextDouble() < 0.5) ? parentA.outputBias : parentB.outputBias;

            // CROSSOVER MATRIX A: 50% chance to inherit link from Parent A or B
            for (int x = 0; x < childHWeights.GetLength(0); x++)
            {
                for (int y = 0; y < childHWeights.GetLength(1); y++)
                {
                    childHWeights[x, y] = (rng.NextDouble() < 0.5) ? parentA.hiddenWeights[x, y] : parentB.hiddenWeights[x, y];

                    // APPLY MUTATION GENE SLOTS
                    if (rng.NextDouble() < mutationRate)
                    {
                        childHWeights[x, y] += (float)(rng.NextDouble() * 2.0 - 1.0) * mutationAmount;
                        childHWeights[x, y] = Mathf.Clamp(childHWeights[x, y], -1f, 1f);
                    }
                }
            }

            // CROSSOVER MATRIX B & BIASES
            for (int x = 0; x < childOWeights.Length; x++)
            {
                childHBiases[x] = (rng.NextDouble() < 0.5) ? parentA.hiddenBiases[x] : parentB.hiddenBiases[x];
                childOWeights[x] = (rng.NextDouble() < 0.5) ? parentA.outputWeights[x] : parentB.outputWeights[x];

                if (rng.NextDouble() < mutationRate)
                {
                    childHBiases[x] += (float)(rng.NextDouble() * 2.0 - 1.0) * mutationAmount;
                    childHBiases[x] = Mathf.Clamp(childHBiases[x], -1f, 1f);
                }
                if (rng.NextDouble() < mutationRate)
                {
                    childOWeights[x] += (float)(rng.NextDouble() * 2.0 - 1.0) * mutationAmount;
                    childOWeights[x] = Mathf.Clamp(childOWeights[x], -1f, 1f);
                }
            }

            // Final Output Bias Mutation
            if (rng.NextDouble() < mutationRate)
            {
                childOBias += (float)(rng.NextDouble() * 2.0 - 1.0) * mutationAmount;
                childOBias = Mathf.Clamp(childOBias, -1f, 1f);
            }

            // Inject the combined, micro-mutated network brain back into the child
            childScript.SetNewWeightsAndBias(childHWeights, childHBiases, childOWeights, childOBias);
        }

        ClearAllPipes();
    }

    private void SpawnAllBirds()
    {
        Vector3 spawnPos = new Vector3(0f, 2.5f, 0f);
        for (int i = 0; i < _birdsToSpawn; i++)
        {
            GameObject bird = Instantiate(_bird, spawnPos, Quaternion.identity);
            _birds.Add(bird);
        }
        _aliveBirds = _birdsToSpawn;
    }

    private void ClearAllPipes()
    {
        GameObject[] activePipes = GameObject.FindGameObjectsWithTag("Pipe");
        foreach (GameObject pipe in activePipes) Destroy(pipe);
    }
}
