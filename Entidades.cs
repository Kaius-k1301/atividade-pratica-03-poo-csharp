using ConsertaByte.Contratos;
namespace ConsertaByte.Modelos;

public abstract class Pessoa
{
    public int Codigo { get; set; }
    public string Nome { get; set; } = "";
    protected Pessoa() { }
    protected Pessoa(int codigo, string nome) { this.Codigo = codigo; this.Nome = nome; }
    public virtual string Mostrar() => $"{Codigo} - {Nome}";
}
public class Cliente : Pessoa
{
    public string Contato { get; set; } = "";
    public Cliente() { }
    public Cliente(int codigo, string nome, string contato) : base(codigo, nome) { this.Contato = contato; }
    public override string Mostrar() => base.Mostrar() + $" | {Contato}";
}
public class Tecnico : Pessoa
{
    public string Especialidade { get; set; } = "";
    public Tecnico() { }
    public Tecnico(int codigo, string nome, string especialidade) : base(codigo, nome) { this.Especialidade = especialidade; }
    public override string Mostrar() => base.Mostrar() + $" | técnico de {Especialidade}";
}
public class Equipamento
{
    public int Codigo { get; set; }
    public int ClienteCodigo { get; set; }
    public string Tipo { get; set; } = "";
    public string Marca { get; set; } = "";
    public string NumeroSerie { get; set; } = "";
    public string DefeitoRelatado { get; set; } = "";
}
public abstract class Atendimento
{
    private string status = "Recebida";
    public int Numero { get; set; }
    public DateTime DataAbertura { get; set; } = DateTime.Now;
    public decimal MaoDeObra { get; set; }
    public string Status { get => status; set => status = value; }
    public virtual decimal CalcularBase() => MaoDeObra;
}
public class Peca
{
    public string Nome { get; set; } = "";
    public decimal Preco { get; set; }
}
public class OrdemServico : Atendimento, ICalculavel
{
    public int EquipamentoCodigo { get; set; }
    public int TecnicoCodigo { get; set; }
    public bool Urgente { get; set; }
    public List<Peca> Pecas { get; set; } = [];
    public override decimal CalcularBase() => base.CalcularBase() + (Urgente ? 35 : 0);
    public decimal CalcularTotal() => CalcularBase() + Pecas.Sum(p => p.Preco);
}
public class Diagnostico
{
    public int OrdemNumero { get; set; }
    public string CausaEncontrada { get; set; } = "";
    public string SolucaoIndicada { get; set; } = "";
    public bool TesteDeLigacao { get; set; }
}
public class AutorizacaoDados
{
    public int OrdemNumero { get; set; }
    public bool AutorizaBackup { get; set; }
    public bool AutorizaFormatacao { get; set; }
    public DateTime RegistradaEm { get; set; } = DateTime.Now;
}
public class OficinaDados
{
    public List<Cliente> Clientes { get; set; } = [];
    public List<Tecnico> Tecnicos { get; set; } = [];
    public List<Equipamento> Equipamentos { get; set; } = [];
    public List<OrdemServico> Ordens { get; set; } = [];
    public List<Diagnostico> Diagnosticos { get; set; } = [];
    public List<AutorizacaoDados> Autorizacoes { get; set; } = [];
}
