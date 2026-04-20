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
    public event Action<EventType, string>  OnResourceReceived;
    public void NewEvent_CRUD(EventType eventType, string resource) {
        OnResourceReceived.Invoke(eventType, resource);
    }
}