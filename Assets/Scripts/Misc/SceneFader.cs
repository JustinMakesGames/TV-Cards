using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public const float DefaultDuration = 0.5f;

    private static SceneFader _instance;

    private Canvas _canvas;
    private Image _image;
    private bool _isFading;

    public static bool IsFading => _instance && _instance._isFading;

    public static IEnumerator FadeToBlack(float duration = DefaultDuration)
    {
        SceneFader fader = GetInstance();
        fader._isFading = true;
        fader._canvas.enabled = true;
        yield return fader.Fade(1, duration);
    }

    public static IEnumerator FadeFromBlack(float duration = DefaultDuration)
    {
        SceneFader fader = GetInstance();
        yield return fader.Fade(0, duration);
        fader._canvas.enabled = false;
        fader._isFading = false;
    }

    public static IEnumerator WaitUntilVisible()
    {
        while (IsFading) yield return null;
    }

    public static void LoadScene(string sceneName, float duration = DefaultDuration)
    {
        if (IsFading) return;

        SceneFader fader = GetInstance();
        fader.StartCoroutine(fader.LoadSceneRoutine(sceneName, duration));
    }

    public static bool IsFader(GameObject other)
    {
        return _instance && _instance.gameObject == other;
    }

    private static SceneFader GetInstance()
    {
        if (!_instance)
        {
            GameObject faderObject = new GameObject("SceneFader");
            DontDestroyOnLoad(faderObject);
            _instance = faderObject.AddComponent<SceneFader>();
            _instance.CreateUI();
        }

        return _instance;
    }

    private void CreateUI()
    {
        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = short.MaxValue;
        _canvas.enabled = false;

        gameObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("Black", typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(transform, false);

        RectTransform rect = (RectTransform)imageObject.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        _image = imageObject.GetComponent<Image>();
        _image.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator LoadSceneRoutine(string sceneName, float duration)
    {
        yield return FadeToBlack(duration);
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return FadeFromBlack(duration);
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = _image.color.a;
        float time = 0;

        while (time < duration)
        {
            yield return null;

            time += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            SetAlpha(Mathf.SmoothStep(startAlpha, targetAlpha, time / duration));
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        _image.color = new Color(0, 0, 0, alpha);
    }
}
