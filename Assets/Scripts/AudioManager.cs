using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    new static AudioManager audio;
    [Header("环境音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;
    [Header("动作音效")]
    public AudioClip[] attackClip;
    public AudioClip[] footClip;

    AudioSource ambientSource;
    AudioSource musicSource;
    AudioSource attackSource;
    AudioSource footSource;

    private void Awake()
    {
        audio = this;
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>(); 
        attackSource = gameObject.AddComponent<AudioSource>();
        footSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        PlayMusic();
    }
    public static void PlayMusic()
    {
        audio.musicSource.clip = audio.musicClip;
        audio.musicSource.loop = true;
        audio.musicSource.Play();
    }

    public static void PlayAttack()
    {
        int index = Random.Range(0, audio.attackClip.Length);
        audio.attackSource.clip = audio.attackClip[index];
        audio.attackSource.Play();
    }

    public static void PlayFoot()
    {
        int index = Random.Range(0, audio.footClip.Length);
        audio.footSource.clip = audio.footClip[index];
        audio.footSource.Play();
    }
}
