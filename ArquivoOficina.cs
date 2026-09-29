using System.Text.Json;
using ConsertaByte.Modelos;
namespace ConsertaByte.Dados;
public static class ArquivoOficina
{
    public static string NomeArquivo { get; } = "assistencia_dados.json";
    public static void Gravar(OficinaDados dados) => File.WriteAllText(NomeArquivo, JsonSerializer.Serialize(dados, new JsonSerializerOptions { WriteIndented = true }));
    public static OficinaDados Ler()
    {
        if (!File.Exists(NomeArquivo)) return new OficinaDados();
        return JsonSerializer.Deserialize<OficinaDados>(File.ReadAllText(NomeArquivo)) ?? new OficinaDados();
    }
}
