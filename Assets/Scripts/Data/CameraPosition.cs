using DG.Tweening;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/CameraPosition")]
    public class CameraPosition : ScriptableObject {
        private float positionYOffset = 0.7f;
        public Vector3 position = new Vector3(0f, 0f, -10f);
        public bool isMoving = false;

        public Tween SetPosition(BoardCharacter boardCharacter, bool smooth = false, float duration = 1f, Ease ease = Ease.OutCubic) {
            return SetPosition(boardCharacter.GetSquare(), smooth, duration, ease);
        }

        public Tween SetPosition(Square targetedSquare, bool smooth = false, float duration = 1f, Ease ease = Ease.OutCubic) {
            isMoving = true;
            int height = targetedSquare != null ? targetedSquare.Height : 0;

            Vector3 target = new Vector3(
                targetedSquare.x - targetedSquare.y,
                (targetedSquare.y + targetedSquare.x) / 2f + (height / Globals.PixelsPerUnit) + positionYOffset,
                position.z
            );

            if (!smooth) {
                return DOTween.To(() => position, x => position = x, target, 0f).OnComplete(() => isMoving = false);
            } else {
                if (position == target) {
                    return DOTween.To(() => position, x => position = x, target, 0f).OnComplete(() => isMoving = false);
                }

                return DOTween.To(() => position, x => position = x, target, duration).SetEase(ease).OnComplete(() => isMoving = false);
            }
        }

        public bool IsOnSquare(Square square) {
            Vector3 squarePosition = new Vector3(
                square.x - square.y,
                (square.y + square.x) / 2f + (square.Height / Globals.PixelsPerUnit) + positionYOffset,
                position.z
            );

            return position == squarePosition;
        }
    }
}
