# Enemy

## How to create a new enemy

- Create a new script with your monster's name
- Create a new prefab with this script attached in Resources/Monsters
- Add the main script components, as well as AI, Side (set it to **Enemy**)
- Create a new prefab in Prefabs/Scenes/Battle/Monsters: it will be your art prefab
- Add an animator, polygon colliders and so on to this prefab
- Then plug it to the BoardCharacter EnemyOrNeutralSpritePrefab field of your Resources/Monsters/mymonster.prefab