using Hospital.Models;

namespace Hospital.Service;

public class NotificationService {
    public NotificationService() {
        EventBroker.Instance.OnResourceReceived += CRUD_EventNotify;
    }
    private void CRUD_EventNotify(EventType type, string info) {
        Console.WriteLine(type.ToString() + ":\n" + info);
    }
}