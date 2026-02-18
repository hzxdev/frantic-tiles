using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteHandler : MonoBehaviour
{
    SpriteRenderer ownRenderer;
    public SpriteRenderer emotePlaceholder;
    public AudioManager audioManager;
    public Emote emoteToBePlayed;
    bool isPlayingEmote;
    PlayerController pcont;

    void Start()
    {
        ownRenderer = GetComponent<SpriteRenderer>();
        pcont = GetComponent<PlayerController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G) && pcont.IsTypingSomewhere())
        {
            PlayEmote(emoteToBePlayed);
        }
        if(!pcont.isAlive)
        {
            emotePlaceholder.enabled = false;
            isPlayingEmote = false;
        }

        emotePlaceholder.flipX = ownRenderer.flipX;
    }

    public void PlayEmote(Emote emote)
    {
        if (isPlayingEmote || !pcont.isAlive) return;
        Invoke("StopCurrentEmote", emote.duration);
        emotePlaceholder.enabled = true;
        emotePlaceholder.sprite = emote.sprite;
        audioManager.PlayOneShot(emote.soundName, false);
        isPlayingEmote = true;
    }

    void StopCurrentEmote()
    {
        emotePlaceholder.enabled = false;
        isPlayingEmote = false;
    }
}
