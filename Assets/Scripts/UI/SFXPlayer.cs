using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
   private AudioSource audioSource;
   public AudioClip clickSound, sweepSound, scrollSound, doorSound, lockDoorSound;

   private void Awake()
   {
      audioSource = GetComponent<AudioSource>();
   }

   public void PlayClick() => audioSource.PlayOneShot(clickSound);
   public void PlaySweep() => audioSource.PlayOneShot(sweepSound);
   public void PlayScroll() => audioSource.PlayOneShot(sweepSound);
   public void PlayDoor() => audioSource.PlayOneShot(sweepSound);
   public void PlayLockDoor() => audioSource.PlayOneShot(sweepSound);
}
