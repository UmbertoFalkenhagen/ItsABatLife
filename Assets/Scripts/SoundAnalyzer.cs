using UnityEngine;

public class SoundAnalyzer : MonoBehaviour
{
    private AudioClip microphoneClip;
    private bool isInitialized = false;
    private bool isSounding = false;
    private float soundPowerAccumulated = 0f;
    private float soundDuration = 0f;
    private float frequency = 0f;
    private float volumeThreshold;

    public void Initialize(float volumeThreshold)
    {
        this.volumeThreshold = volumeThreshold;
        microphoneClip = Microphone.Start(null, true, 1, 44100);
        isInitialized = true;
    }

    void Update()
    {
        if (isInitialized && Microphone.IsRecording(null))
        {
            float volume = GetMicrophoneVolume();

            if (volume > volumeThreshold)
            {
                if (!isSounding)
                {
                    isSounding = true;
                    soundPowerAccumulated = 0f;
                    soundDuration = 0f;
                }

                soundPowerAccumulated += volume;
                soundDuration += Time.deltaTime;
                frequency = AnalyzeFrequency();
            }
            else if (isSounding)
            {
                isSounding = false;
            }
        }
    }

    public bool HasEndedSoundSegment()
    {
        return !isSounding && soundDuration > 0f;
    }

    public float GetSoundPower()
    {
        return soundPowerAccumulated * soundDuration;
    }

    public float GetFrequency()
    {
        return frequency;
    }

    public float GetSoundDuration()
    {
        return soundDuration;
    }

    public void ResetSoundData()
    {
        soundPowerAccumulated = 0f;
        soundDuration = 0f;
        frequency = 0f;
    }

    private float GetMicrophoneVolume()
    {
        float[] data = new float[256];
        int micPosition = Microphone.GetPosition(null) - 256;
        if (micPosition < 0) return 0;

        microphoneClip.GetData(data, micPosition);

        float sum = 0f;
        foreach (float sample in data)
        {
            sum += Mathf.Abs(sample);
        }

        return sum / data.Length;
    }

    private float AnalyzeFrequency()
    {
        float[] data = new float[1024];
        microphoneClip.GetData(data, 0);

        // Basic FFT
        float[] spectrum = new float[1024];
        FFT(data, spectrum);

        float maxVal = 0f;
        int maxIndex = 0;
        for (int i = 0; i < spectrum.Length; i++)
        {
            if (spectrum[i] > maxVal)
            {
                maxVal = spectrum[i];
                maxIndex = i;
            }
        }

        float freq = maxIndex * AudioSettings.outputSampleRate / 2f / spectrum.Length;
        return freq;
    }

    // Basic FFT placeholder — in production, use Unity's DSP API or a proper FFT plugin if needed
    private void FFT(float[] input, float[] output)
    {
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = Mathf.Abs(input[i]); // crude approximation just to keep your logic functional
        }
    }
}
