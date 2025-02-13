using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance { get; private set; } // Singleton Instance

    public enum ObjectType
    {
        LandingSpot,
        Obstacle,
        Enemy,
        Collectible,
        SpecialCollectible,
        Interactable
    }

    [Header("Mode Settings")]
    public string mode = "Normal"; // Change to "Colorblind" for colorblind mode

    [Header("Materials")]
    private Dictionary<ObjectType, Material> normalMaterials = new Dictionary<ObjectType, Material>();
    private Dictionary<ObjectType, Material> colorblindMaterials = new Dictionary<ObjectType, Material>();

    [Header("Material Assignments")]
    public Material landingSpotMaterialNormal;
    public Material obstacleMaterialNormal;
    public Material enemyMaterialNormal;
    public Material collectibleMaterialNormal;
    public Material specialCollectibleMaterialNormal;
    public Material interactableMaterialNormal;

    public Material landingSpotMaterialColorblind;
    public Material obstacleMaterialColorblind;
    public Material enemyMaterialColorblind;
    public Material collectibleMaterialColorblind;
    public Material specialCollectibleMaterialColorblind;
    public Material interactableMaterialColorblind;

    private void Awake()
    {
        // Singleton Enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void Start()
    {
        InitializeDictionaries();
        ApplyMaterials();
    }

    private void InitializeDictionaries()
    {
        // Normal Mode Materials
        normalMaterials[ObjectType.LandingSpot] = landingSpotMaterialNormal;
        normalMaterials[ObjectType.Obstacle] = obstacleMaterialNormal;
        normalMaterials[ObjectType.Enemy] = enemyMaterialNormal;
        normalMaterials[ObjectType.Collectible] = collectibleMaterialNormal;
        //normalMaterials[ObjectType.SpecialCollectible] = specialCollectibleMaterialNormal;
        //normalMaterials[ObjectType.Interactable] = interactableMaterialNormal;

        // Colorblind Mode Materials
        colorblindMaterials[ObjectType.LandingSpot] = landingSpotMaterialColorblind;
        colorblindMaterials[ObjectType.Obstacle] = obstacleMaterialColorblind;
        colorblindMaterials[ObjectType.Enemy] = enemyMaterialColorblind;
        colorblindMaterials[ObjectType.Collectible] = collectibleMaterialColorblind;
        //colorblindMaterials[ObjectType.SpecialCollectible] = specialCollectibleMaterialColorblind;
        //colorblindMaterials[ObjectType.Interactable] = interactableMaterialColorblind;
    }

    private void ApplyMaterials()
    {
        Dictionary<ObjectType, Material> selectedMaterials = (mode == "Colorblind") ? colorblindMaterials : normalMaterials;

        ApplyMaterialToTaggedObjects("LandingSpot", selectedMaterials[ObjectType.LandingSpot]);
        ApplyMaterialToTaggedObjects("Obstacle", selectedMaterials[ObjectType.Obstacle]);
        ApplyMaterialToTaggedObjects("Enemy", selectedMaterials[ObjectType.Enemy]);
        ApplyMaterialToTaggedObjects("Collectible", selectedMaterials[ObjectType.Collectible]);
        //ApplyMaterialToTaggedObjects("SpecialCollectible", selectedMaterials[ObjectType.SpecialCollectible]);
        //ApplyMaterialToTaggedObjects("Interactable", selectedMaterials[ObjectType.Interactable]);
    }

    private void ApplyMaterialToTaggedObjects(string tag, Material material)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.material = material;
            }

            // Ensure the object has a GlowingObstacle component
            EnsureGlowingObstacleComponent(obj);
        }
    }

    private void EnsureGlowingObstacleComponent(GameObject obj)
    {
        if (obj.GetComponent<GlowingObstacle>() == null)
        {
            obj.AddComponent<GlowingObstacle>();
        }
    }

    public void SetMode(string newMode)
    {
        mode = newMode;
        ApplyMaterials();
    }
}
