using UnityEngine;

public class AlignmentChart : MonoBehaviour
{
    [SerializeField]
    RectTransform blip;

    [SerializeField]
    Vector2 chartSize;

    public Vector2 UpdateBlipPosition(float economicScore, float socialScore) {
        var anchoredEconomic = AnchorToThirds(economicScore);
        var anchoredSocial = AnchorToThirds(socialScore);

        if (anchoredEconomic > 10f) {
            Debug.LogError($"Economic score is too high: {economicScore}");
        }
        if (anchoredEconomic < -10f) {
            Debug.LogError($"Economic score is too low: {economicScore}");
        }
        if (anchoredSocial > 10f) {
            Debug.LogError($"Social score is too high: {socialScore}");
        }
        if (anchoredSocial < -10f) {
            Debug.LogError($"Social score is too low: {socialScore}");
        }

        anchoredEconomic = Mathf.Clamp(anchoredEconomic, -10f, 10f);
        anchoredSocial = Mathf.Clamp(anchoredSocial, -10f, 10f);

        blip.anchoredPosition = new Vector2(
            anchoredEconomic / 10f * chartSize.x,
            anchoredSocial / 10f * chartSize.y
        );

        return new Vector2(anchoredEconomic, anchoredSocial);
    }

    float AnchorToThirds(float value) {
        var third = 10f / 3f;
        if (value >= -0.01f && value <= 0.01f) {
            return 0f;
        }
        if (value > 0) {
            if (value <= third) {
                return third;
            }
            if (value <= 2f * third) {
                return 2f * third;
            }
            return 10f;
        }
        if (value >= -third) {
            return -third;
        }
        if (value >= -2f * third) {
            return -2f * third;
        }
        return -10f;
    }
}
