using System.Diagnostics;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace Hospital.Models.HelperStructures;

public enum Formats {
    Json,
    Xml,
    Csv
}

public interface IExport {
    byte[] ExportData(List<DoctorCheckups> doctorCheckups);
    byte[] ExportData(List<Person> accounts );
    string extention { get;  }
    string ContentType { get; }
}

public class CsvExportStrategy : IExport {
    public string extention => ".csv";
    public string ContentType => "text/csv";
    public byte[] ExportData(List<DoctorCheckups> doctorCheckups) {
        var builder = new System.Text.StringBuilder();
        builder.AppendLine("DoctorName, DoctorEmail, PatientName, PatientEmail, Description, AppointmentDate");
        foreach (var v in doctorCheckups) {
            builder.AppendLine($"{v.Doctor.Account.Username}, {v.Doctor.Email}, {v.Patient.Account.Username}, {v.Patient.Email}, {v.Description}, {v.AppointmentDate}");
        }
        return System.Text.Encoding.UTF8.GetBytes(builder.ToString());
    }
    public byte[] ExportData(List<Person> accounts) {
        var builder = new System.Text.StringBuilder();
        builder.AppendLine("UserName, Email, Role, Specialty");
        foreach (var v in accounts) {
            string speciality = "";
            if (v is Doctor d) {
                speciality = d.Specialty;
            }
            builder.AppendLine($"{v.Account.Username}, {v.Email}, {v.Role.ToString()}, {speciality}");
        }
        return System.Text.Encoding.UTF8.GetBytes(builder.ToString());
    }
}
public class JsonExportStrategy : IExport {
    public string extention => ".json";
    public string ContentType => "application/json";
    public byte[] ExportData(List<DoctorCheckups> doctorCheckups) {
        var options = new JsonSerializerOptions { 
            WriteIndented = true 
        };
        var exportData = doctorCheckups.Select(v => new {
            DoctorName = v.Doctor.Account.Username,
            DoctorEmail = v.Doctor.Email,
            PatientName = v.Patient.Account.Username,
            PatientEmail = v.Patient.Email,
            Description = v.Description,
            Date = v.AppointmentDate.ToShortDateString()
        });
        return JsonSerializer.SerializeToUtf8Bytes(exportData, options);
    }
    public byte[] ExportData(List<Person> accounts) {
        var options = new JsonSerializerOptions { 
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        var exportData = accounts.Select(v =>new {
            UserName = v.Account.Username,
            Email = v.Email,
            Role = v.Role.ToString(),
            Speciality = (v as Doctor)?.Specialty
        });
        return JsonSerializer.SerializeToUtf8Bytes(exportData, options);
    }
}

public class XmlExportStrategy : IExport {
    public string extention => ".xml";
    public string ContentType => "application/xml";
    public byte[] ExportData(List<DoctorCheckups> doctorCheckups) {
        var doc = new XDocument(
            new XElement("DoctorCheckups",
                    doctorCheckups.Select(v => new XElement("Checkup", 
                            new XElement("DoctorName", v.Doctor.Account.Username),
                            new XElement("DoctorEmail", v.Doctor.Email),
                            new XElement("PatientName", v.Patient.Account.Username),
                            new XElement("PatientEmail", v.Patient.Email),
                            new XElement("Description", v.Description),
                            new XElement("AppointmentDate", v.AppointmentDate.ToShortDateString())
                        ))
                )
            );
        var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }

    public byte[] ExportData(List<Person> accounts) {
        var doc = new XDocument(
            new XElement("accounts",
                accounts.Select(v => {
                    var s = (v as Doctor)?.Specialty;
                    return new XElement("account",
                        new XElement("UserName", v.Account.Username),
                        new XElement("Email", v.Email),
                        new XElement("Role", v.Role),
                        s == null ? null : new XElement("Specialty", s)
                    );
                } )
            )
        );
        var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }
}

public static class ExportHelper {
    public static IExport GetStrategy(Formats f) {
        return f switch {
            Formats.Xml => new XmlExportStrategy(),
            Formats.Json => new JsonExportStrategy(),
            Formats.Csv => new CsvExportStrategy()
        };
    }
} 
