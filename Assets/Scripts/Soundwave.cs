using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundwave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a GlowingObstacle component
        GlowingObstacle glowingObstacle = collision.gameObject.GetComponent<GlowingObstacle>();
        if (glowingObstacle != null)
        {
            // Get the sphere's scale as a measure of its size and pass it to HitBySoundwave
            float sphereSize = transform.localScale.x;
            glowingObstacle.HitBySoundwave(sphereSize);

            // Destroy the sphere after it has impacted the obstacle
            Destroy(gameObject);
        }
    }
}
