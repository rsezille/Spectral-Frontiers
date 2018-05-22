using System.Collections.Generic;

public class Player {
    public string playerName;
    public int gold;

    public List<Character> ownedChars = new List<Character>();

    public Player() {
        playerName = Globals.DefaultPlayerName;
        gold = 0;

        //TODO
        Character ritz = new Character("Ritz");
        Character mew = new Character("Mew");
        Character kupo = new Character("Kupo");

        ownedChars.Add(ritz);//TODO
        ritz.SetCurrentHP(10);
        ownedChars.Add(mew);//TODO
        mew.SetCurrentHP(20);
        ownedChars.Add(kupo);//TODO
        kupo.SetCurrentHP(30);
    }
}
