using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveImageOnClick : MonoBehaviour
{
    [Header("References")]
    public RectTransform target;
    public Image background;

    [Header("Sprites")]
    public Sprite spriteOn;
    public Sprite spriteOff;

    [Header("Settings")]
    public float moveDistance = 40f;
    public float moveDuration = 0.3f;

    private bool isOn = true;
    private bool isMoving = false;

    void Start()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        target.anchoredPosition = new Vector2(moveDistance, target.anchoredPosition.y);

        if (background != null && spriteOn != null)
            background.sprite = spriteOn;
    }

    public void OnClickMove()
    {
        if (isMoving) return;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        isMoving = true;
        float elapsed = 0f;

        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos;
        endPos.x = isOn ? -moveDistance : moveDistance;

        Sprite targetSprite = isOn ? spriteOff : spriteOn;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveDuration);
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        target.anchoredPosition = endPos;

        if (background != null)
            background.sprite = targetSprite;

        isOn = !isOn;
        isMoving = false;
    }

    public void ApplyBGMSettings()
    {
        PlayerPrefs.SetInt("BGM_Enabled", isOn ? 0 : 1);
        PlayerPrefs.Save();

        if (isOn)
            AudioManager.Instance?.OnBGMToggleChanged(false);
        else
            AudioManager.Instance?.OnBGMToggleChanged(true);
    }

    public void ApplySFXSettings()
    {
        PlayerPrefs.SetInt("BGM_Enabled", isOn ? 0 : 1);
        PlayerPrefs.Save();

        if (isOn)
            AudioManager.Instance?.OnSFXToggleChanged(false);
        else
            AudioManager.Instance?.OnSFXToggleChanged(true);
    }
}
