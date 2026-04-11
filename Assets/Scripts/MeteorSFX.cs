using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSFX : MonoBehaviour
{
    public AudioClip[] deathSounds; // add multiple if you want variation

    public void PlayDeathSound(Vector3 position)
    {
        if (deathSounds.Length == 0) return;

        int index = Random.Range(0, deathSounds.Length);

        // slight pitch variation so it doesn’t sound copy-paste
        float pitch = Random.Range(0.9f, 1.1f);

        GameObject temp = new GameObject("TempAudio");
        temp.transform.position = position;

        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = deathSounds[index];
        source.pitch = pitch;
        source.spatialBlend = 0f; // 2D sound
        source.Play();

        Destroy(temp, source.clip.length);
    }
}
