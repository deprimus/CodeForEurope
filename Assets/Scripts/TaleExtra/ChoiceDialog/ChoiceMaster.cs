using UnityEngine;
using TMPro;
using TaleUtil;
using TaleUtil.Scripts.Choice.Dialog;
using System.ComponentModel;

namespace System.Runtime.CompilerServices {
    // Required for records to work in Unity
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

public static partial class TaleExtra {
    public static partial class Choice {
        public static TaleUtil.Action Dialog(Args title, params ChoiceItem[] choices) =>
            Tale.Choice.Style("dialog", title, choices);
    }
}

namespace TaleUtil.Scripts.Choice.Dialog {
    // Since Unity doesn't support C# 10+, we have to use records.
    // When C# 10 is available, replace these with "global using"
    public record Args(string title) {
        public static implicit operator Args(string title) =>
            new Args(title);
    }
    public record ChoiceItem(string label, Sprite img, Delegates.ShallowDelegate callback) {
        public static implicit operator ChoiceItem((string, Sprite, Delegates.ShallowDelegate) args) =>
            new ChoiceItem(args.Item1, args.Item2, args.Item3);
    }

    public class ChoiceMaster : ChoiceMaster<Args, ChoiceItem>
    {
        enum State {
            IDLE,
            TRANSITION_IN,
            WAIT_FOR_INPUT,
            TRANSITION_OUT
        }

        State state;

        [SerializeField]
        internal TextMeshProUGUI title;
        [SerializeField]
        internal ChoiceObj[] choiceObjs;

        Animator animator;

        Args args;
        ChoiceItem[] choices;
        Delegates.ShallowDelegate onEnd;

        void Awake() {
            animator = GetComponent<Animator>();
        }

        public override void Present(Args args, ChoiceItem[] choices, Delegates.ShallowDelegate onEnd)
        {
            if (choices == null || choices.Length == 0) {
                Log.Warning("No choices passed to dialog style choice picker");
                onEnd();
                return;
            }

            if (choices.Length > choiceObjs.Length) {
                Log.Error("CHOICE", string.Format("Dialog style choice picker supports a maximum of {0} choices, received {1} choices", choiceObjs.Length, choices.Length));
                onEnd();
                return;
            }

            this.args = args;
            this.choices = choices;
            this.onEnd = onEnd;

            state = State.TRANSITION_IN;
            animator.SetTrigger("TransitionIn");

            ShowChoices();
        }

        public bool IsChoosing() {
            return state == State.WAIT_FOR_INPUT;
        }

        void Update() {
            switch (state) {
                case State.TRANSITION_IN: {
                    if (animator.StateFinished("ChoiceDialogIn")) {
                        state = State.WAIT_FOR_INPUT;
                        animator.SetTrigger("Idle");
                    }

                    break;
                }
                case State.TRANSITION_OUT: {
                    if (animator.StateFinished("ChoiceDialogOut")) {
                        state = State.IDLE;
                        animator.SetTrigger("Idle");

                        onEnd();
                    }

                    break;
                }
            }
        }

        void ShowChoices() {
            for (int i = 0; i < choiceObjs.Length; i++) {
                var obj = choiceObjs[i];

                if (i < choices.Length) {
                    obj.gameObject.SetActive(true);

                    var callback = choices[i].callback;

                    obj.Present(choices[i].label, choices[i].img, () => {
                        state = State.TRANSITION_OUT;
                        animator.SetTrigger("TransitionOut");

                        if (callback != null) {
                            callback();
                        }
                    });
                } else {
                    obj.gameObject.SetActive(false);
                }
            }

            title.text = args.title;
        }
    }
}