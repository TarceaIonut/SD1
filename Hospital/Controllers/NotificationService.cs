using Hospital.Models;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace Hospital.Service;

public class NotificationService {
    public NotificationService() {
        EventBroker.Instance.OnResourceReceived += CRUD_EventNotify;
        EventBroker.Instance.OnResourceReceived += send_email;
    }
    private void CRUD_EventNotify(EventType type, string info, string email, string username) {
        Console.WriteLine(type.ToString() + ":\n" + info);
        Console.WriteLine("email: " + email);
        Console.WriteLine("username: " + username);
    }

    private void send_email(EventType type, string info, string email, string username) {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", "tudor.ionut.tarcea@gmail.com"));
        message.To.Add(new MailboxAddress(username, email));
        message.Subject = type.ToString();

        message.Body = new TextPart("plain") {
            Text = info
        };

        using (var client = new SmtpClient()) {
            try {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("tudor.ionut.tarcea@gmail.com", "civl bgyw mrto azlt");
                client.Send(message);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            finally {
                client.Disconnect(true);
            }
        }
    }
}