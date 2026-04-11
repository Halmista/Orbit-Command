using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip[] laserSounds;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayLaser()
    {
        if (laserSounds.Length == 0 || audioSource == null) return;

        int index = Random.Range(0, laserSounds.Length);
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(laserSounds[index]);
    }
}