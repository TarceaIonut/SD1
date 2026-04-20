namespace Hospital.Models;


public enum EventType {
    CREATE,
    DELETE,
    UPDATE,
}
public class EventBroker {
    private static readonly Lazy<EventBroker> _eventBroker = new(() => new EventBroker());
    public static EventBroker Instance => _eventBroker.Value;
    private EventBroker() {} 
    public event Action<EventType, string, string, string>  OnResourceReceived;
    public void NewEvent_CRUD(EventType eventType, string resource, string email, string username) {
        OnResourceReceived.Invoke(eventType, resource, email, username);
    }
}