﻿using UnityEngine;

public class SFSpritePicker : MonoBehaviour {
    public int selectedIndex = -1;
    public int selectedTileset = 0;
    public GameObject[] tileset;

    public GameObject waterPrefab;
    public Color waterColor = new Color(0.52f, 0.82f, 1f, 0.56f);
    public Color underwaterColor = new Color(0.7f, 0.78f, 1f, 1f);
    public int waterOffset = 32;
}
