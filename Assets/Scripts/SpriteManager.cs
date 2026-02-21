using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public Sprite emoteHeart;
    public Sprite emoteAngry;

    public static SpriteManager instance;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
