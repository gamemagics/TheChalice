using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.Events;

public class SoundEffects : MonoBehaviour {

    [SerializeField] private DialogueRunner runner;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] clips;

    private UnityAction<string> playAction = null;

    private void Awake() {
        runner.AddCommandHandler("PlayEffect", PlayEffect);
        runner.AddCommandHandler("PlayOnce", PlayOnce);
    }

    [YarnCommand("PlayEffect")]
    public void PlayEffect(string[] parameters) {
        int index = int.Parse(parameters[0]);
        if (index < 0 && playAction != null) {
            dialogueUI.onLineUpdate.RemoveListener(playAction);
            playAction = null;
        }
        else {
            playAction = (string foo) => {
                Play(index, float.Parse(parameters[1]));
            };
            dialogueUI.onLineUpdate.AddListener(playAction);
        }
    }

    public void PauseEffect() {
        if (audioSource.isPlaying) {
            audioSource.Pause();
        }
    }

    [YarnCommand("PlayOnce")]
    public void PlayOnce(string[] parameters) {
        if (playAction != null) {
            dialogueUI.onLineUpdate.RemoveListener(playAction);
        }
        Play(int.Parse(parameters[0]), float.Parse(parameters[1]));
    }

    private void Play(int index, float volume) {
        audioSource.clip = clips[index];
        audioSource.volume = volume;
        audioSource.Play();
    }
}
