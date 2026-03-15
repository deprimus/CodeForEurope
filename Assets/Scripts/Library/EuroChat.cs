using UnityEngine;
using TMPro;

public class EuroChat : MonoBehaviour
{
    public static EuroChat Instance { get; private set; }

    [Header("References")]
    public RectTransform content;    // Scroll > Content
    public GameObject postPrefab;    // Post prefab
    public GameObject commentPrefab; // Comment prefab

    private void Awake()
    {
        Instance = this;
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

        var title = go.transform.Find("Header/Text/Title");
        if (title != null)
            title.GetComponent<TMP_Text>().text = post.author.name;

        var body = go.transform.Find("Body/Text");
        if (body != null)
            body.GetComponent<TMP_Text>().text = post.content;

        var image = go.transform.Find("Body/Image");
        if (image != null)
            image.gameObject.SetActive(!string.IsNullOrEmpty(post.imagePath));

        var commentsParent = go.transform.Find("Body/Comments");
        if (commentsParent != null)
        {
            // Clear placeholder comments
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
                    upvotes.GetComponent<TMP_Text>().text = comment.reaction.likes.ToString();

                var downvotes = reaction.Find("Downvotes/Text");
                if (downvotes != null)
                    downvotes.GetComponent<TMP_Text>().text = comment.reaction.dislikes.ToString();
            }
        }
    }
}
