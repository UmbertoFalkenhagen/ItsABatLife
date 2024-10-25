using UnityEngine;
using UnityEngine.XR;

public class EcholocationController : MonoBehaviour
{
    [Header("Sound Settings")]
    public float volumeThreshold = 0.1f;       // Volume threshold for detection
    public float minSphereScale = 0.5f;        // Minimum scale for sphere size
    public float maxSphereScale = 3f;          // Maximum scale for sphere size
    public float sphereSpeedMultiplier = 2f;   // Speed multiplier for sphere based on pitch

    public GameObject spherePrefab;            // Sphere prefab for echolocation pulse

    private SoundAnalyzer soundAnalyzer;

    void Start()
    {
        // Locate SoundAnalyzer in the scene and initialize it with threshold settings
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
            // Calculate sphere properties based on sound data
            float powerFactor = Mathf.Clamp(soundAnalyzer.GetSoundPower(), minSphereScale, maxSphereScale);
            float sphereSpeed = Mathf.Clamp(soundAnalyzer.GetFrequency() * sphereSpeedMultiplier, 1f, 20f);

            // Spawn the sphere
            SpawnSphereWithScaleAndSpeed(powerFactor, sphereSpeed);

            // Reset sound data for the next sound segment
            soundAnalyzer.ResetSoundData();
        }

        // Testing with right index trigger
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightIndexTriggerPressed) && isRightIndexTriggerPressed)
        {
            // Test: Spawn sphere with default power and speed if the trigger is pressed
            SpawnSphereWithScaleAndSpeed(1f, 5f);
        }
    }

    private void SpawnSphereWithScaleAndSpeed(float scale, float speed)
    {
        GameObject sphere = Instantiate(spherePrefab, transform.position, Quaternion.identity);
        sphere.transform.localScale = Vector3.one * scale;

        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }
}
