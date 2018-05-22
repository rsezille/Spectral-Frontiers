using UnityEngine;

public class BattleCamera : MonoBehaviour {
    private Camera cam;

    // Camera speed
    public float speed = 7f;
    private Vector3 velocity = Vector3.zero;

    // Movements
    public bool isAutoMoving = false; // If the camera is already moving (ie. cinematics) - Disable camera controls
    public bool smooth = false; // If the camera goes toward the target by moving or instantly
    public Vector3 target;

    void Awake() {
        cam = GetComponent<Camera>();
    }

    void Update() {
        if (!isAutoMoving) {
            if (Input.GetAxis(InputBinds.Zoom) != 0) {
                cam.orthographicSize = Mathf.Clamp(
                    cam.orthographicSize - Input.GetAxis(InputBinds.Zoom) / 0.5f,
                    0.1f,
                    Screen.height / (Globals.TileHeight * 2f) * 2f
                );
            }

            float tmpSpeed = speed;

            // Lower the speed if the camera is going diagonally
            if (Input.GetAxisRaw(InputBinds.CameraH) != 0 && Input.GetAxisRaw(InputBinds.CameraV) != 0) {
                tmpSpeed = speed / Mathf.Sqrt(2f);
            }

            transform.position += new Vector3(tmpSpeed * Time.deltaTime * Input.GetAxisRaw(InputBinds.CameraH), tmpSpeed * Time.deltaTime * Input.GetAxisRaw(InputBinds.CameraV));
        } else {
            if (smooth) {
                transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.3f);
                //transform.position = Vector3.Lerp(transform.position, target, 0.1f);
            } else {
                transform.position = target;
            }

            if ((transform.position == target || Vector3.Distance(transform.position, target) < 0.1)) {
                transform.position = target;
                isAutoMoving = false;
            }
        }
    }

    public void ResetCameraSize() {
        cam.orthographicSize = Screen.height / (Globals.TileHeight * 2f);
    }
}
