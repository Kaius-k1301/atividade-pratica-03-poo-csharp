using ConsertaByte.Dados;
using ConsertaByte.Modelos;

OficinaDados banco = ArquivoOficina.Ler();
if (banco.Tecnicos.Count == 0) banco.Tecnicos.Add(new Tecnico(1, "Rafael", "hardware"));
int escolha = -1;
do
{
    Console.WriteLine("\n--------------------------------");
    Console.WriteLine(" CONSERTABYTE - BANCADA TÉCNICA");
    Console.WriteLine("--------------------------------");
    Console.WriteLine("1  Cadastrar cliente");
    Console.WriteLine("2  Receber computador");
    Console.WriteLine("3  Abrir ordem de serviço");
    Console.WriteLine("4  Lançar peça na ordem");
    Console.WriteLine("5  Registrar diagnóstico");
    Console.WriteLine("6  Mostrar orçamento");
    Console.WriteLine("7  Alterar etapa do reparo");
    Console.WriteLine("8  Autorizar backup e formatação");
    Console.WriteLine("9  Listar ordens pendentes de autorização");
    Console.WriteLine("10 Consultar ordem");
    Console.WriteLine("11 Gravar arquivo");
    Console.WriteLine("0  Encerrar");
    Console.Write("Escolha: ");
    try
    {
        escolha = Convert.ToInt32(Console.ReadLine());
        if (escolha == 1) NovoCliente();
        else if (escolha == 2) ReceberEquipamento();
        else if (escolha == 3) AbrirOrdem();
        else if (escolha == 4) ColocarPeca();
        else if (escolha == 5) FazerDiagnostico();
        else if (escolha == 6) MostrarOrcamento();
        else if (escolha == 7) MudarStatus();
        else if (escolha == 8) AutorizarDados();
        else if (escolha == 9) Pendencias();
        else if (escolha == 10) Consultar();
        else if (escolha == 11) { ArquivoOficina.Gravar(banco); Console.WriteLine("Arquivo atualizado."); }
        else if (escolha != 0) Console.WriteLine("Número fora do menu.");
    }
    catch (Exception erro) { Console.WriteLine("Não foi possível: " + erro.Message); }
} while (escolha != 0);
ArquivoOficina.Gravar(banco);

