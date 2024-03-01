using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    // UI
    [SerializeField]
    AudioClip buttonSound;

    // game noties
    [SerializeField]
    AudioClip loseSound;
    [SerializeField]
    AudioClip deadSound;

    // snake
    [SerializeField]
    AudioClip snakeRotationSound;

    // eating
    [SerializeField]
    AudioClip eatAppleSound;
    [SerializeField]
    AudioClip eatEmptyAppleSound;
    [SerializeField]
    AudioClip eatBrickAppleSound;
    [SerializeField]
    AudioClip eatMarmeladeAppleSound;
    [SerializeField]
    AudioClip eatSegmentSound;
    [SerializeField]
    AudioClip eatWallSound;

    public void PlayAudio(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
        Debug.Log("Play Sound "+clip.name);
    }

    public void PlayButtonClickSound()
    {
        PlayAudio(buttonSound);
    }
    public void PlayLoseSound()
    {
        PlayAudio(loseSound);
    }
    public void PlayDeadSound()
    {
        PlayAudio(deadSound);
    }
    public void PlaySnakeRotationSound()
    {
        PlayAudio(snakeRotationSound);
    }
}
