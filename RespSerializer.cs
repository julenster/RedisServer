using System.Security.AccessControl;

namespace RedisServer;
// ESTA CLASE ES PARA SERIALIZAR LISTAS AL FORMATO RESP PARA ENVIAR LOS MENSAJES
public class RespSerializer
{
    public string Serialize(List<object> lista)
    {
        var mensaje = "";
        foreach (var entidad in lista)
        {
            mensaje = mensaje + SerializeEntidad(entidad);
        }
        return mensaje;
    }

    private static string SerializeEntidad(object obj)
    {
        return obj switch
        {
            string s => SerializeBulkString(s),
            int i => SerializeInt(i),
            RedisError err => SerializeErrorMessage(err),
            StringSimple ss => SerializeStringSimple(ss),
            null => SerializeNullBulkString(),
            List<object> l => SerializeArray(l),
            _ => throw new NotImplementedException("Ayuda el colapso es mental"),
        };

    }

    private static string SerializeBulkString(string obj)
    {   var objSerializado = "$" + obj.Length + '\r' + '\n' + obj + '\r' + '\n';
        return objSerializado;
    }

    private static string SerializeInt(int obj)
    {
        var objSerializado = ":" + obj + '\r' + '\n';
        return objSerializado;
    }

    private static string SerializeStringSimple(StringSimple obj)
    {   var objSerializado = "+" + obj.Contenido + '\r' + '\n';
        return objSerializado;
    }

    private static string SerializeErrorMessage(RedisError obj)
    {   var objSerializado = "-" + obj.MensajeError + '\r' + '\n';
        return objSerializado;
    }

    private static string SerializeNullBulkString()
    {
        return "$-1\r\n";
    }

    private static string SerializeArray(List<object> lista)
    {
        string mensaje = "*" + lista.Count + '\r' + '\n';
        foreach (var entidad in lista)
        {
            mensaje = mensaje + SerializeEntidad(entidad);
        }

        return mensaje;
    }
}