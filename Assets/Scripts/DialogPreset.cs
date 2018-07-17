using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogPreset : MonoBehaviour, IPointerClickHandler {
    [Header("Direct references")]
    public TextMeshProUGUI textMesh;
    public Animator cursor;
    public Canvas canvas;

    private DialogBox dialogBox;

    private void Awake() {
        dialogBox = GetComponentInParent<DialogBox>();

        textMesh.richText = true;

        dialogBox.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }

    public void OnPointerClick(PointerEventData eventData) {
        dialogBox.Next();
    }

    public void NextCursor() {
        cursor.Play("NextCursor");
    }

    public void EndCursor() {
        cursor.Play("EndCursor");
    }

    public void NoCursor() {
        cursor.Play("NoCursor");
    }
}
