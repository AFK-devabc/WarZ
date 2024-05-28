using UnityEngine;
using System.Collections;

public class PrLightCurveAnimator : MonoBehaviour {

    private Light LightComp;
    public AnimationCurve LightFactor;
    public Renderer LightMaterial;
    public bool affectRange = false;
    private float ActualLightFactor = 0.0f;
    private float ActualLightMaterialFactor = 0.0f;
    private float ActualLightRange = 0.0f;
    
    public float FactorSpeed = 1.0f;
    public bool active = true;
    private float randomStart = 0.0f;

	// Use this for initialization
	void Start () {
        if (GetComponent<Light>())
        {
            LightComp = GetComponent<Light>();
            ActualLightFactor = LightComp.intensity;
            ActualLightRange = LightComp.range;
            if (LightMaterial)
                ActualLightMaterialFactor = LightMaterial.material.GetFloat("_EmissionFactor");
            randomStart = Random.Range(0.0f, 0.9f);
        }
           
        else
            DestroyComp();
   }
	
	// Update is called once per frame
	void Update () {
        if (LightComp && active)
        {
            LightComp.intensity = ActualLightFactor * LightFactor.Evaluate((Time.time + randomStart) * FactorSpeed);
            if (affectRange)
                LightComp.range = ActualLightRange * LightFactor.Evaluate((Time.time + randomStart) * FactorSpeed);

            if (LightMaterial)
                LightMaterial.material.SetFloat("_EmissionFactor", ActualLightMaterialFactor * LightFactor.Evaluate(Time.time * FactorSpeed));
        }
    }

    void DestroyComp()
    {
        Debug.LogWarning("Light curve animator script can´t find light component on gameobject : " + gameObject.name + "Destroying Component...");
        Destroy(this);
    }
}
