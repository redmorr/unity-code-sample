using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private AudioSource[] sources;
    [SerializeField] private AudioSource source;

    private readonly int count = 20;
    
    private int index;

    [Button]
    public void Setup()
    {
        sources = new AudioSource[count];
        for (int i = 0; i < count; i++)
        {
            GameObject gameObject = new GameObject(string.Format("Audio Source {0}", i));
            gameObject.transform.SetParent(transform);
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].minDistance = 3f;
            sources[i].maxDistance = 20f;
            sources[i].outputAudioMixerGroup = mixerGroup;
        }
    }
    
    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        source = sources[index];
        source.spatialBlend = 0f;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.clip = clip;
        if (source.volume != volume)
        {
            source.volume = volume;
        }
        source.Play();
        index = (index + 1) % sources.Length;
    }
    
    public void PlayClipAtPosition(AudioClip clip, float volume, Vector3 position)
    {
        source = sources[index];
        source.transform.position = position;
        source.spatialBlend = 1f;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.clip = clip;
        if (source.volume != volume)
        {
            source.volume = volume;
        }
        source.Play();
        index = (index + 1) % sources.Length;
    }
}