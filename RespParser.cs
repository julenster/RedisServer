using System.Reflection.Metadata;

namespace RedisServer;
// TODO EL CODIGO DE AQUI ES PARA DESERIALIZAR MENSAJES.
public class RespParser
{
    public List<object> Parse(string mensaje) //Metodo  mega sigma que parsea todo lo que hay en un string, devuelve una lista con los objetos que tiene dentro en orden.
    {
        var mensajeParseado = new List<object>();
        var indice = 0;
        while (indice < mensaje.Length)
        {
            mensajeParseado.Add(HandleLine(ref indice, mensaje));
        }

        return mensajeParseado;
    }
    public string ParseString(ref int indice, string mensaje) // Para el String se pone un + y al final un \r\n, por lo que tengo que borrar el primer char y los 4 ultimos.
    {
        
        return ReadLine(ref indice, mensaje);
    }

    public string ParseNullBulkString(ref int indice, string mensaje) // Luego cambiar a private y que sea el propio parsebulkstring el que lidie con eso
    {
        return null;
    }

    public string
        ParseBulkString(ref  int indice,  string mensaje) // Los bulks Strings son algo asi --> $4\r\nhola\r\n o por ejemplo $4\r\nping\r\n
    {
        var numeroCaracteresAux = ReadLine(ref indice, mensaje);
        if (int.TryParse(numeroCaracteresAux, out var numeroCaracteres))
        {
            if (numeroCaracteres < 0)
            {
                return ParseNullBulkString(ref indice, mensaje);
            }
            var principio = indice;
            indice = indice + numeroCaracteres;
            var respuesta = mensaje.Substring(principio, indice - principio);
            indice = indice + 2;
            return respuesta;
        }

        throw new FormatException();
    }

    public RedisError ParseError(ref int indice, string mensaje) // Es como el String, pero tiene un - al principio
    {
        var mensajeParsed = ReadLine(ref indice, mensaje);
        RedisError error = new(mensajeParsed);
        return error;
    }

    public int
        ParseInt(ref int indice, string mensaje) // Los enteros solo tienen un unico caracter al principio como los strings, un: por lo que puedo hacer lo mismo
    {
        string mensajeAuxiliar = ReadLine(ref indice, mensaje);
        if (int.TryParse(mensajeAuxiliar, out var numeroParsed)) return numeroParsed;
        return -1;
    }

    public List<object> ParseArray(ref int indice, string mensaje) // Los arrays son del tipo "*2\r\n$3\r\nget\r\n$3\r\nkey\r\n"
    {
        var numeroItemsAux = ReadLine(ref indice, mensaje);
        var numeroItems = int.Parse(numeroItemsAux);
        if (numeroItems < 0)
        {
            return null;
        }
        var  lista = new List<object>();
        while (numeroItems > 0)
        {
           var item= HandleLine(ref indice, mensaje);
           lista.Add(item);
           numeroItems--;
        }

        return lista;
    }

    private String ReadLine(ref int indice, string mensaje) // el indice se cambia por referencia, no se si mola asi pero lo quiero hacer asi
    {
        var linea = "";
        var principio = indice;
        while (indice < mensaje.Length && !(mensaje[indice] == '\r' && mensaje[indice + 1] == '\n'))
        {
            indice++;
        }
        linea = mensaje.Substring(principio, indice - principio);
        indice += 2;
        return linea;
    }
    private object HandleLine(ref int indice, String mensaje)
    {
        var prefijo = mensaje[indice++];
        return prefijo switch
        {   ':' => ParseInt(ref indice, mensaje),
            '+' => ParseString(ref indice,mensaje),
            '$' => ParseBulkString(ref indice, mensaje),
            '-' => ParseError(ref indice,mensaje),
            '*' => ParseArray(ref indice, mensaje),
            _ => throw new NotImplementedException("No se ha implementado todavia este handler")
        };
    }
    
}