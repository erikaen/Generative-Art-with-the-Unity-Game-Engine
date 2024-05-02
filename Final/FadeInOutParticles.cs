using System.Linq;
using System.Text;
using UnityEngine;

public class FadeInOutParticles : MonoBehaviour
{
    private EffectSettings effectSettings;
    private ParticleSystem[] particles;
    private bool oldVisibleStat;
    public float containmentRadius = 5f;  
    public float minimumDistance = 0.5f;  
    public float spacingSpeed = 1f;       
    public Vector2 sizeRange = new Vector2(1f, 1f); 

    private void GetEffectSettingsComponent(Transform tr)
    {
        var parent = tr.parent;
        if (parent != null)
        {
            effectSettings = parent.GetComponentInChildren<EffectSettings>();
            if (effectSettings == null)
                GetEffectSettingsComponent(parent.transform);
        }
    }

    void Start()
    {
        GetEffectSettingsComponent(transform);
        particles = effectSettings.GetComponentsInChildren<ParticleSystem>();
        oldVisibleStat = effectSettings.IsVisible;
    }

    void Update()
    {
        if (effectSettings.IsVisible != oldVisibleStat)
        {
            foreach (var particleSystem in particles)
            {
                if (effectSettings.IsVisible)
                {
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Stop();
                }
            }
            oldVisibleStat = effectSettings.IsVisible;
        }

        // Contain particles within the sphere and maintain spacing
        foreach (var particleSystem in particles)
        {
            ContainParticles(particleSystem);
            MaintainParticleSpacing(particleSystem);
        }
    }

    private void ContainParticles(ParticleSystem ps)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        int numParticlesAlive = ps.GetParticles(particles);
        Vector3 center = transform.position;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (Vector3.Distance(center, particles[i].position) > containmentRadius)
            {
                
                Vector3 randomDirection = Random.insideUnitSphere.normalized;
                particles[i].position = center + randomDirection * containmentRadius;
                particles[i].velocity = randomDirection * Random.Range(0.5f, 1.5f); // Randomize velocity within range
            }

            // Randomize size
            float randomSize = Random.Range(sizeRange.x, sizeRange.y);
            particles[i].startSize = randomSize;
        }

        ps.SetParticles(particles, numParticlesAlive);
    }

    private void MaintainParticleSpacing(ParticleSystem ps)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        int numParticlesAlive = ps.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            for (int j = i + 1; j < numParticlesAlive; j++)
            {
                Vector3 pos1 = particles[i].position;
                Vector3 pos2 = particles[j].position;
                float dist = Vector3.Distance(pos1, pos2);

                if (dist < minimumDistance)
                {
                    Vector3 direction = (pos1 - pos2).normalized;
                    float correction = (minimumDistance - dist) / 2.0f;
                    particles[i].position += direction * correction * spacingSpeed * Time.deltaTime;
                    particles[j].position -= direction * correction * spacingSpeed * Time.deltaTime;
                }
            }
        }

        ps.SetParticles(particles, numParticlesAlive);
    }
}
