using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

public class TsTouchPlayback : MonoBehaviour
{
    [SerializeField] 
    private TsHapticPlayer m_hapticPlayer;

    [Range(1, 150)]
    public int frequency;

    [Range(1, 100)]
    public int amplitude;

    [Range(1, 320)]
    public int pulseWidth;

    [Range(10, 10000)]
    public int durationMs;

    public bool looped = false;

    public int currentChannelIndex = 0;

    public TsHumanBoneIndex TargetBoneIndex = TsHumanBoneIndex.LeftThumbDistal;

    private IHapticDynamicPlayable m_hapticPlayable;

    private Dictionary<TsHumanBoneIndex, List<IMapping2dElectricChannel>> m_channels = new Dictionary<TsHumanBoneIndex, List<IMapping2dElectricChannel>>();


    private void Validate()
    {
        ValidateChannels();
        if (m_hapticPlayable != null)
        {
            m_hapticPlayable.Stop();
        }

        m_hapticPlayable = m_hapticPlayer.CreateTouch(frequency, amplitude, pulseWidth, durationMs);
        m_hapticPlayable.IsLooped = looped;
    }

    private void ValidateChannels()
    {
        m_channels.Clear();
        var channels = m_hapticPlayer.Device.Mapping2d.ElectricChannels;
        foreach (var channel in channels)
        {
            if (!m_channels.ContainsKey(channel.BoneIndex))
            {
                m_channels.Add(channel.BoneIndex, new List<IMapping2dElectricChannel>());
            }
            m_channels[channel.BoneIndex].Add(channel);
        }
    }

    public void Play(int channelIndex)
    {
        Validate();

        if (!m_channels.TryGetValue(TargetBoneIndex, out var channelsGroup))
        {
            return;
        }

        var index = channelIndex % channelsGroup.Count;
        var channel = channelsGroup[index];
        m_hapticPlayable.Play();
        m_hapticPlayable.AddChannel(channel);
    }

    public void PlayCurrent()
    {
        Play(currentChannelIndex);
    }

    public void TogglePause()
    {
        m_hapticPlayable.IsPaused = !m_hapticPlayable.IsPaused;
    }

    public void Stop()
    {
        m_hapticPlayable.Stop();
    }
}
