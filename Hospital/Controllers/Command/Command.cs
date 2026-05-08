namespace Hospital.Controllers.Command;

public interface ICommand<T> {
    Task<T>  ExecuteAsync();
}