namespace RedisServer;

public record StringSimple
{
    private string contenido;

    public StringSimple(string contenido)
    {
        this.Contenido = contenido;
    }

    public string Contenido
    {
        get => contenido;
        set => contenido = value ?? throw new ArgumentNullException(nameof(value));
    }
}