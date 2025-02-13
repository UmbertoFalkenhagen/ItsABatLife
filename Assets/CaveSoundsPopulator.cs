using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveSoundsPopulator : MonoBehaviour
{
    [Header("Cave Sound Settings")]
    public List<Transform> soundAreas; // List of Sound Area Markers (Invisible Spheres)
    public float minWindInterval = 15f; // Minimum time between wind sounds
    public float maxWindInterval = 30f; // Maximum time between wind sounds
    public float minCaveSoundInterval = 5f; // Minimum time between cave sounds
    public float maxCaveSoundInterval = 12f; // Maximum time between cave sounds

    [Header("Echolocation Settings")]
    public GameObject echolocationSpherePrefab; // Prefab for the echolocation sphere
    public float sphereLifetime = 3f; // Lifetime of each sphere
    public float sphereSpeed = 3f; // Speed at which the spheres travel
    public int sphereCount = 8; // Always shoot exactly 8 spheres

    private List<string> caveSounds = new List<string> { "waterdrop1", "waterdrop2", "waterdrop3", "waterdrop4", "sand", "animal" };

    void Start()
    {
        if (soundAreas.Count == 0)
        {
            Debug.LogError("CaveSoundsPopulator: No sound areas assigned! Place SoundAreaMarker prefabs in the scene.");
            return;
        }

        // Start playing environmental sounds
        StartCoroutine(PlayWindSounds());
        StartCoroutine(PlayCaveSounds());
    }

    private IEnumerator PlayWindSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWindInterval, maxWindInterval));

            Vector3 randomPosition = GetRandomPositionInArea();
            float randomVolume = Random.Range(0.5f, 2f); // Ensure wind has a minimum volume of 0.5 and a max of 2

            // Play the wind sound at a random position
            SoundManager.Instance.PlayAtWorldPosition("Wind_Background", randomPosition, randomVolume);
        }
    }

    private IEnumerator PlayCaveSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minCaveSoundInterval, maxCaveSoundInterval));

            string randomSound = caveSounds[Random.Range(0, caveSounds.Count)];
            Vector3 randomPosition = GetRandomPositionInArea();
            float randomVolume = Random.Range(0.5f, 2f); // Ensure cave sounds have a minimum volume of 0.5 and a max of 2

            // Play cave sound at world position
            SoundManager.Instance.PlayAtWorldPosition(randomSound, randomPosition, randomVolume);

            // Generate echolocation spheres in a circular pattern
            SpawnEcholocationSpheres(randomPosition);
        }
    }

    /// <summary>
    /// Gets a random position inside one of the sound areas.
    /// </summary>
    private Vector3 GetRandomPositionInArea()
    {
        Transform randomArea = soundAreas[Random.Range(0, soundAreas.Count)];
        float radius = randomArea.localScale.x * 0.5f; // Get the actual radius of the sphere

        // Get a random position within the sphere
        Vector3 randomOffset = Random.insideUnitSphere * radius;
        return randomArea.position + randomOffset;
    }

    /// <summary>
    /// Spawns exactly 8 echolocation spheres in a circular pattern.
    /// </summary>
    private void SpawnEcholocationSpheres(Vector3 origin)
    {
        float angleStep = 360f / sphereCount; // Divide the circle into equal parts

        for (int i = 0; i < sphereCount; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

            GameObject sphere = Instantiate(echolocationSpherePrefab, origin, Quaternion.identity);
            Rigidbody rb = sphere.GetComponent<Rigidbody>();

            // Set velocity in a circular pattern
            rb.velocity = direction * sphereSpeed;

            // Destroy the sphere after its lifetime
            Destroy(sphere, sphereLifetime);
        }
    }
}
