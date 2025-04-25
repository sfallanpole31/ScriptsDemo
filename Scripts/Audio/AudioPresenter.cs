using UniRx;
using VContainer;

public class AudioPresenter
{
    [Inject]
    public AudioPresenter(Model model, AudioView audioView, UIManagerView uIManagerView, GameSetting gameSetting)
    {
        model.InitGrid.Subscribe(_ =>
        {
            audioView.bgmAudioSource.clip = audioView.BGM;
            audioView.bgmAudioSource.Play();

            if (gameSetting.isMusicOn)
            {
                audioView.bgmAudioSource.mute = false;
                uIManagerView.bgmButton.isOn = true;
            }
            else
            { 
                audioView.bgmAudioSource.mute = true;
                uIManagerView.bgmButton.isOn = false;
            }


            if (gameSetting.isSoundOn)
            {
                audioView.explodeAudioSource.mute = false;
                audioView.dropSoundSource.mute = false;
                uIManagerView.soundButton.isOn = true;
            }
            else
            {
                audioView.explodeAudioSource.mute = true;
                audioView.dropSoundSource.mute = true;
                uIManagerView.soundButton.isOn = false;
            }

        });

        model.DroppingFruitModel.Subscribe(_ =>
        {
            audioView.PlayDropeSound();
        });

        model.OnFruitMatchDestroy.Subscribe(_ =>
        {
            audioView.PlayExplodeSound();
        });

        uIManagerView.bgmButton.OnValueChangedAsObservable()
            .Skip(1)
            .Subscribe(_ =>
            {
                if (uIManagerView.bgmButton.isOn)
                {
                    audioView.bgmAudioSource.mute = false;
                    gameSetting.isMusicOn = true;
                }
                else
                {
                    audioView.bgmAudioSource.mute = true;
                    gameSetting.isMusicOn = false;
                }

            });

        uIManagerView.soundButton.OnValueChangedAsObservable()
            .Skip(1)
            .Subscribe(_ =>
            {

                if (uIManagerView.soundButton.isOn)
                {
                    audioView.explodeAudioSource.mute = false;
                    audioView.dropSoundSource.mute = false;
                    gameSetting.isSoundOn = true;
                }
                else
                {
                    audioView.explodeAudioSource.mute = true;
                    audioView.dropSoundSource.mute = true;
                    gameSetting.isSoundOn = false;
                }

        
            });

    }


}