void NovoCliente()
{
    Console.Write("Nome do cliente: "); string nome = Console.ReadLine() ?? "";
    Console.Write("Telefone ou e-mail: "); string contato = Console.ReadLine() ?? "";
    if (nome.Trim().Length < 3) throw new Exception("nome muito curto");
    banco.Clientes.Add(new Cliente(NovoCodigo(banco.Clientes.Select(c => c.Codigo)), nome, contato));
    Console.WriteLine("Cliente incluído.");
}
void ReceberEquipamento()
{
    if (!banco.Clientes.Any()) { Console.WriteLine("Nenhum cliente cadastrado."); return; }
    banco.Clientes.ForEach(c => Console.WriteLine(c.Mostrar()));
    Console.Write("Código do cliente: "); int cliente = int.Parse(Console.ReadLine() ?? "");
    if (!banco.Clientes.Any(c => c.Codigo == cliente)) throw new Exception("cliente não localizado");
    Console.Write("Tipo (desktop/notebook): "); string tipo = Console.ReadLine() ?? "";
    Console.Write("Marca: "); string marca = Console.ReadLine() ?? "";
    Console.Write("Número de série: "); string serie = Console.ReadLine() ?? "";
    Console.Write("Defeito contado pelo cliente: "); string defeito = Console.ReadLine() ?? "";
    banco.Equipamentos.Add(new Equipamento { Codigo = NovoCodigo(banco.Equipamentos.Select(e => e.Codigo)), ClienteCodigo = cliente, Tipo = tipo, Marca = marca, NumeroSerie = serie, DefeitoRelatado = defeito });
    Console.WriteLine("Equipamento recebido.");
}
void AbrirOrdem()
{
    if (!banco.Equipamentos.Any()) { Console.WriteLine("Receba um equipamento primeiro."); return; }
    banco.Equipamentos.ForEach(e => Console.WriteLine($"{e.Codigo} - {e.Tipo} {e.Marca} - {e.DefeitoRelatado}"));
    Console.Write("Equipamento: "); int equipamento = int.Parse(Console.ReadLine() ?? "");
    if (!banco.Equipamentos.Any(e => e.Codigo == equipamento)) throw new Exception("equipamento inválido");
    Console.Write("Valor da mão de obra: "); decimal mao = decimal.Parse(Console.ReadLine() ?? "");
    if (mao < 0) throw new Exception("valor negativo não é permitido");
    Console.Write("É urgente? (s/n): "); bool urgente = (Console.ReadLine() ?? "").ToLower() == "s";
    banco.Ordens.Add(new OrdemServico { Numero = NovoCodigo(banco.Ordens.Select(o => o.Numero)), EquipamentoCodigo = equipamento, TecnicoCodigo = 1, MaoDeObra = mao, Urgente = urgente });
    Console.WriteLine("Ordem aberta.");
}
void ColocarPeca()
{
    OrdemServico ordem = PedirOrdem();
    Console.Write("Peça usada: "); string nome = Console.ReadLine() ?? "";
    Console.Write("Preço: "); decimal preco = decimal.Parse(Console.ReadLine() ?? "");
    if (preco <= 0) throw new Exception("preço deve ser maior que zero");
    ordem.Pecas.Add(new Peca { Nome = nome, Preco = preco });
}
void FazerDiagnostico()
{
    OrdemServico ordem = PedirOrdem();
    Console.Write("Causa encontrada: "); string causa = Console.ReadLine() ?? "";
    Console.Write("Solução indicada: "); string solucao = Console.ReadLine() ?? "";
    Console.Write("O equipamento ligou no teste? (s/n): "); bool ligou = (Console.ReadLine() ?? "").ToLower() == "s";
    banco.Diagnosticos.RemoveAll(d => d.OrdemNumero == ordem.Numero);
    banco.Diagnosticos.Add(new Diagnostico { OrdemNumero = ordem.Numero, CausaEncontrada = causa, SolucaoIndicada = solucao, TesteDeLigacao = ligou });
    ordem.Status = "Diagnosticada";
    Console.WriteLine("Diagnóstico guardado.");
}
void MostrarOrcamento()
{
    OrdemServico ordem = PedirOrdem();
    Console.WriteLine($"Mão de obra e urgência: R$ {ordem.CalcularBase():F2}");
    ordem.Pecas.ForEach(p => Console.WriteLine($"{p.Nome}: R$ {p.Preco:F2}"));
    Console.WriteLine($"TOTAL: R$ {ordem.CalcularTotal():F2}");
}
void MudarStatus()
{
    OrdemServico ordem = PedirOrdem();
    Console.Write("Novo status (Aguardando peça/Em reparo/Pronta/Entregue): "); string status = Console.ReadLine() ?? "";
    string[] aceitos = ["Aguardando peça", "Em reparo", "Pronta", "Entregue"];
    if (!aceitos.Contains(status)) throw new Exception("status não aceito");
    if (status == "Em reparo" && !banco.Autorizacoes.Any(a => a.OrdemNumero == ordem.Numero)) throw new Exception("registre a decisão sobre os dados antes do reparo");
    ordem.Status = status;
}
void AutorizarDados()
{
    OrdemServico ordem = PedirOrdem();
    Console.Write("Autoriza backup? (s/n): "); bool backup = (Console.ReadLine() ?? "").ToLower() == "s";
    Console.Write("Autoriza formatação? (s/n): "); bool formatar = (Console.ReadLine() ?? "").ToLower() == "s";
    banco.Autorizacoes.RemoveAll(a => a.OrdemNumero == ordem.Numero);
    banco.Autorizacoes.Add(new AutorizacaoDados { OrdemNumero = ordem.Numero, AutorizaBackup = backup, AutorizaFormatacao = formatar });
    Console.WriteLine("Decisão registrada.");
}
void Pendencias()
{
    var pendentes = banco.Ordens.Where(o => !banco.Autorizacoes.Any(a => a.OrdemNumero == o.Numero)).ToList();
    if (!pendentes.Any()) { Console.WriteLine("Nenhuma pendência."); return; }
    pendentes.ForEach(o => Console.WriteLine($"OS {o.Numero} - equipamento {o.EquipamentoCodigo}"));
}
void Consultar()
{
    OrdemServico ordem = PedirOrdem();
    Equipamento e = banco.Equipamentos.First(x => x.Codigo == ordem.EquipamentoCodigo);
    Console.WriteLine($"OS {ordem.Numero} | {e.Tipo} {e.Marca} | {ordem.Status} | R$ {ordem.CalcularTotal():F2}");
    Diagnostico? d = banco.Diagnosticos.FirstOrDefault(x => x.OrdemNumero == ordem.Numero);
    if (d != null) Console.WriteLine($"Diagnóstico: {d.CausaEncontrada}. Solução: {d.SolucaoIndicada}");
}
OrdemServico PedirOrdem()
{
    if (!banco.Ordens.Any()) throw new Exception("não existem ordens");
    Console.Write("Número da OS: "); int numero = int.Parse(Console.ReadLine() ?? "");
    return banco.Ordens.FirstOrDefault(o => o.Numero == numero) ?? throw new Exception("ordem não encontrada");
}
static int NovoCodigo(IEnumerable<int> codigos) => codigos.Any() ? codigos.Max() + 1 : 1;
