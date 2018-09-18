using System.Collections.Generic;

public class Player {
    public string playerName;

    public List<Character> characters = new List<Character>();

    public Player() {
        playerName = Globals.DefaultPlayerName;

        // TODO [ALPHA] Use scenario to load default characters
        Character ritz = new Character(playerName);
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
