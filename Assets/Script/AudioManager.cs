using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource audiobackground;
    [SerializeField] AudioClip audiobackgroundclip;
    [SerializeField] AudioSource EffectAudio;
    [SerializeField] AudioClip Coinclip;
    [SerializeField] AudioClip Attackclip;
    [SerializeField] AudioClip Jumpclip;
    [SerializeField] AudioClip Hurt;
    [SerializeField] AudioClip pickkey;
    [SerializeField] AudioClip openChest;
    [SerializeField] AudioClip openDoor;
    [SerializeField] AudioClip dash;
    [SerializeField] AudioClip skill;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayAudioBackGround();
    }

    public void PlayAudioBackGround()
    {
        if (audiobackground != null && audiobackgroundclip != null)
        {
            audiobackground.clip = audiobackgroundclip;
            audiobackground.loop = true;
            audiobackground.Play();
        }
    }

    public void AttackAudio() => EffectAudio.PlayOneShot(Attackclip);
    public void CoinClip() => EffectAudio.PlayOneShot(Coinclip);
    public void JumpAudio() => EffectAudio.PlayOneShot(Jumpclip);
    public void HurtClip() => EffectAudio.PlayOneShot(Hurt);
    public void PickKey() => EffectAudio.PlayOneShot(pickkey);
    public void OpenChest() => EffectAudio.PlayOneShot(openChest);
    public void OpenDoor() => EffectAudio.PlayOneShot(openDoor);
    public void Dash() => EffectAudio.PlayOneShot(dash);
    public void Skill() => EffectAudio.PlayOneShot(skill);
}
