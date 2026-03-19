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
