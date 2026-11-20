namespace RedisServer;

public class RedisError 
{
    private string mensajeError;

    public RedisError(string mensajeError)
    {
        this.mensajeError = mensajeError;
    }

    public string MensajeError
    {
        get => mensajeError;
        set => mensajeError = value ?? throw new ArgumentNullException(nameof(value));
    }
}