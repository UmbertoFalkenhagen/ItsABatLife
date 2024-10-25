using UnityEngine;

public class GlowingObstacle : MonoBehaviour
{
    [Header("Glow Settings")]
    public float glowIncreaseFactor = 0.2f;        // Amount to increase glow per hit
    public float maxGlowIntensity = 5f;            // Maximum emission intensity
    public float glowDecreaseRate = 0.5f;          // Rate at which the glow decreases over time
    public float glowDelay = 1.5f;                 // Delay before dimming starts

    private Material obstacleMaterial;
    private float currentGlowIntensity = 0f;       // Current emission intensity
    private float timeSinceLastHit = 0f;           // Timer to track delay

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            obstacleMaterial = renderer.material;
            obstacleMaterial.EnableKeyword("_EMISSION");
            obstacleMaterial.SetColor("_EmissionColor", Color.black);  // Start with no glow
        }
        else
        {
            Debug.LogError("No Renderer found on the obstacle. Please add a Renderer component.");
        }
    }

    void Update()
    {
        // Update time since last hit
        timeSinceLastHit += Time.deltaTime;

        // Gradually decrease the glow intensity after the delay
        if (timeSinceLastHit > glowDelay && currentGlowIntensity > 0)
        {
            currentGlowIntensity -= glowDecreaseRate * Time.deltaTime;
            currentGlowIntensity = Mathf.Max(currentGlowIntensity, 0f);

            Color emissionColor = Color.white * currentGlowIntensity;
            obstacleMaterial.SetColor("_EmissionColor", emissionColor);
        }
    }

    public void HitBySoundwave(float sphereSize)
    {
        // Increase the glow based on sphere size
        currentGlowIntensity += sphereSize * glowIncreaseFactor;
        currentGlowIntensity = Mathf.Min(currentGlowIntensity, maxGlowIntensity);

        // Update the material's emission color
        Color emissionColor = Color.white * currentGlowIntensity;
        obstacleMaterial.SetColor("_EmissionColor", emissionColor);

        // Reset the delay timer
        timeSinceLastHit = 0f;
    }
}
