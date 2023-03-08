
using TsSDK;
using UnityEngine;
using UnityEngine.UI;

public class TsHapticAnimationPlayback : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_pauseButton;
    [SerializeField] private Button m_stopButton;
    [SerializeField] private Slider m_progressSlider;


    [SerializeField]
    private TsHapticPlayer m_hapticPlayer;

    [SerializeField]
    private TsHapticAnimationAsset m_animationAsset;

    private void Start()
    {
        m_playButton.onClick.AddListener(Play);
        m_pauseButton.onClick.AddListener(Pause);
        m_stopButton.onClick.AddListener(Stop);
    }

    private void Update()
    {
        if (m_hapticPlayer == null || m_hapticPlayer.PlayerHandle == null)
        {
            return;
        }
        var playable = m_hapticPlayer.GetPlayable(m_animationAsset.Instance as IHapticAsset);
        if (playable.IsPlaying)
        {
            var duration = GetDuration();
            var time = GetTime();

            var progress = ((float) time) / duration;
            m_progressSlider.value = progress;
        }
    }

    public void Play()
    {
        if (m_hapticPlayer == null)
        {
            return;
        }
        var playable = m_hapticPlayer.GetPlayable(m_animationAsset.Instance as IHapticAsset);

        playable.Play();
    }

    public void Stop()
    {
        if (m_hapticPlayer == null)
        {
            return;
        }
        var playable = m_hapticPlayer.GetPlayable(m_animationAsset.Instance as IHapticAsset);
        playable.Stop();
    }

    public void Pause()
    {
        if (m_hapticPlayer == null)
        {
            return;
        }
        var playable = m_hapticPlayer.GetPlayable(m_animationAsset.Instance as IHapticAsset);
        playable.IsPaused = true;
    }

    public ulong GetTime()
    {
        if (m_hapticPlayer == null)
        {
            return 0;
        }
        var playable = m_hapticPlayer.GetPlayable(m_animationAsset.Instance as IHapticAsset);
        return playable.TimeMs;
    }

    public ulong GetDuration()
    {
        if (m_hapticPlayer == null)
        {
            return 0;
        }
        var playable = m_hapticPlayer.GetPlayable(m_animationAsset.Instance as IHapticAsset);
        return playable.DurationMs;
    }
}
