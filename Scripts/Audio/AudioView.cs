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
        // �p�G���b���o�� clip�A�N������
        if (explodeAudioSource.isPlaying)
            return;

        explodeAudioSource.clip = explodeSound;
        explodeAudioSource.Play();
        print("�z������");
    }
    public void PlayDropeSound()
    {
        // �p�G���b���o�� clip�A�N������
        if (dropSoundSource.isPlaying )
            return;

        dropSoundSource.clip = dropSound;
        dropSoundSource.Play();
        print("��������");
    }


}
