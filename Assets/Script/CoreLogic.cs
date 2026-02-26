using Unity.UI.Shaders.Sample;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class CoreLogic : MonoBehaviour
{

    [Header("Speed Check")]
    public float speedThreshold = 1000f;
    private bool hasReachedTopSpeed = false;

    [Header("Fuel Limit&Time")]
    [SerializeField] private float fuelLimit = 10000f;
    public float currentFuel = 0f;
    public float currentSpeed = 0f;

    [Header("UI Elements")]
    [Header("UI Elements")]
    public Slider speedSlider;
    public Slider fuelSlider;
    private Rigidbody rb;


    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private GameObject Portal_prefab;
    public float spawnDistance = 50f; // How far ahead to spawn
    private bool portalSpawned = false;
    private float TimeMeasure = 0f; 
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentFuel = fuelLimit;
        fuelSlider.maxValue = fuelLimit;
        fuelSlider.value = currentFuel;

        speedSlider.maxValue = speedThreshold;
        speedSlider.value = 0;
    }
    private void FixedUpdate()
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        speedSlider.value = currentSpeed;
        TimeMeasure += Time.deltaTime;
        TimeText.text = "Time:"+TimeMeasure.ToString();
        if (currentSpeed >= speedThreshold && !portalSpawned)
        {
            SpawnPortal();
        }
       
       
    }

    void SpawnPortal()
    {
        portalSpawned = true;
        InfoText.text = "Portal Stabilized! Enter to Escape!";

        // Calculate position: Player current position + (Player's forward direction * distance)
        Vector3 spawnPos = transform.position + (transform.forward * spawnDistance);

        // Spawn the portal
        Instantiate(Portal_prefab, spawnPos, Quaternion.identity);

        Debug.Log("Portal Spawned at: " + spawnPos);
    }

    public void FuelCosumption(float amount)
    {
        currentFuel -= amount;
        fuelSlider.value = currentFuel;
        //fuelSlider.value = currentFuel;
        if (currentFuel <=0f)
        {
            Debug.Log("Fuel Limit Reached!");
            
        }
    }


}
