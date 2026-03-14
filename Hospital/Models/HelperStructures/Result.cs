namespace Hospital.Models;

public class Result<TV,TE> where TV:class where TE:class {
    public TV? Value;
    public TE? Error;
    private Result(TV? value, TE? error) {
        this.Value = value;
        this.Error = error;
    }
    public bool HasValue() => Value != null;
    public static Result<TV,TE> Success(TV value) => new(value, null);
    public static Result<TV,TE> Error_(TE error) => new(null, error);

    public static Result<TV, TE> ResultIf(TV? value, TE error, bool isCorrect) {
        if (isCorrect) return Success(value!);
        return Error_(error);
    }
}