// -----------------------------------------------------------------------------
// QuizManager.cs
// Loads political_ideology.csv at runtime, displays questions, and tracks
// economic/social scores based on QuizAnswer (SD, D, A, SA). Logs final scores
// after the last question.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public enum QuizAnswer {
    StronglyDisagree,
    Disagree,
    Agree,
    StronglyAgree
}

public class QuizManager : MonoBehaviour {
    public TextMeshProUGUI title;
    public TextMeshProUGUI questionCount;

    public GameObject phaseInit;
    public GameObject phaseQuiz;
    public GameObject phaseResult;

    public RectTransform blip;

    public Vector2 chart;

    public TextMeshProUGUI resultText;
    public TextMeshProUGUI resultDescription;

    float economic;
    float social;

    struct QuizRow {
        public string Question;
        public int Axis;
        public float SD, D, A, SA;
        public int Direction;
    }

    List<QuizRow> rows;
    int current;
    bool loaded;

    void Awake() {
        Tale.Async(
            Tale.Queue(
                Tale.Advance(),
                Tale.Exec(async () => await Init())
            )
        );
    }

    async Task Init() {
        var path = Application.streamingAssetsPath + "/Data/political_ideology.csv";

        if (!path.StartsWith("http") && !path.StartsWith("jar:")) {
            path = "file://" + path;
        }

        using (var req = UnityWebRequest.Get(path)) {
            var tcs = new TaskCompletionSource<bool>();
            req.SendWebRequest().completed += _ => tcs.TrySetResult(true);
            await tcs.Task;

            if (req.result != UnityWebRequest.Result.Success) {
                Debug.LogError("QuizManager: Failed to load CSV: " + req.error);
                return;
            }
            ParseCsv(req.downloadHandler.text);
        }

        loaded = true;
        economic = 0f;
        social = 0f;
        current = 0;

        phaseInit.SetActive(false);
        phaseQuiz.SetActive(true);

        ShowCurrentQuestion();
    }

    void ParseCsv(string raw) {
        rows = new List<QuizRow>();
        var lines = raw.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var culture = CultureInfo.InvariantCulture;

        for (int i = 1; i < lines.Length; i++) {
            var line = lines[i];
            var fields = ParseCsvLine(line);

            if (fields.Count < 7) {
                continue;
            }

            rows.Add(new QuizRow {
                Question = fields[0],
                Axis = int.TryParse(fields[1], NumberStyles.Integer, culture, out int ax) ? ax : 1,
                SD = float.TryParse(fields[2], NumberStyles.Float, culture, out float sd) ? sd : 0f,
                D = float.TryParse(fields[3], NumberStyles.Float, culture, out float d) ? d : 0f,
                A = float.TryParse(fields[4], NumberStyles.Float, culture, out float a) ? a : 0f,
                SA = float.TryParse(fields[5], NumberStyles.Float, culture, out float sa) ? sa : 0f,
                Direction = int.TryParse(fields[6], NumberStyles.Integer, culture, out int dir) ? dir : 1
            });
        }
    }

    static List<string> ParseCsvLine(string line) {
        var result = new List<string>();
        int i = 0;

        while (i < line.Length) {
            if (line[i] == '"') {
                i++;

                int start = i;

                while (i < line.Length && line[i] != '"') {
                    i++;
                }

                result.Add(line.Substring(start, i - start));

                if (i < line.Length) {
                    i++;
                }
            } else {
                int start = i;

                while (i < line.Length && line[i] != ',') {
                    i++;
                }

                result.Add(line.Substring(start, i - start).Trim());
            }
            if (i < line.Length && line[i] == ',') {
                i++;
            }
        }

        return result;
    }

    void ShowCurrentQuestion() {
        if (!loaded || rows == null || current >= rows.Count) {
            return;
        }

        if (title != null) {
            title.text = rows[current].Question;
        }

        if (questionCount != null) {
            questionCount.text = $"{current + 1} / {rows.Count}";
        }
    }

