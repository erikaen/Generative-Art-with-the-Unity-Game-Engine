using UnityEngine;

public class FadeInOutShaderColor : MonoBehaviour
{
    public string ShaderColorName = "_Color";
    public float StartDelay = 0;
    public float FadeInSpeed = 0;
    public float FadeOutDelay = 0;
    public float FadeOutSpeed = 0;
    public bool UseSharedMaterial;

    private Material mat;
    private Color oldColor, currentColor;
    private float oldAlpha, alpha;
    private bool canStart, canStartFadeOut, fadeInCompleted, fadeOutCompleted;

    private void Start()
    {
        InitMaterial();
    }

    private void InitMaterial()
    {
        if (UseSharedMaterial)
        {
            if (GetComponent<Renderer>() != null) mat = GetComponent<Renderer>().sharedMaterial;
        }
        else
        {
            if (GetComponent<Renderer>() != null) mat = GetComponent<Renderer>().material;
        }

        if (mat == null) return;

        oldColor = mat.GetColor(ShaderColorName);
        currentColor = oldColor;

        if (FadeInSpeed > 0.001f) currentColor.a = 0;
        mat.SetColor(ShaderColorName, currentColor);

        oldAlpha = currentColor.a;

        if (StartDelay > 0.001f)
            Invoke("SetupStartDelay", StartDelay);
        else
            canStart = true;

        if (FadeInSpeed <= 0.001f && FadeOutDelay > 0)
            Invoke("SetupFadeOutDelay", FadeOutDelay);
    }

    void OnEnable()
    {
        InitMaterial();
    }

    private void SetupStartDelay()
    {
        canStart = true;
    }

    private void SetupFadeOutDelay()
    {
        canStartFadeOut = true;
    }

    private void Update()
    {
        if (!canStart) return;

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
        alpha = oldAlpha + Time.deltaTime / FadeInSpeed;
        if (alpha >= oldColor.a)
        {
            alpha = oldColor.a;
            fadeInCompleted = true;
            oldAlpha = alpha;
            if (FadeOutDelay > 0)
                Invoke("SetupFadeOutDelay", FadeOutDelay);
            else
                canStartFadeOut = true;
        }
        currentColor.a = alpha;
        mat.SetColor(ShaderColorName, currentColor);
    }

    private void FadeOut()
    {
        alpha = oldAlpha - Time.deltaTime / FadeOutSpeed;
        if (alpha <= 0)
        {
            alpha = 0;
            fadeOutCompleted = true;
        }
        currentColor.a = alpha;
        mat.SetColor(ShaderColorName, currentColor);
        oldAlpha = alpha;
    }
}
