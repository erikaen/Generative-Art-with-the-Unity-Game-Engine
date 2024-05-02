using UnityEngine;

public class FadeInOutLight : MonoBehaviour
{
    public float StartDelay = 0;       
    public float FadeInSpeed = 0;      
    public float FadeOutDelay = 0;     
    public float FadeOutSpeed = 0;     

    private Light goLight;             
    private float startIntensity;      
    private float currentIntensity;    
    private bool canStart;             
    private bool canStartFadeOut;      
    private bool fadeInCompleted;      
    private bool fadeOutCompleted;     

    private void Start()
    {
        goLight = GetComponent<Light>();
        startIntensity = goLight.intensity;
        InitDefaultVariables();
    }

    private void InitDefaultVariables()
    {
        fadeInCompleted = false;
        fadeOutCompleted = false;
        canStartFadeOut = false;
        currentIntensity = 0;

        goLight.intensity = (FadeInSpeed > 0.001f) ? 0 : startIntensity;

        if (StartDelay > 0.001f)
            Invoke("SetupStartDelay", StartDelay);
        else
            canStart = true;

        if (FadeInSpeed <= 0.001f && FadeOutDelay > 0)
            Invoke("SetupFadeOutDelay", FadeOutDelay);
    }

    void OnEnable()
    {
        InitDefaultVariables();
    }

    void SetupStartDelay()
    {
        canStart = true;
    }

    void SetupFadeOutDelay()
    {
        canStartFadeOut = true;
    }

    private void Update()
    {
        if (!canStart)
            return;

        if (FadeInSpeed > 0.001f && !fadeInCompleted)
        {
            FadeIn();
        }

        if (FadeOutSpeed > 0.001f && !fadeOutCompleted && canStartFadeOut)
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        currentIntensity += Time.deltaTime / FadeInSpeed * startIntensity;
        if (currentIntensity >= startIntensity)
        {
            currentIntensity = startIntensity;
            fadeInCompleted = true;
            if (FadeOutDelay > 0)
                Invoke("SetupFadeOutDelay", FadeOutDelay);
            else
                canStartFadeOut = true;
        }
        goLight.intensity = currentIntensity;
    }

    private void FadeOut()
    {
        currentIntensity -= Time.deltaTime / FadeOutSpeed * startIntensity;
        if (currentIntensity <= 0)
        {
            currentIntensity = 0;
            fadeOutCompleted = true;
        }
        goLight.intensity = currentIntensity;
    }
}
