using System.Collections.Generic;
using UnityEngine;

public class RadarSweepHighlighter : MonoBehaviour
{
    public RadarSweepController radar;
    public Transform radarOrigin;

    [Header("Hit band")]
    public float hitTolerance = 0.5f; // how close to the band counts as a hit

    [Header("Which categories get highlighted")]
    public List<RadarTarget.Category> highlightCategories = new List<RadarTarget.Category>
    {
        RadarTarget.Category.Enemy,
        RadarTarget.Category.Objective
    };

    RadarTarget[] _targets;
    Dictionary<RadarTarget, float> _cooldowns = new();

    [Header("Cooldown per target to avoid retrigger spam")]
    public float retriggerCooldown = 0.4f;

    void Start()
    {
        _targets = FindObjectsOfType<RadarTarget>(true);
    }

    void Update()
    {
        if (!radar || !radarOrigin) return;

        float sweepDist = radar.CurrentSweepDistance();
        Vector3 o = radarOrigin.position;

        foreach (var t in _targets)
        {
            if (!t) continue;
            if (!highlightCategories.Contains(t.category)) continue;

            float cd = _cooldowns.TryGetValue(t, out var v) ? v : 0f;
            if (cd > 0f)
            {
                _cooldowns[t] = cd - Time.deltaTime;
                continue;
            }

            Vector3 p = t.transform.position;

            // Compare XZ distance to sweep ring
            float dist = Vector2.Distance(new Vector2(p.x, p.z), new Vector2(o.x, o.z));

            if (Mathf.Abs(dist - sweepDist) <= hitTolerance)
            {
                t.TriggerHighlight();
                _cooldowns[t] = retriggerCooldown;
            }
        }
    }
}
