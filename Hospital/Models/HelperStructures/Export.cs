using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Hospital.Models.HelperStructures;

public enum Formats {
    Json,
    Xml,
    Csv
}

public interface IExport {
    byte[] ExportData(List<DoctorCheckups> doctorCheckups);
    byte[] ExportData(List<Person> accounts);
    string Extention { get; }
    string ContentType { get; }
}

public abstract class ExportTemplate : IExport {
    public abstract string Extention { get; }
    public abstract string ContentType { get; }

    public byte[] ExportData(List<DoctorCheckups> doctorCheckups) {
        var formattedItems = doctorCheckups.Select(FormatCheckup);
        var combinedData = CombineCheckups(formattedItems);
        return GenerateBytes(combinedData);
    }

    public byte[] ExportData(List<Person> accounts) {
        var formattedItems = accounts.Select(FormatPerson);
        var combinedData = CombinePersons(formattedItems);
        return GenerateBytes(combinedData);
    }

    protected abstract object FormatCheckup(DoctorCheckups item);
    protected abstract object FormatPerson(Person item);
    
    protected abstract object CombineCheckups(IEnumerable<object> formattedItems);
    protected abstract object CombinePersons(IEnumerable<object> formattedItems);
    
    protected abstract byte[] GenerateBytes(object combinedData);
}

public class CsvExportStrategy : ExportTemplate {
    public override string Extention => ".csv";
    public override string ContentType => "text/csv";

    protected override object FormatCheckup(DoctorCheckups v) {
        return $"{v.Doctor.Account.Username}, {v.Doctor.Email}, {v.Patient.Account.Username}, {v.Patient.Email}, {v.Description}, {v.AppointmentDate}";
    }

    protected override object FormatPerson(Person v) {
        string speciality = v is Doctor d ? d.Specialty : string.Empty;
        return $"{v.Account.Username}, {v.Email}, {v.Role.ToString()}, {speciality}";
    }

    protected override object CombineCheckups(IEnumerable<object> formattedItems) {
        var builder = new StringBuilder();
        builder.AppendLine("DoctorName, DoctorEmail, PatientName, PatientEmail, Description, AppointmentDate");
        foreach (string item in formattedItems) {
            builder.AppendLine(item);
        }
        return builder.ToString();
    }

    protected override object CombinePersons(IEnumerable<object> formattedItems) {
        var builder = new StringBuilder();
        builder.AppendLine("UserName, Email, Role, Specialty");
        foreach (string item in formattedItems) {
            builder.AppendLine(item);
        }
        return builder.ToString();
    }

    protected override byte[] GenerateBytes(object combinedData) {
        return Encoding.UTF8.GetBytes((string)combinedData);
    }
}

public class JsonExportStrategy : ExportTemplate {
    public override string Extention => ".json";
    public override string ContentType => "application/json";

    protected override object FormatCheckup(DoctorCheckups v) {
        return new {
            DoctorName = v.Doctor.Account.Username,
            DoctorEmail = v.Doctor.Email,
            PatientName = v.Patient.Account.Username,
            PatientEmail = v.Patient.Email,
            Description = v.Description,
            Date = v.AppointmentDate.ToShortDateString()
        };
    }

    protected override object FormatPerson(Person v) {
        return new {
            UserName = v.Account.Username,
            Email = v.Email,
            Role = v.Role.ToString(),
            Speciality = (v as Doctor)?.Specialty
        };
    }

    protected override object CombineCheckups(IEnumerable<object> formattedItems) {
        return formattedItems.ToList();
    }

    protected override object CombinePersons(IEnumerable<object> formattedItems) {
        return formattedItems.ToList();
    }

    protected override byte[] GenerateBytes(object combinedData) {
        var options = new JsonSerializerOptions { 
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.SerializeToUtf8Bytes(combinedData, options);
    }
}

public class XmlExportStrategy : ExportTemplate {
    public override string Extention => ".xml";
    public override string ContentType => "application/xml";

    protected override object FormatCheckup(DoctorCheckups v) {
        return new XElement("Checkup", 
            new XElement("DoctorName", v.Doctor.Account.Username),
            new XElement("DoctorEmail", v.Doctor.Email),
            new XElement("PatientName", v.Patient.Account.Username),
            new XElement("PatientEmail", v.Patient.Email),
            new XElement("Description", v.Description),
            new XElement("AppointmentDate", v.AppointmentDate.ToShortDateString())
        );
    }

    protected override object FormatPerson(Person v) {
        var s = (v as Doctor)?.Specialty;
        return new XElement("account",
            new XElement("UserName", v.Account.Username),
            new XElement("Email", v.Email),
            new XElement("Role", v.Role),
            s == null ? null : new XElement("Specialty", s)
        );
    }

    protected override object CombineCheckups(IEnumerable<object> formattedItems) {
        return new XDocument(
            new XElement("DoctorCheckups", formattedItems.Cast<XElement>())
        );
    }

    protected override object CombinePersons(IEnumerable<object> formattedItems) {
        return new XDocument(
            new XElement("accounts", formattedItems.Cast<XElement>())
        );
    }

    protected override byte[] GenerateBytes(object combinedData) {
        var doc = (XDocument)combinedData;
        using var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();
    }
}

public static class ExportHelper {
    public static IExport GetStrategy(Formats f) {
        return f switch {
            Formats.Xml => new XmlExportStrategy(),
            Formats.Json => new JsonExportStrategy(),
            Formats.Csv => new CsvExportStrategy(),
            _ => throw new NotImplementedException("Unknown format: " + f)
        };
    }
}