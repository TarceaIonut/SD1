namespace Hospital.Controllers.Command;

public interface ICommand<T> {
    Task<T>  ExecuteAsync();
}
public interface ICommand<T, TG> {
    Task<T>  ExecuteAsync(TG g);
}