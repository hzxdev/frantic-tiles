using UnityEngine.Audio;
using System;
using UnityEngine;
using Photon.Pun;

public class AudioManager : MonoBehaviourPun
{

    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
 
    }



    // Update is called once per frame
    public void Play(string name)
    {
        Sound s =  Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Play();
    }

    public void PlayOneShot(string name, bool rpc)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.PlayOneShot(s.clip);
        if (rpc)
            photonView.RPC(nameof(PlayOneShotRPC), RpcTarget.Others, name);
    }

    public void PlayOneShotIfNotPlaying(string name, bool rpc)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        if(!s.source.isPlaying)
        {
            PlayOneShot(name, rpc);
        }
        
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }

    public AudioSource ReturnSource(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source;
    }

    [PunRPC]
    public void PlayOneShotRPC(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.PlayOneShot(s.clip);
    }

}
