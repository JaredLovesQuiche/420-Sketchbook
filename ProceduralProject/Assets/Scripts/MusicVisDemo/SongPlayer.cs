using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(AudioSource))]
public class SongPlayer : MonoBehaviour
{
    public AudioClip[] playlist;
    private AudioSource player;

    private int currentTrack = -1;

    private void Start()
    {
        player = GetComponent<AudioSource>();
        PlayTrackRandom();
    }

    private void Update()
    {
        if (!player.isPlaying)
        {
            PlayTrackNext();
        }
    }

    public void PlayTrack(int n)
    {
        if (n < 0 || n >= playlist.Length) return;
        currentTrack = n;
        player.PlayOneShot(playlist[n]);
    }

    public void PlayTrackRandom()
    {
        PlayTrack(Random.Range(0, playlist.Length));
    }

    public void PlayTrackNext()
    {
        int nextTrack = currentTrack + 1;
        if (nextTrack >= playlist.Length) nextTrack = 0;
        PlayTrack(nextTrack);
    }
}

[CustomEditor(typeof(SongPlayer))]
public class MusicPlayerEditor : Editor
{

}
