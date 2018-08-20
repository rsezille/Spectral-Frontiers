using SF;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
    public enum Type {
        Color, Gradient, HorizontalGradient
    };

    [System.Serializable]
    public class Preset {
        public string name;
        public Color main;
        public Color off1;
        public Color off2;
        public Type type;
    }

    public List<Preset> presets;

    [Header("Material static links")]
    public Material gradientMaterial;
    public Material horizontalGradientMaterial;

    [Header("Default gradient")]
    public Type defaultType = Type.Gradient;
    public Color defaultMain = new Vector4(0.43f, 0.68f, 0.83f);
    public Color defaultOff1 = new Vector4(0.22f, 0.36f, 0.88f);
    public Color defaultOff2 = new Vector4(1, 1, 1);

    private void OnDestroy() {
        RenderSettings.skybox = null;
    }

    public void Load(RawMission.Background rawBackground) {
        if (!string.IsNullOrEmpty(rawBackground.preset)) {
            Preset preset = presets.Find(x => x.name == rawBackground.preset);

            if (preset != null) {
                switch (preset.type) {
                    case Type.Color:
                        RenderSettings.skybox = gradientMaterial;
                        RenderSettings.skybox.SetVector("_Color1", preset.main);
                        RenderSettings.skybox.SetVector("_Color2", preset.main);
                        break;
                    case Type.Gradient:
                        RenderSettings.skybox = gradientMaterial;
                        RenderSettings.skybox.SetVector("_Color1", preset.main);
                        RenderSettings.skybox.SetVector("_Color2", preset.off1);
                        break;
                    case Type.HorizontalGradient:
                        RenderSettings.skybox = horizontalGradientMaterial;
                        RenderSettings.skybox.SetVector("_Color1", preset.main);
                        RenderSettings.skybox.SetVector("_Color2", preset.off1);
                        RenderSettings.skybox.SetVector("_Color3", preset.off2);
                        break;
                }

                return;
            }
        }

        Color mainColor = defaultMain;
        Color off1Color = defaultOff1;
        Color off2Color = defaultOff2;
        
        if (!string.IsNullOrEmpty(rawBackground.mainColor)) {
            Color mainColorOut;

            if (ColorUtility.TryParseHtmlString(rawBackground.mainColor, out mainColorOut)) {
                mainColor = mainColorOut;
            }
        }

        if (!string.IsNullOrEmpty(rawBackground.off1Color)) {
            Color off1ColorOut;

            if (ColorUtility.TryParseHtmlString(rawBackground.off1Color, out off1ColorOut)) {
                off1Color = off1ColorOut;
            }
        }

        if (!string.IsNullOrEmpty(rawBackground.off2Color)) {
            Color off2ColorOut;

            if (ColorUtility.TryParseHtmlString(rawBackground.off2Color, out off2ColorOut)) {
                off2Color = off2ColorOut;
            }
        }

        switch (EnumUtil.ParseEnum(rawBackground.type, defaultType)) {
            case Type.Color:
                RenderSettings.skybox = gradientMaterial;
                RenderSettings.skybox.SetVector("_Color1", mainColor);
                RenderSettings.skybox.SetVector("_Color2", mainColor);
                break;
            case Type.Gradient:
                RenderSettings.skybox = gradientMaterial;
                RenderSettings.skybox.SetVector("_Color1", mainColor);
                RenderSettings.skybox.SetVector("_Color2", off1Color);
                break;
            case Type.HorizontalGradient:
                RenderSettings.skybox = horizontalGradientMaterial;
                RenderSettings.skybox.SetVector("_Color1", mainColor);
                RenderSettings.skybox.SetVector("_Color2", off1Color);
                RenderSettings.skybox.SetVector("_Color2", off2Color);
                break;
        }
    }
}
