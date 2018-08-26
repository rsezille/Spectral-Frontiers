using DG.Tweening;
using SF;
using UnityEngine;

public class SunLight : MonoBehaviour {
    private enum Type {
        Day, Night, Turn, Auto
    };

    private Light light;
    private bool turnType = false;
    private float turnTime = 0f;

    public float hardCapIntensity = 1.1f; // When the sun hurts really hard, in the desert during summer
    public float dayIntensity = 0.85f;
    public float nightIntensity = 0.1f;

    private void Awake() {
        light = GetComponent<Light>();
    }

    public float GetIntensity() {
        return light.intensity;
    }

    /**
     * Normalize the current intensity to 0-1 range, depending on dayIntensity and nightIntensity
     * Can be useful for Lerp values
     */
    public float GetNormalizedIntensity() {
        return Mathf.Clamp((GetIntensity() - nightIntensity) / (dayIntensity - nightIntensity), nightIntensity, dayIntensity);
    }

    public void Load(string missionLighting) {
        Type lightingType = EnumUtil.ParseEnum(missionLighting, Type.Day);

        switch (lightingType) {
            case Type.Day:
                light.intensity = dayIntensity;
                break;
            case Type.Night:
                light.intensity = nightIntensity;
                break;
            case Type.Turn:
                light.intensity = dayIntensity;
                turnType = true;
                break;
            case Type.Auto: // Mainly for testing purpose
                light.intensity = nightIntensity;

                float speed = 1f;
                DOTween
                    .Sequence()
                    .Append(light.DOIntensity(dayIntensity, speed).SetEase(Ease.Linear).OnUpdate(BattleManager.instance.EventOnLightChange))
                    .AppendInterval(speed * 4f)
                    .Append(light.DOIntensity(nightIntensity, speed).SetEase(Ease.Linear).OnUpdate(BattleManager.instance.EventOnLightChange))
                    .AppendInterval(speed * 4f)
                    .SetLoops(-1);
                break;
        }

        BattleManager.instance.EventOnLightChange();
    }

    /**
     * If the lighting of the mission is set to turn, turns will modify the intensity
     */
    public void NewTurn() {
        if (turnType) {
            turnTime += 0.2f;

            if (Mathf.Approximately(turnTime, 10f) || turnTime > 10f) {
                turnTime = 0f;
                light.intensity = dayIntensity;
                BattleManager.instance.EventOnLightChange();
            }

            if (turnTime <= 5 && turnTime >= 4) {
                light.intensity = Mathf.Lerp(dayIntensity, nightIntensity, turnTime - 4f);
                BattleManager.instance.EventOnLightChange();
            } else if (turnTime <= 10 && turnTime >= 9) {
                light.intensity = Mathf.Lerp(nightIntensity, dayIntensity, turnTime - 9f);
                BattleManager.instance.EventOnLightChange();
            }
        }
    }
}
