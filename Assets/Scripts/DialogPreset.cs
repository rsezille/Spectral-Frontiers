using TMPro;
using UnityEngine;

public class DialogPreset : MonoBehaviour {
    [Header("Direct references")]
    public TextMeshProUGUI textMesh;
    public GameObject nextCursor;
    public GameObject endCursor;

    private void Awake() {
        textMesh.richText = true;
    }
}
