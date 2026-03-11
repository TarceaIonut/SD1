namespace Hospital.Models;

public class Pair<TF,TS> {
    public TF First;
    public TS Second;
    public Pair(TF first, TS second) {
        this.First = first;
        this.Second = second;
    }
}