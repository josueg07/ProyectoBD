using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ProyectoBD.FormatoVisual;

// Presentation only. Never alters the JSON saved in the audit trail.
public static class FormatoAuditoria
{
    private static readonly Dictionary<string, string> Etiquetas = new()
    {
        ["NumeroFactura"] = "Número de factura", ["ID_Factura"] = "ID de factura",
        ["ID_DetalleF"] = "ID de detalle", ["ID_ProductoV"] = "ID de variante",
        ["ID_Lote"] = "ID de lote", ["ID_UbicacionA"] = "ID de ubicación",
        ["ID_MetodoP"] = "ID de método de pago", ["ID_Usuario"] = "ID de usuario",
        ["ID_Rol"] = "ID de rol", ["ID_DetalleOC"] = "ID de detalle de orden",
        ["Metodo"] = "Método", ["Referencia"] = "Referencia",
        ["PrecioUnitario"] = "Precio unitario", ["CostoUnitario"] = "Costo unitario",
        ["CantidadRecibida"] = "Cantidad recibida", ["NumeroLote"] = "Número de lote",
        ["DisponibleAntes"] = "Disponible antes", ["DisponibleDespues"] = "Disponible después",
        ["Reintegrada"] = "Cantidad reintegrada", ["NombreUsuario"] = "Nombre de usuario"
    };

    public static string Mostrar(object? valor)
    {
        if (valor is null or DBNull || string.IsNullOrWhiteSpace(Convert.ToString(valor)))
            return "Sin datos registrados";
        string original = Convert.ToString(valor)!;
        try
        {
            using var documento = JsonDocument.Parse(original);
            var texto = new StringBuilder();
            Escribir(documento.RootElement, texto, 0);
            return texto.ToString().TrimEnd();
        }
        catch (JsonException)
        {
            // Legacy text or malformed historical JSON must remain available.
            return original;
        }
    }

    private static void Escribir(JsonElement dato, StringBuilder texto, int nivel)
    {
        string sangria = new(' ', nivel * 2);
        if (dato.ValueKind == JsonValueKind.Object)
        {
            if (!dato.EnumerateObject().Any()) { texto.AppendLine(sangria + "Sin datos"); return; }
            foreach (var campo in dato.EnumerateObject())
            {
                string etiqueta = Etiquetas.TryGetValue(campo.Name, out var nombre) ? nombre
                    : Regex.Replace(campo.Name.Replace('_', ' '), "([a-záéíóúñ])([A-Z])", "$1 $2");
                texto.Append(sangria).Append(etiqueta).Append(':');
                if (campo.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
                {
                    texto.AppendLine();
                    Escribir(campo.Value, texto, nivel + 1);
                }
                else
                {
                    string prefijo = campo.Value.ValueKind == JsonValueKind.Number &&
                        campo.Name is "Total" or "TotalOriginal" or "Monto" or "PrecioUnitario" or "CostoUnitario" or "Subtotal" ? "Q" : "";
                    texto.Append(' ').Append(prefijo).AppendLine(Valor(campo.Value));
                }
            }
        }
        else if (dato.ValueKind == JsonValueKind.Array)
        {
            int numero = 0;
            foreach (var elemento in dato.EnumerateArray())
            {
                texto.Append(sangria).Append("Registro ").Append(++numero).AppendLine(":");
                Escribir(elemento, texto, nivel + 1);
            }
            if (numero == 0) texto.AppendLine(sangria + "Sin registros");
        }
        else texto.Append(sangria).AppendLine(Valor(dato));
    }

    private static string Valor(JsonElement dato) => dato.ValueKind switch
    {
        JsonValueKind.String => dato.GetString() ?? "",
        JsonValueKind.Null => "Sin dato",
        JsonValueKind.True => "Sí",
        JsonValueKind.False => "No",
        _ => dato.GetRawText()
    };
}
