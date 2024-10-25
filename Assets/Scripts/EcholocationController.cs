using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EcholocationController : MonoBehaviour
{
    [Header("Sound Settings")]
    public float volumeThreshold = 0.1f;
    public float minSphereScale = 0.5f;
    public float maxSphereScale = 3f;
    public float sphereLifetimeMultiplier = 0.1f;  // Multiplier to control lifetime based on pitch

    public GameObject spherePrefab;
    private SoundAnalyzer soundAnalyzer;

    void Start()
    {
        soundAnalyzer = FindObjectOfType<SoundAnalyzer>();

        if (soundAnalyzer != null)
        {
            soundAnalyzer.Initialize(volumeThreshold);
        }
        else
        {
            Debug.LogError("SoundAnalyzer not found in the scene.");
        }
    }

    void Update()
    {
        if (soundAnalyzer != null && soundAnalyzer.HasEndedSoundSegment())
        {
            float powerFactor = Mathf.Clamp(soundAnalyzer.GetSoundPower(), minSphereScale, maxSphereScale);
            float sphereLifetime = Mathf.Clamp(soundAnalyzer.GetFrequency() * sphereLifetimeMultiplier, 1f, 10f);  // Lifetime between 1 and 10 seconds

            // Spawn the sphere
            SpawnSphereWithScaleAndLifetime(powerFactor, sphereLifetime);

            soundAnalyzer.ResetSoundData();
        }

        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightIndexTriggerPressed) && isRightIndexTriggerPressed)
        {
            SpawnSphereWithScaleAndLifetime(1f, 5f);
        }
    }

    private void SpawnSphereWithScaleAndLifetime(float scale, float lifetime)
    {
        GameObject sphere = Instantiate(spherePrefab, transform.position, Quaternion.identity);
        sphere.transform.localScale = Vector3.one * scale;

        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 5f;  // Fixed speed for all spheres

        // Set the sphere to destroy itself after 'lifetime' seconds
        //Destroy(sphere, lifetime);
    }
}
