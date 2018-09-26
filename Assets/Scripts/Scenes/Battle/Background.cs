using System.Collections.Generic;
using UnityEngine;

namespace SF {
    public class Background : MonoBehaviour {
        public enum Type {
            Color, Gradient, HorizontalGradient
        };

        [System.Serializable]
        public class Preset { // TODO: Do this with ScriptableObject?
            public string name;
            public Color main;
            public Color off1;
            public Color off2;
            public Type type;
        }

        public List<Preset> presets;

        [Header("Dependencies")]
        public MissionVariable missionToLoad;

        [Header("Material static links")]
        public Material gradientMaterial;
        public Material horizontalGradientMaterial;

        private void OnDestroy() {
            RenderSettings.skybox = null;
        }

        public void LoadMission() {
            if (!string.IsNullOrEmpty(missionToLoad.value.background.preset)) {
                Preset preset = presets.Find(x => x.name == missionToLoad.value.background.preset);

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

            switch (missionToLoad.value.background.type) {
                case Type.Color:
                    RenderSettings.skybox = gradientMaterial;
                    RenderSettings.skybox.SetVector("_Color1", missionToLoad.value.background.mainColor);
                    RenderSettings.skybox.SetVector("_Color2", missionToLoad.value.background.mainColor);
                    break;
                case Type.Gradient:
                    RenderSettings.skybox = gradientMaterial;
                    RenderSettings.skybox.SetVector("_Color1", missionToLoad.value.background.mainColor);
                    RenderSettings.skybox.SetVector("_Color2", missionToLoad.value.background.off1Color);
                    break;
                case Type.HorizontalGradient:
                    RenderSettings.skybox = horizontalGradientMaterial;
                    RenderSettings.skybox.SetVector("_Color1", missionToLoad.value.background.mainColor);
                    RenderSettings.skybox.SetVector("_Color2", missionToLoad.value.background.off1Color);
                    RenderSettings.skybox.SetVector("_Color2", missionToLoad.value.background.off2Color);
                    break;
            }
        }
    }
}
