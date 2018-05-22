using UnityEngine;
using System;
using System.Collections;

/**
 * Contain generic coroutines
 **/
public static class GenericCoroutine {
    /**
     * Lerp smoothly a RectTransform from initial position to target position
     */
    public static IEnumerator ToggleItem(RectTransform rt, Vector3 initial, Vector3 target, Action callback = null) {
        rt.anchoredPosition3D = initial;

        while (Vector3.Distance(rt.anchoredPosition3D, target) > 0.1f) {
            rt.anchoredPosition3D = Vector3.Lerp(rt.anchoredPosition3D, target, 0.1f); // or animationSpeed * Time.deltaTime

            yield return null;
        }

        rt.anchoredPosition3D = target;

        if (callback != null)
            callback();
    }

    public static IEnumerator MoveRectTransformSmooth(RectTransform rt, Vector3 initial, Vector3 target, float speed = 0.2f, Action callback = null) {
        rt.anchoredPosition3D = initial;
        Vector3 velocity = Vector3.zero;

        while (Vector3.Distance(rt.anchoredPosition3D, target) > 0.1f) {
            rt.anchoredPosition3D = Vector3.SmoothDamp(rt.anchoredPosition3D, target, ref velocity, speed); // or animationSpeed * Time.deltaTime

            yield return null;
        }

        rt.anchoredPosition3D = target;

        if (callback != null)
            callback();
    }

    public static IEnumerator RotateRectTransform(RectTransform rt, Quaternion target, float speed = 3f, Action callback = null) {
        float factor = 0f;
        Quaternion initial = rt.rotation;

        while (factor < 1f) {
            rt.rotation = Quaternion.Lerp(initial, target, factor);
            factor += Time.deltaTime * speed;

            yield return null;
        }

        rt.rotation = target;

        if (callback != null)
            callback();
    }

    public static IEnumerator RotateRectTransformSmooth(RectTransform rt, Quaternion target, float speed = 10f, Action callback = null) {
        while (Vector3.Distance(rt.rotation.eulerAngles, target.eulerAngles) > 0.1f) {
            rt.rotation = Quaternion.Lerp(rt.rotation, target, speed * Time.deltaTime);

            yield return null;
        }

        rt.rotation = target;

        if (callback != null)
            callback();
    }
}
