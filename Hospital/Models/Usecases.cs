namespace Hospital.Models;

public interface IVisitor : IUseCases {}
public interface IPatient : IUseCases {}
public interface IDoctor : IVisitor {}
public interface IAdministrator : IDoctor {}

public interface IUseCases { }

