using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnableAnimation : MonoBehaviour
{
    private bool _prevOpened = false;
    public bool opened = false;

    public float EnabledScale = 1.0f;
    public float DisabledScale = 0.0f;

    public float AnimDuration = 1.0f;

    bool animationPlaying = false;

    private Vector3 baseScale;

    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;
        if (!opened)
        {
            transform.localScale = DisabledScale * baseScale;
        }
    }

    private void Update()
    {
        if (!_prevOpened && opened)
        {
            Enable();
        }
        else if (_prevOpened && !opened)
        {
            Disable();
        }
        _prevOpened = opened;
    }

    public void SetEnabled(bool enable)
    {
        opened = enable;
    }

    private void Enable()
    {
        if (animationPlaying)
        {
            return;
        }
        else
        {
            StartCoroutine(OpenItem());
        }
    }

    public bool IsReady()
    {
        return opened && !animationPlaying;
    }

    private void Disable()
    {
        if (animationPlaying)
        {
            return;
        }
        else
        {
            StartCoroutine(CloseItem());
        }
    }

    IEnumerator OpenItem()
    {
        float elapsed = 0.0f;
        animationPlaying = true;
        while (elapsed < AnimDuration)
        {
            float progress = elapsed / AnimDuration;
            float scale = EaseInOutQuad(DisabledScale, EnabledScale, progress);

            transform.localScale = scale * baseScale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = EnabledScale * baseScale;

        animationPlaying = false;

    }

    IEnumerator CloseItem()
    {
        float elapsed = 0.0f;
        animationPlaying = true;
        while (elapsed < AnimDuration)
        {
            float progress = elapsed / AnimDuration;
            float scale = EaseInOutQuad(EnabledScale, DisabledScale, progress);

            transform.localScale = scale * baseScale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = DisabledScale * baseScale;

        animationPlaying = false;

    }

    private static float EaseInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value + start;
        value--;
        return -end * 0.5f * (value * (value - 2) - 1) + start;
    }

    
}