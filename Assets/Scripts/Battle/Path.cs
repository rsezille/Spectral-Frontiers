using System.Collections.Generic;

public class Path {
    public List<Square> steps = new List<Square>();

    public int GetLength() {
        return steps.Count;
    }

    public Square GetStep(int index) {
        return steps[index];
    }

    public void AppendStep(Square square) {
        steps.Add(square);
    }

    public void PrependStep(Square square) {
        steps.Insert(0, square);
    }
}
