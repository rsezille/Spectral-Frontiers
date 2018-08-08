using System.Collections.Generic;

public class Player {
    public string playerName;
    public int gold;

    public List<Character> characters = new List<Character>();

    public Player() {
        playerName = Globals.DefaultPlayerName;
        gold = 0;

        // TODO [ALPHA] Use scenario to load default characters
        Character ritz = new Character("Hero");
        Character mew = new Character("Mew");
        Character kupo = new Character("Mewtwo");

        characters.Add(ritz);
        ritz.SetCurrentHP(10);
        characters.Add(mew);
        mew.SetCurrentHP(20);
        characters.Add(kupo);
        kupo.SetCurrentHP(30);
    }
}
