using UnityEngine;
using UnityEngine.UI;

public class HealthbarBehavior : MonoBehaviour
{

    [SerializeField] private GameObject slider;
    private Transform camera;
    //[SerializeField] private Color low;

    //[SerializeField] private Color high;

    //[SerializeField] private Vector3 offSet;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthAni;

    private void OnEnable()
    {
        camera = GameObject.Find("MainCamera").transform;
    }
    public void SetHealth(float health, float maxHealth)
    {
        slider.SetActive(health < maxHealth);
        healthBar.fillAmount = (float)health / maxHealth;  
    }
    private void Update()
    {
        transform.LookAt(transform.position + camera.rotation * Vector3.back + camera.rotation * Vector3.up);
    }
    private void FixedUpdate()
    {
        healthAni.fillAmount = Mathf.Lerp(healthAni.fillAmount, healthBar.fillAmount, Time.fixedDeltaTime );
    }
}
