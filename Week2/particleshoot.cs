using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShoot : MonoBehaviour
{
    public GameObject[] particlePrefabs;
    public GameObject[] lightPrefabs; // Array of light source prefabs
    public GameObject[] blackSquarePrefabs; // Array of black square prefabs
    public GameObject[] whiteSquarePrefabs; // Array of white square prefabs
    public GameObject[] blackSpherePrefabs; // Array of black sphere prefabs
    public GameObject[] whiteSpherePrefabs; // Array of white sphere prefabs
    public float shootForce = 100f;
    public int maxParticles = 10; // Maximum number of particles to generate
    public int maxSquaresAndSpheres = 5; // Maximum number of squares and spheres to generate
    public float minSize = 0.5f; // Minimum size of particles
    public float maxSize = 1.0f; // Maximum size of particles
    public float lightRadius = 5f; // Maximum radius for light spawn

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < maxParticles; i++)
            {
                ShootParticle();
                SpawnRandomLight(); // Spawn a random light source
            }
            SpawnSquaresAndSpheres(); // Spawn squares and spheres once after shooting particles
        }
    }

    void ShootParticle()
    {
        int randomIndex = Random.Range(0, particlePrefabs.Length);
        GameObject particle = Instantiate(particlePrefabs[randomIndex], transform.position, Random.rotation); // Randomize rotation

        // Randomize size
        float size = Random.Range(minSize, maxSize);
        particle.transform.localScale = new Vector3(size, size, size);

        Rigidbody rb = particle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        }
    }

    void SpawnRandomLight()
    {
        if (lightPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, lightPrefabs.Length);
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * lightRadius; // Random position within a sphere
            Instantiate(lightPrefabs[randomIndex], randomPosition, Quaternion.identity);
        }
    }

    void SpawnSquaresAndSpheres()
    {
        int squaresAndSpheresToSpawn = Mathf.Min(maxSquaresAndSpheres, blackSquarePrefabs.Length + whiteSquarePrefabs.Length + blackSpherePrefabs.Length + whiteSpherePrefabs.Length);

        for (int i = 0; i < squaresAndSpheresToSpawn; i++)
        {
            int randomIndex = Random.Range(0, 4); // Randomly choose between black and white squares and spheres

            GameObject prefabToSpawn = null;

            if (randomIndex == 0 && blackSquarePrefabs.Length > 0)
            {
                // Spawn black square
                prefabToSpawn = blackSquarePrefabs[Random.Range(0, blackSquarePrefabs.Length)];
            }
            else if (randomIndex == 1 && whiteSquarePrefabs.Length > 0)
            {
                // Spawn white square
                prefabToSpawn = whiteSquarePrefabs[Random.Range(0, whiteSquarePrefabs.Length)];
            }
            else if (randomIndex == 2 && blackSpherePrefabs.Length > 0)
            {
                // Spawn black sphere
                prefabToSpawn = blackSpherePrefabs[Random.Range(0, blackSpherePrefabs.Length)];
            }
            else if (randomIndex == 3 && whiteSpherePrefabs.Length > 0)
            {
                // Spawn white sphere
                prefabToSpawn = whiteSpherePrefabs[Random.Range(0, whiteSpherePrefabs.Length)];
            }

            if (prefabToSpawn != null)
            {
                // Spawn slightly behind the particles
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * (lightRadius - 1f);
                Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
            }
        }
    }
}

