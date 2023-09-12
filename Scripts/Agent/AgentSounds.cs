using UnityEngine;

[CreateAssetMenu]
public class AgentSoundData : ScriptableObject
{
    public AudioClip Spawn;
    public AudioClip Die;
    public AudioClip Hurt;
    public AudioClip[] Footsteps;
}