    public void Choose(QuizAnswer answer) {
        if (!loaded || rows == null || current >= rows.Count) {
            return;
        }

        QuizRow row = rows[current];
        float score = answer switch {
            QuizAnswer.StronglyDisagree => row.SD,
            QuizAnswer.Disagree => row.D,
            QuizAnswer.Agree => row.A,
            QuizAnswer.StronglyAgree => row.SA,
            _ => 0f
        };
        score *= row.Direction;

        if (row.Axis == 1) {
            economic += score;
        } else {
            social += score;
        }
        
        current++;

        if (current >= rows.Count) {
            Debug.Log($"Economic: {economic}, Social: {social}");

            var anchoredEconomic = AnchorToThirds(economic);
            var anchoredSocial = AnchorToThirds(social);
            blip.anchoredPosition = new Vector2(anchoredEconomic / 10f * chart.x, anchoredSocial / 10f * chart.y);

            var alignment = GetPlayerAlignment();
            var str = "<color=gray>Neutral</color>";
            var description = "Your answers reflect a preference for pragmatism and moderation. Within a democracy, this approach acts as a stabilizing force, prioritizing incremental progress and compromise over ideological polarization or radical shifts.";

            if (alignment.HasValue) {
                switch (alignment.Value) {
                case FactionType.Left:
                    str = "<color=#43FF43>Green</color>";
                    description = "Your answers reflect a strong preference for ecological sustainability. In a democratic system, this perspective advocates for the integration of environmental health into the legislative process, balancing immediate public demands with the long-term survival of natural resources.";
                    break;
                case FactionType.Traditionalist:
                    str = "<color=#FF3443>Traditionalist</color>";
                    description = "Your answers reflect a preference for historical continuity and social stability. Within democratic frameworks, these values often manifest as a \"braking\" mechanism, ensuring that social changes do not erode the established institutions and cultural heritage that underpin the state.";
                    break;
                case FactionType.Right:
                    str = "<color=#FFD800>Progressist</color>";
                    description = "Your answers reflect a strong preference for social reform and egalitarianism. In a democracy, this outlook drives the expansion of civil rights and the restructuring of institutions to better serve marginalized groups and address systemic inequalities.";
                    break;
                case FactionType.Libertarian:
                    str = "<color=#7FC9FF>Liberal</color>";
                    description = "Your answers reflect a preference for individual autonomy and civil liberties. This is a cornerstone of liberal democracy, operating on the principle that the state exists primarily to protect the rights of the individual and ensure equality before the law.";
                    break;
                }
            }

            var third = 10f / 3f;
            var inOuterThird = Mathf.Abs(anchoredEconomic) >= 2f * third || Mathf.Abs(anchoredSocial) >= 2f * third;
            if (!alignment.HasValue || (anchoredEconomic == 0 && anchoredSocial == 0)) {
                resultText.text = "{str} preference";
            } else if (inOuterThird) {
                resultText.text = $"High {str} preference";
            } else {
                resultText.text = $"Moderate {str} preference";
            }
            resultDescription.text = description;

            phaseQuiz.SetActive(false);
            phaseResult.SetActive(true);

            return;
        }

        ShowCurrentQuestion();
    }

    static float AnchorToThirds(float value) {
        var third = 10f / 3f;
        if (value >= -0.01f && value <= 0.01f) {
            return 0f;
        }
        if (value > 0) {
            if (value <= third) return third;
            if (value <= 2f * third) return 2f * third;
            return 10f;
        }
        if (value >= -third) return -third;
        if (value >= -2f * third) return -2f * third;
        return -10f;
    }

    FactionType? GetPlayerAlignment() {
        bool isLeft = economic < 0;
        bool isLibertarian = social < 0;

        if (economic == 0 && social == 0) {
            return null;
        }
        
        if (isLeft && isLibertarian) {
            return FactionType.Left;
        }

        if (isLeft && !isLibertarian) {
            return FactionType.Traditionalist;
        }

        if (!isLeft && !isLibertarian) {
            return FactionType.Libertarian;
        }

        return FactionType.Right;
    }
}
