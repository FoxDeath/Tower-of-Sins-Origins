using UnityEngine;


public class PulsingLight : MonoBehaviour
{
    #region Attributes
    private UnityEngine.Rendering.Universal.Light2D lightObject;

    [SerializeField] float maxIntensity = 0.5f;

    [SerializeField] float minIntensity = 0f;

    [SerializeField] float pulseSpeed = 0.25f;

    private float targetIntensity = 1f;

    private float currentIntensity;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        lightObject = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    private void Update()
    {
        currentIntensity = Mathf.MoveTowards(lightObject.intensity, targetIntensity, Time.deltaTime * pulseSpeed);

        if(currentIntensity >= maxIntensity)
        {
            currentIntensity = maxIntensity;

            targetIntensity = minIntensity;
        }
        else if(currentIntensity <= minIntensity)
        {
            currentIntensity = minIntensity;
            
            targetIntensity = maxIntensity;
        }

        lightObject.intensity = currentIntensity;
    }
    #endregion
}
