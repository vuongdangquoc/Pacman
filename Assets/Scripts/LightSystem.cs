using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// For blind mode.
/// </summary>
public class LightSystem : MonoBehaviour
{
    public static LightSystem Instance { get; private set; }
    public new Light2D light;
    public Pacman pacman;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        light = GetComponent<Light2D>();
    }
    private void Update()
    {
        gameObject.transform.position = pacman.transform.position;
        Camera.main.orthographicSize = 10f;

        Vector3 cameraPos = pacman.transform.position;
        cameraPos.z = -10;
        Camera.main.gameObject.transform.position = cameraPos;
    }
    public void ResetLight()
    {
        light.pointLightOuterRadius = 5;
        light.pointLightInnerRadius = 4;
    }
    public void IncreaseLight()
    {
        if (light.pointLightOuterRadius < 20)
        {
            light.pointLightOuterRadius += 2;
            light.pointLightInnerRadius += 2;
        }
    }
    public void SetPacman(Pacman newPacman)
    {
        pacman = newPacman;
    }
}