using UnityEngine;
public class RadarSweepController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource radarAudio;

    [Header("Assign")]
    public Renderer radarSurfaceRenderer;   // plane renderer using SurfaceRadar material
    public Transform radarOrigin;           // player or empty object at scan center

    [Header("Sweep settings (must match material intent)")]
    public float radarRadius = 25f;
    public float sweepWidth = 1f;
    public float sweepSpeed = 6f;

    MaterialPropertyBlock _mpb;

    float lastSweep = 0f;

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (!radarSurfaceRenderer || !radarOrigin) return;

        radarSurfaceRenderer.GetPropertyBlock(_mpb);
        _mpb.SetVector("_RadarOrigin", radarOrigin.position);
        _mpb.SetFloat("_RadarRadius", radarRadius);
        _mpb.SetFloat("_SweepWidth", sweepWidth);
        _mpb.SetFloat("_SweepSpeed", sweepSpeed);
        radarSurfaceRenderer.SetPropertyBlock(_mpb);

        // --- Radar sound sync ---
        float sweep = Mathf.Repeat(Time.time * sweepSpeed, Mathf.Max(radarRadius, 0.0001f));

        // If sweep wrapped around, play sound
        if (sweep < lastSweep)
        {
            if (radarAudio)
                radarAudio.Play();
        }

        lastSweep = sweep;
    }

    // Expose current sweep distance (world units) so we can trigger category highlight
    public float CurrentSweepDistance()
    {
        float t = Time.time;
        return Mathf.Repeat(t * sweepSpeed, Mathf.Max(radarRadius, 0.0001f));
    }
}

