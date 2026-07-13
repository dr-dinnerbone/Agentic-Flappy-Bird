using UnityEngine;

public class FlyBehaviour : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float _velocity = 5f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Manual Evolution Boundaries")]
    [SerializeField] private Transform _floorTransform;
    [SerializeField] private Transform _ceilingTransform;

    private Rigidbody2D _rb;
    private Collider2D _collider;
    private SpriteRenderer _renderer;

    private float displacementToPipeX;
    private float displacementToPipeY;
    private float displacementToCeiling;
    private float displacementToFloor;

    public Brain brain;
    public bool _alive { get; private set; }
    public float _timer { get; private set; }

    void Awake()
    {
        _alive = true;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();

        if (brain == null)
        {
            brain = new Brain(5);
        }
    }

    void Start()
    {
        if (_floorTransform == null) { GameObject f = GameObject.FindWithTag("Floor"); if (f != null) _floorTransform = f.transform; }
        if (_ceilingTransform == null) { GameObject c = GameObject.FindWithTag("Ceiling"); if (c != null) _ceilingTransform = c.transform; }
    }

    void Update()
    {
        if (!_alive) return;

        _timer += Time.deltaTime;

        float maxPipeX = 8f;
        float maxPipeY = 3f;
        float minPipeY = -3f;

        GameObject nextPipe = FindNextPipe();
        if (nextPipe is not null)
        {
            float rawX = nextPipe.transform.position.x - transform.position.x;
            float rawY = nextPipe.transform.position.y - transform.position.y;

            displacementToPipeX = Mathf.Clamp01(rawX / maxPipeX);

            float totalYRange = maxPipeY - minPipeY;
            displacementToPipeY = ((rawY - minPipeY) / totalYRange) * 2f - 1f;
        }
        else
        {
            displacementToPipeX = 1f;
            displacementToPipeY = 0f;
        }

        float floorPos = (_floorTransform != null) ? _floorTransform.position.y : -5f;
        float ceilingPos = (_ceilingTransform != null) ? _ceilingTransform.position.y : 5f;
        float levelHeightRange = ceilingPos - floorPos;

        float rawFloorDist = floorPos - transform.position.y;
        displacementToFloor = Mathf.Clamp(((rawFloorDist - floorPos) / levelHeightRange) * 2f - 1f, -1f, 1f);

        float rawCeilDist = transform.position.y - ceilingPos;
        displacementToCeiling = Mathf.Clamp(((rawCeilDist - floorPos) / levelHeightRange) * 2f - 1f, -1f, 1f);

        float normalizedVelocity = Mathf.Clamp(_rb.linearVelocityY / 8f, -1f, 1f);

        float[] brainInputs = new float[5];
        brainInputs[0] = displacementToFloor;
        brainInputs[1] = displacementToCeiling;
        brainInputs[2] = displacementToPipeX;
        brainInputs[3] = displacementToPipeY;
        brainInputs[4] = normalizedVelocity;

        brain.UpdateInputs(brainInputs);

        if (brain.Think())
        {
            _rb.linearVelocityY = _velocity;
        }
    }

    void FixedUpdate()
    {
        if (!_alive) return;
        transform.rotation = Quaternion.Euler(0, 0, _rb.linearVelocityY * _rotationSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_alive) return;
        HandleDeath();
    }

    private void HandleDeath()
    {
        _alive = false;

        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezePosition;

        if (_collider != null) _collider.enabled = false;
        if (_renderer != null) _renderer.enabled = false;
    }

    private GameObject FindNextPipe()
    {
        GameObject[] allPipes = GameObject.FindGameObjectsWithTag("Pipe");
        GameObject closestUpcomingPipe = null;
        float closestDistanceX = float.MaxValue;

        foreach (GameObject pipe in allPipes)
        {
            float distanceX = pipe.transform.position.x - transform.position.x;

            // FIXED: Change -1.5f to a strict forward-only or minimal margin check (e.g., -0.1f)
            // This stops the bird from looking backward at old pipes floating on the left side of the screen
            if (distanceX > -0.1f)
            {
                if (distanceX < closestDistanceX)
                {
                    closestDistanceX = distanceX;
                    closestUpcomingPipe = pipe;
                }
            }
        }
        return closestUpcomingPipe;
    }


    public void SetNewWeightsAndBias(float[,] hWeights, float[] hBiases, float[] oWeights, float oBias)
    {
        // Injects the multi-neuron variables straight into a fresh brain instance
        brain = new Brain(hWeights, hBiases, oWeights, oBias);
    }


    public void ResetBird(Vector3 startingPosition)
    {
        _alive = true;
        _timer = 0f;

        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;

        if (_rb != null)
        {
            _rb.constraints = RigidbodyConstraints2D.None;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (_collider != null) _collider.enabled = true;
        if (_renderer != null) _renderer.enabled = true;
    }
}
