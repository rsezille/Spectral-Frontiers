using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MouseReactive))]
[ExecuteInEditMode]
public class SpriteManager : MonoBehaviour {
    // Components
    private MouseReactive mouseReactive;

    private BoardCharacter boardCharacter;

    [Header("Animation controls")]
    public Collider2D[] colliders;
    private int _enabledColliderIndex = 0;
    public int enabledColliderIndex = 0;

    private void Awake() {
        mouseReactive = GetComponent<MouseReactive>();
        mouseReactive.MouseEnter = new UnityEvent();
        mouseReactive.MouseEnter.AddListener(MouseEnter);
        mouseReactive.MouseLeave = new UnityEvent();
        mouseReactive.MouseLeave.AddListener(MouseLeave);
        mouseReactive.Click = new UnityEvent();
        mouseReactive.Click.AddListener(Click);



        reflectGo = new GameObject("Water Reflect", typeof(SpriteRenderer));
        reflectGo.transform.parent = transform;
        reflectGo.transform.localPosition = Vector3.zero;
        reflectGo.layer = LayerMask.NameToLayer("Reflectable");

        SpriteRenderer sr = reflectGo.GetComponent<SpriteRenderer>();
        sr.flipY = !GetComponent<SpriteRenderer>().flipY;
        sr.sortingOrder = 10;
    }

    private GameObject reflectGo;

    private void Start() {
        boardCharacter = GetComponentInParent<BoardCharacter>();

        CheckColliders();
    }

    private void Update() {
        if (_enabledColliderIndex != enabledColliderIndex) {
            _enabledColliderIndex = enabledColliderIndex;

            CheckColliders();
        }
    }

    private void LateUpdate() {
        SpriteRenderer sr = reflectGo.GetComponent<SpriteRenderer>();

        sr.flipX = GetComponent<SpriteRenderer>().flipX;
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void CheckColliders() {
        for (int i = 0; i < colliders.Length; i++) {
            if (i == enabledColliderIndex) {
                colliders[i].enabled = true;
            } else {
                colliders[i].enabled = false;
            }
        }
    }

    /**
     * Triggered by Board
     */
    public void MouseEnter() {
        if (boardCharacter != null) {
            boardCharacter.MouseEnter();
        }
    }

    /**
     * Triggered by Board
     */
    public void MouseLeave() {
        if (boardCharacter != null) {
            boardCharacter.MouseLeave();
        }
    }

    /**
     * Triggered by Board
     */
    public void Click() {
        if (boardCharacter != null) {
            boardCharacter.Click();
        }
    }
}
