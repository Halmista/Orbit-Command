using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip[] soundEffects; // drag as many sounds as you want
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // loop through A to Z
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
        {
            if (Input.GetKeyDown(key))
            {
                PlayRandomSound();
            }
        }
    }

    void PlayRandomSound()
    {
        if (soundEffects.Length == 0) return;

        int index = Random.Range(0, soundEffects.Length);

        // optional pitch variation (makes it less repetitive)
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        audioSource.PlayOneShot(soundEffects[index]);
    }
}