using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
   private AudioSource audioSource;
   public AudioClip clickSound, sweepSound;

   private void Awake()
   {
      audioSource = GetComponent<AudioSource>();
   }

   public void PlayClick() => audioSource.PlayOneShot(clickSound);
   public void PlaySweep() => audioSource.PlayOneShot(sweepSound);
}
