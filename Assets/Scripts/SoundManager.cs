using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip pointSound;
    [SerializeField] private AudioClip hitSound;
    public void PlayJump()
    {
        audioSource.PlayOneShot(jumpSound);
    }
    public void PlayPoint()
    {
        audioSource.PlayOneShot(pointSound);
    }
    public void PlayHit()
    {
        audioSource.PlayOneShot(hitSound);
    }
}