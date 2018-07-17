using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogPreset : MonoBehaviour, IPointerClickHandler {
    [Header("Direct references")]
    public TextMeshProUGUI textMesh;
    public GameObject nextCursor;
    public GameObject endCursor;

    private DialogBox dialogBox;

    private void Awake() {
        dialogBox = GetComponentInParent<DialogBox>();

        textMesh.richText = true;
    }

    public void OnPointerClick(PointerEventData eventData) {
        dialogBox.Next();
    }
}
