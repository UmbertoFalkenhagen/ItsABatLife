using UnityEngine;

public class GlowingObstacle : MonoBehaviour
{
    [Header("Glow Settings")]
    public float glowIncreaseFactor = 0.2f;      // Amount to increase glow per hit
    public float maxGlowIntensity = 5f;         // Maximum emission intensity
    public float glowDecreaseRate = 0.5f;       // Rate at which the glow decreases over time
    public float glowDelay = 1.5f;              // Delay before dimming starts

    private Material obstacleMaterial;
    private float currentGlowIntensity = 0f;    // Current emission intensity
    private float timeSinceLastHit = 0f;        // Timer to track glow decay delay
    private Color baseEmissionColor;            // Stores the object's original emission color

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            obstacleMaterial = renderer.material;
            obstacleMaterial.EnableKeyword("_EMISSION");

            // Get the base emission color of the object's material
            baseEmissionColor = obstacleMaterial.GetColor("_EmissionColor");

            // Ensure object starts with NO glow (completely dark)
            obstacleMaterial.SetColor("_EmissionColor", Color.black);
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

            // Apply the object's original emission color multiplied by the intensity
            obstacleMaterial.SetColor("_EmissionColor", baseEmissionColor * currentGlowIntensity);
        }
    }

    public void HitBySoundwave(float sphereSize)
    {
        // Increase the glow based on sphere size
        currentGlowIntensity += sphereSize * glowIncreaseFactor;
        currentGlowIntensity = Mathf.Min(currentGlowIntensity, maxGlowIntensity);

        // Apply the object's original emission color multiplied by the new intensity
        obstacleMaterial.SetColor("_EmissionColor", baseEmissionColor * currentGlowIntensity);

        // Reset the delay timer
        timeSinceLastHit = 0f;
    }
}
