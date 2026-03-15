using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EuroChat : MonoBehaviour
{
    public static EuroChat Instance { get; private set; }

    [Header("References")]
    public RectTransform content;
    public GameObject postPrefab;
    public GameObject commentPrefab;

    private Sprite[] _catAvatars;

    private static readonly Color ColorGreen       = new Color(0.26f, 0.80f, 0.26f);
    private static readonly Color ColorLiberal      = new Color(1.00f, 0.84f, 0.00f);
    private static readonly Color ColorTraditional  = new Color(0.20f, 0.47f, 0.93f);
    private static readonly Color ColorProgressist  = new Color(0.98f, 0.26f, 0.26f);
    private static readonly Color ColorNeutral      = new Color(0.60f, 0.60f, 0.60f);

    private void Awake()
    {
        Instance = this;
        _catAvatars = new Sprite[50];
        for (int i = 0; i < 50; i++)
        {
            _catAvatars[i] = Resources.Load<Sprite>($"Avatars/cat_{i:D2}");
        }
        gameObject.SetActive(false);
    }

    public void Show()
    {
        ClearContent();

        var law = GameManager.Instance.CurrentLaw;
        if (law == null) return;

        var posts = GameDatabase.Instance.GetPostsForLaw(law.Name);

        foreach (var post in posts)
            SpawnPost(post);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ClearContent()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);
    }

    private void SpawnPost(PostJson post)
    {
        var go = Instantiate(postPrefab, content);

        // Avatar
        var avatar = go.transform.Find("Header/Avatar");
        if (avatar != null)
            SetAvatar(avatar, post.faction);

        var title = go.transform.Find("Header/Text/Title");
        if (title != null)
            title.GetComponent<TMP_Text>().text = post.author.name;

        var body = go.transform.Find("Body/Text");
        if (body != null)
            body.GetComponent<TMP_Text>().text = post.content;

        var image = go.transform.Find("Body/Image");
        if (image != null)
            image.gameObject.SetActive(!string.IsNullOrEmpty(post.imagePath));

        var reaction = go.transform.Find("Body/Reaction");
        if (reaction != null)
            SetRandomReaction(reaction);

        var commentsParent = go.transform.Find("Body/Comments");
        if (commentsParent != null)
        {
            for (int i = commentsParent.childCount - 1; i >= 0; i--)
                Destroy(commentsParent.GetChild(i).gameObject);

            if (post.comments != null)
            {
                foreach (var comment in post.comments)
                    SpawnComment(comment, commentsParent);
            }
        }
    }

    private void SpawnComment(CommentJson comment, Transform parent)
    {
        var go = Instantiate(commentPrefab, parent);

        // Comment avatar - random cat, neutral color
        var avatar = go.transform.Find("Avatar");
        if (avatar != null)
            SetAvatar(avatar, null);

        var title = go.transform.Find("Content/Content/Title");
        if (title != null)
            title.GetComponent<TMP_Text>().text = comment.author.name;

        var body = go.transform.Find("Content/Content/Date");
        if (body != null)
            body.GetComponent<TMP_Text>().text = comment.content;

        if (comment.reaction != null)
        {
            var reaction = go.transform.Find("Content/Reaction");

            if (reaction != null)
            {
                var upvotes = reaction.Find("Upvotes/Text");
                if (upvotes != null)
                    upvotes.GetComponent<TMP_Text>().text = FormatCount(comment.reaction.likes);

                var downvotes = reaction.Find("Downvotes/Text");
                if (downvotes != null)
                    downvotes.GetComponent<TMP_Text>().text = FormatCount(comment.reaction.dislikes);
            }
        }
    }

    private void SetAvatar(Transform avatar, string faction)
    {
        var img = avatar.GetComponent<Image>();
        if (img == null) return;

        if (_catAvatars != null && _catAvatars.Length > 0)
            img.sprite = _catAvatars[Random.Range(0, _catAvatars.Length)];

        img.color = Color.white;

        // Faction status dot
        if (!string.IsNullOrEmpty(faction))
        {
            var dot = new GameObject("FactionDot", typeof(RectTransform), typeof(Image));
            dot.transform.SetParent(avatar, false);
            var rt = dot.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(1, 0);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(12, 12);
            dot.GetComponent<Image>().color = GetFactionColor(faction);
        }
    }

    private static Color GetFactionColor(string faction)
    {
        if (string.IsNullOrEmpty(faction)) return Color.white;

        return faction.ToUpper() switch
        {
            "GREEN"        => ColorGreen,
            "LIBERAL"      => ColorLiberal,
            "TRADITIONAL"  => ColorTraditional,
            "PROGRESSIST"  => ColorProgressist,
            "NEUTRU"       => ColorNeutral,
            _              => Color.white
        };
    }

    private void SetRandomReaction(Transform reaction)
    {
        int likes = Random.Range(450, 2501);
        int dislikes = Mathf.RoundToInt(likes * Random.Range(0.15f, 0.20f));

        var upvotes = reaction.Find("Upvotes/Text");
        if (upvotes != null)
            upvotes.GetComponent<TMP_Text>().text = FormatCount(likes);

        var downvotes = reaction.Find("Downvotes/Text");
        if (downvotes != null)
            downvotes.GetComponent<TMP_Text>().text = FormatCount(dislikes);
    }

    private static string FormatCount(int n)
    {
        if (n >= 1000)
            return (n / 1000f).ToString("0.#") + "k";
        return n.ToString();
    }
}
