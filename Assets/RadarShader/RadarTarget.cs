using UnityEngine;

public class RadarTarget : MonoBehaviour
{
    public enum Category { Enemy, Objective, Loot, Neutral }

    public Category category = Category.Neutral;

    public Color highlightColor = Color.yellow;
    public float highlightDuration = 0.25f;

    public string colorProperty = "_BaseColor";

    Renderer _r;
    MaterialPropertyBlock _mpb;
    Color _original;
    float _timer;

    void Awake()
    {
        _r = GetComponentInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();

        if (_r && _r.sharedMaterial && _r.sharedMaterial.HasProperty(colorProperty))
            _original = _r.sharedMaterial.GetColor(colorProperty);
        else
            _original = Color.white;
    }

    public void TriggerHighlight()
    {
        _timer = highlightDuration;
    }

    void Update()
    {
        if (!_r) return;

        _r.GetPropertyBlock(_mpb);

        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            float k = Mathf.Clamp01(_timer / Mathf.Max(highlightDuration, 0.0001f));

            // Quick flash and then fade
            Color c = Color.Lerp(_original, highlightColor, 1f - k);
            _mpb.SetColor(colorProperty, c);
        }
        else
        {
            _mpb.SetColor(colorProperty, _original);
        }

        _r.SetPropertyBlock(_mpb);
    }
}
