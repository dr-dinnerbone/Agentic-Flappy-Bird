using UnityEngine;

public class MovePipe : MonoBehaviour
{
    [SerializeField] private float _velocity = 0.65f;
    void Update()
    {
        transform.position += Vector3.left * _velocity * Time.deltaTime;
    }
}
