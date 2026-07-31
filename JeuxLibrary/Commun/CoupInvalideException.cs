using System;

namespace JeuxLibrary.Commun;

public class CoupInvalideException : Exception
{
    public CoupInvalideException(string message)
        : base(message)
    {
    }

    public CoupInvalideException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
