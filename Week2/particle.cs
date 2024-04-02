using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleWave : MonoBehaviour
{
    public float lifetime = 5f;
    public float rotationSpeed = 50f; // Rotation speed of the particle

    void Start()
    {
        // Destroy the particle game object after the specified lifetime
        Destroy(gameObject, lifetime);

        // Enable the renderer component after a short delay
        StartCoroutine(EnableRendererDelayed(0.5f));
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    // Coroutine to enable the renderer component after a delay
    IEnumerator EnableRendererDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Renderer rend = GetComponent<Renderer>();

        // If the renderer component exists, enable it
        if (rend != null)
        {
            rend.enabled = true;
        }
    }
}
