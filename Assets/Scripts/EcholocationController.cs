using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class EcholocationController : MonoBehaviour
{
    [Header("Sound Settings")]
    public float volumeThreshold = 0.1f;
    public float minSphereScale = 0.5f;
    public float maxSphereScale = 3f;
    public float minSphereSpeed = 2f;
    public float maxSphereSpeed = 8f;
    public float baseLifetime = 10f;

    public GameObject spherePrefab;

    private SoundAnalyzer soundAnalyzer;
    private Camera xrCamera;

    // TEST input
    private bool testButtonHeld = false;
    private float testHoldTime = 0f;
    public float testPower = 5f; // fixed test speed/power

    void Start()
    {
        soundAnalyzer = FindObjectOfType<SoundAnalyzer>();
        xrCamera = GetComponent<Camera>();

        if (soundAnalyzer != null)
        {
            soundAnalyzer.Initialize(volumeThreshold);
        }
        else
        {
            Debug.LogError("SoundAnalyzer not found in the scene.");
        }

        if (xrCamera == null)
        {
            Debug.LogError("EcholocationController must be on the XR Main Camera.");
        }
    }

    void Update()
    {
        if (xrCamera == null || soundAnalyzer == null) return;

        // === Real Voice Input ===
        if (soundAnalyzer.HasEndedSoundSegment())
        {
            float power = Mathf.Clamp(soundAnalyzer.GetFrequency(), minSphereSpeed, maxSphereSpeed);
            float soundDuration = soundAnalyzer.GetSoundDuration();
            int sphereCount = 1 + Mathf.FloorToInt(soundDuration) * 2;

            ShootArcedSpheres(sphereCount, power);
            soundAnalyzer.ResetSoundData();
        }

        // === Test Input (Left Index Trigger) ===
        if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTriggerHeld))
        {
            if (leftTriggerHeld)
            {
                // Start or continue holding
                if (!testButtonHeld)
                {
                    testButtonHeld = true;
                    testHoldTime = 0f;
                }

                testHoldTime += Time.deltaTime;
            }
            else if (testButtonHeld)
            {
                // Released
                testButtonHeld = false;
                int sphereCount = 1 + Mathf.FloorToInt(testHoldTime) * 2;
                ShootArcedSpheres(sphereCount, testPower);
                testHoldTime = 0f;
            }
        }

        // === Optional: right trigger quick test ===
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTriggerPressed) && rightTriggerPressed)
        {
            ShootArcedSpheres(5, 5f);
        }
    }

    void ShootArcedSpheres(int count, float speed)
    {
        if (count < 1) return;

        if (count == 1)
        {
            // Shoot directly forward
            Vector3 direction = xrCamera.transform.forward;

            GameObject sphere = Instantiate(spherePrefab, xrCamera.transform.position, Quaternion.identity);
            sphere.transform.localScale = Vector3.one;

            Rigidbody rb = sphere.GetComponent<Rigidbody>();
            rb.velocity = direction.normalized * speed;

            Destroy(sphere, baseLifetime);
            return;
        }

        // Shoot in a 90° arc
        float arcAngle = 90f;
        float angleStep = arcAngle / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angleOffset = -arcAngle / 2f + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);
            Vector3 direction = rotation * xrCamera.transform.forward;

            GameObject sphere = Instantiate(spherePrefab, xrCamera.transform.position, Quaternion.identity);
            sphere.transform.localScale = Vector3.one;

            Rigidbody rb = sphere.GetComponent<Rigidbody>();
            rb.velocity = direction.normalized * speed;

            Destroy(sphere, baseLifetime);
        }
    }
}
