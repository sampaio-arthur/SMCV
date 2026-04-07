namespace SMCV.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entity, Guid id)
        : base($"{entity} with Id '{id}' was not found.") { }
}
