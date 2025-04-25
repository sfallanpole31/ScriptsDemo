using UnityEngine;

public class AudioView : MonoBehaviour
{
    public AudioSource bgmAudioSource;
    public AudioSource explodeAudioSource;
    public AudioSource dropSoundSource;

    public AudioClip BGM;
    public AudioClip explodeSound;
    public AudioClip dropSound;


    public void PlayExplodeSound()
    {
        // 如果正在播這個 clip，就不重播
        if (explodeAudioSource.isPlaying)
            return;

        explodeAudioSource.clip = explodeSound;
        explodeAudioSource.Play();
        print("爆炸音效");
    }
    public void PlayDropeSound()
    {
        // 如果正在播這個 clip，就不重播
        if (dropSoundSource.isPlaying )
            return;

        dropSoundSource.clip = dropSound;
        dropSoundSource.Play();
        print("掉落音效");
    }


}
