using UnityEngine;

public class SoundAnalyzer : MonoBehaviour
{
    private AudioClip microphoneClip;
    private bool isInitialized = false;
    private bool isSounding = false;
    private float soundPowerAccumulated = 0f;
    private float soundDuration = 0f;
    private float frequency = 0f;

    // Public method to initialize with parameters from EcholocationController
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
                isSounding = true;
                soundPowerAccumulated += volume;
                soundDuration += Time.deltaTime;
                frequency = AnalyzeFrequency();  // Use AnalyzeFrequency to calculate frequency
            }
            else if (isSounding)
            {
                isSounding = false;
            }
        }
    }

    public bool HasEndedSoundSegment()
    {
        return !isSounding && soundDuration > 0;
    }

    public float GetSoundPower()
    {
        return soundPowerAccumulated * soundDuration;
    }

    public float GetFrequency()
    {
        return frequency;
    }

    public void ResetSoundData()
    {
        soundPowerAccumulated = 0f;
        soundDuration = 0f;
    }

    private float GetMicrophoneVolume()
    {
        float[] data = new float[256];
        int micPosition = Microphone.GetPosition(null) - 256;
        if (micPosition < 0) return 0;

        microphoneClip.GetData(data, micPosition);
        float totalVolume = 0;

        foreach (float sample in data)
        {
            totalVolume += Mathf.Abs(sample);
        }

        return totalVolume / data.Length;
    }

    private float AnalyzeFrequency()
    {
        float[] data = new float[1024];
        microphoneClip.GetData(data, 0);

        float[] spectrum = new float[1024];
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = microphoneClip;
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        float maxSpectrumValue = 0f;
        int maxIndex = 0;
        for (int i = 0; i < spectrum.Length; i++)
        {
            if (spectrum[i] > maxSpectrumValue)
            {
                maxSpectrumValue = spectrum[i];
                maxIndex = i;
            }
        }

        float freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;
        Destroy(audioSource);
        return freq;
    }
}
