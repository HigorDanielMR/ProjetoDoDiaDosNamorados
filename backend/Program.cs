using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adicionar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAppVue",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configurar pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirAppVue");

// Servir arquivos estáticos da pasta uploads
var caminhoUploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(caminhoUploads))
{
    Directory.CreateDirectory(caminhoUploads);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(caminhoUploads),
    RequestPath = "/uploads"
});

// Endpoints da API - Contagem de Tempo
app.MapGet("/api/contagem/data-inicial", () =>
{
    var dataInicial = new DateTime(2022, 3, 21, 0, 0, 0, DateTimeKind.Utc);
    return Results.Ok(new { dataInicial = dataInicial.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") });
});

app.MapGet("/api/contagem/tempo-atual", () =>
{
    var tempoAtual = DateTime.UtcNow;
    return Results.Ok(new { tempoAtual = tempoAtual.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") });
});

app.MapGet("/api/contagem/calcular", () =>
{
    var dataInicial = new DateTime(2022, 3, 21, 0, 0, 0, DateTimeKind.Utc);
    var tempoAtual = DateTime.UtcNow;
    var diferenca = tempoAtual - dataInicial;
    
    var anos = (int)(diferenca.TotalDays / 365.25);
    var meses = (int)((diferenca.TotalDays % 365.25) / 30.44);
    var dias = (int)(diferenca.TotalDays % 30.44);
    var horas = diferenca.Hours;
    var minutos = diferenca.Minutes;
    var segundos = diferenca.Seconds;
    var milissegundos = diferenca.Milliseconds;
    
    return Results.Ok(new
    {
        anos,
        meses,
        dias,
        horas,
        minutos,
        segundos,
        milissegundos,
        totalDias = (int)diferenca.TotalDays,
        totalHoras = (int)diferenca.TotalHours,
        totalMinutos = (int)diferenca.TotalMinutes,
        totalSegundos = (int)diferenca.TotalSeconds,
        totalMilissegundos = (long)diferenca.TotalMilliseconds
    });
});

// Endpoint para fazer upload de fotos
app.MapPost("/api/fotos/enviar", async (HttpRequest requisicao) =>
{
    try
    {
        if (!requisicao.HasFormContentType)
        {
            return Results.BadRequest("Tipo de conteúdo inválido");
        }

        var formulario = await requisicao.ReadFormAsync();
        var arquivos = formulario.Files;

        if (arquivos.Count == 0)
        {
            return Results.BadRequest("Nenhum arquivo foi enviado");
        }

        var fotosEnviadas = new List<object>();
        var caminhoUploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        foreach (var arquivo in arquivos)
        {
            if (arquivo.Length > 0 && arquivo.ContentType.StartsWith("image/"))
            {
                // Gerar nome único para o arquivo
                var extensaoArquivo = Path.GetExtension(arquivo.FileName);
                var nomeUnico = $"{Guid.NewGuid()}{extensaoArquivo}";
                var caminhoCompleto = Path.Combine(caminhoUploads, nomeUnico);

                // Salvar arquivo no servidor
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                // Criar informações da foto
                var informacoesFoto = new
                {
                    id = Guid.NewGuid().ToString(),
                    nome = arquivo.FileName,
                    nomeArquivo = nomeUnico,
                    url = $"/uploads/{nomeUnico}",
                    dataEnvio = DateTime.UtcNow,
                    tamanho = arquivo.Length
                };

                fotosEnviadas.Add(informacoesFoto);

                // Salvar metadados da foto em arquivo JSON
                await SalvarMetadadosFoto(informacoesFoto);
            }
        }

        return Results.Ok(new { fotos = fotosEnviadas });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao enviar arquivos: {ex.Message}");
    }
});

// Endpoint para buscar todas as fotos
app.MapGet("/api/fotos", async () =>
{
    try
    {
        var caminhoMetadados = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "metadados-fotos.json");
        
        if (!File.Exists(caminhoMetadados))
        {
            return Results.Ok(new { fotos = new List<object>() });
        }

        var conteudoJson = await File.ReadAllTextAsync(caminhoMetadados);
        var fotos = System.Text.Json.JsonSerializer.Deserialize<List<object>>(conteudoJson);
        
        return Results.Ok(new { fotos = fotos ?? new List<object>() });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao buscar fotos: {ex.Message}");
    }
});

// Endpoint para deletar foto específica
app.MapDelete("/api/fotos/{idFoto}", async (string idFoto) =>
{
    try
    {
        var caminhoMetadados = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "metadados-fotos.json");
        
        if (!File.Exists(caminhoMetadados))
        {
            return Results.NotFound("Foto não encontrada");
        }

        var conteudoJson = await File.ReadAllTextAsync(caminhoMetadados);
        var fotos = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(conteudoJson) ?? new List<Dictionary<string, object>>();
        
        var fotoParaDeletar = fotos.FirstOrDefault(f => f.ContainsKey("id") && f["id"].ToString() == idFoto);
        
        if (fotoParaDeletar == null)
        {
            return Results.NotFound("Foto não encontrada");
        }

        // Deletar arquivo físico do servidor
        if (fotoParaDeletar.ContainsKey("nomeArquivo"))
        {
            var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fotoParaDeletar["nomeArquivo"].ToString());
            if (File.Exists(caminhoArquivo))
            {
                File.Delete(caminhoArquivo);
            }
        }

        // Remover dos metadados
        fotos.Remove(fotoParaDeletar);
        
        // Salvar metadados atualizados
        var jsonAtualizado = System.Text.Json.JsonSerializer.Serialize(fotos, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(caminhoMetadados, jsonAtualizado);

        return Results.Ok(new { mensagem = "Foto deletada com sucesso" });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao deletar foto: {ex.Message}");
    }
});

// Endpoints para Perfis e Mensagens
app.MapGet("/api/perfis", () =>
{
    try
    {
        var perfis = new[]
        {
            new
            {
                id = "adriana",
                nome = "Adriana",
                genero = "Feminino",
                idade = 19,
                aniversario = "28/03/2006",
                descricao = "Uma pessoa especial que ilumina cada dia com seu sorriso radiante. Ama viajar, ler livros e cozinhar pratos deliciosos. Sempre traz alegria e positividade para todos ao seu redor.",
                cor = "#EC4899",
                emoji = "👸",
                fotoPerfil = "",
                hobbies = new[] { "Leitura", "Carros", "Viagens", "Drift" },
                profissao = "Psicologia",
                cidadeNatal = "Goiânia, GO"
            },
            new
            {
                id = "higor",
                nome = "Higor Daniel",
                genero = "Masculino",
                idade = 19,
                aniversario = "11/11/2005",
                descricao = "Um homem apaixonado.",
                cor = "#8B5CF6",
                emoji = "🤴",
                fotoPerfil = "https://images.pexels.com/photos/220453/pexels-photo-220453.jpeg?auto=compress&cs=tinysrgb&w=400",
                hobbies = new[] { "Programação", "Futebol", "Motos", "Drift" },
                profissao = "Desenvolvedor",
                cidadeNatal = "Aparecida de Goiânia, GO"
            }
        };
        
        Console.WriteLine($"✅ Retornando {perfis.Length} perfis");
        return Results.Ok(new { perfis });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao buscar perfis: {ex.Message}");
        return Results.Problem($"Erro ao buscar perfis: {ex.Message}");
    }
});

app.MapGet("/api/mensagens/{idPerfil}", async (string idPerfil) =>
{
    try
    {
        Console.WriteLine($"🔄 Buscando mensagens para perfil: {idPerfil}");
        
        var caminhoMensagens = Path.Combine(Directory.GetCurrentDirectory(), "uploads", $"mensagens-{idPerfil}.json");
        
        if (!File.Exists(caminhoMensagens))
        {
            Console.WriteLine($"📝 Arquivo de mensagens não existe para {idPerfil}, retornando lista vazia");
            return Results.Ok(new { mensagens = new List<object>() });
        }

        var conteudoJson = await File.ReadAllTextAsync(caminhoMensagens);
        var mensagens = System.Text.Json.JsonSerializer.Deserialize<List<object>>(conteudoJson);
        
        Console.WriteLine($"✅ Retornando {mensagens?.Count ?? 0} mensagens para {idPerfil}");
        return Results.Ok(new { mensagens = mensagens ?? new List<object>() });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao buscar mensagens para {idPerfil}: {ex.Message}");
        return Results.Problem($"Erro ao buscar mensagens: {ex.Message}");
    }
});

app.MapPost("/api/mensagens/{idPerfil}", async (string idPerfil, HttpRequest requisicao) =>
{
    try
    {
        Console.WriteLine($"📨 Recebendo nova mensagem para perfil: {idPerfil}");
        
        using var reader = new StreamReader(requisicao.Body);
        var corpo = await reader.ReadToEndAsync();
        var dados = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(corpo);
        
        if (!dados.ContainsKey("mensagem"))
        {
            Console.WriteLine("❌ Mensagem não fornecida no corpo da requisição");
            return Results.BadRequest("Mensagem é obrigatória");
        }

        var novaMensagem = new
        {
            id = Guid.NewGuid().ToString(),
            mensagem = dados["mensagem"].ToString(),
            data = DateTime.Now.ToString("yyyy-MM-dd"),
            dataCompleta = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            idPerfil
        };

        Console.WriteLine($"💾 Salvando mensagem: {novaMensagem.mensagem}");
        await SalvarMensagemDiaria(idPerfil, novaMensagem);
        
        Console.WriteLine($"✅ Mensagem salva com sucesso para {idPerfil}");
        return Results.Ok(new { mensagem = "Mensagem salva com sucesso", dados = novaMensagem });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao salvar mensagem para {idPerfil}: {ex.Message}");
        return Results.Problem($"Erro ao salvar mensagem: {ex.Message}");
    }
});

// Endpoint para upload de foto de perfil
app.MapPost("/api/perfis/{idPerfil}/foto", async (string idPerfil, HttpRequest requisicao) =>
{
    try
    {
        if (!requisicao.HasFormContentType)
        {
            return Results.BadRequest("Tipo de conteúdo inválido");
        }

        var formulario = await requisicao.ReadFormAsync();
        var arquivo = formulario.Files.FirstOrDefault();

        if (arquivo == null || arquivo.Length == 0)
        {
            return Results.BadRequest("Nenhum arquivo foi enviado");
        }

        if (!arquivo.ContentType.StartsWith("image/"))
        {
            return Results.BadRequest("Arquivo deve ser uma imagem");
        }

        var caminhoUploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "perfis");
        if (!Directory.Exists(caminhoUploads))
        {
            Directory.CreateDirectory(caminhoUploads);
        }

        // Gerar nome único para o arquivo
        var extensaoArquivo = Path.GetExtension(arquivo.FileName);
        var nomeUnico = $"perfil-{idPerfil}-{Guid.NewGuid()}{extensaoArquivo}";
        var caminhoCompleto = Path.Combine(caminhoUploads, nomeUnico);

        // Salvar arquivo no servidor
        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await arquivo.CopyToAsync(stream);
        }

        var urlFoto = $"/uploads/perfis/{nomeUnico}";

        // Salvar informações da foto do perfil
        await SalvarFotoPerfil(idPerfil, urlFoto, nomeUnico);

        return Results.Ok(new { 
            mensagem = "Foto de perfil salva com sucesso", 
            urlFoto = urlFoto,
            nomeArquivo = nomeUnico
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao salvar foto de perfil: {ex.Message}");
    }
});

// Função auxiliar para salvar metadados das fotos
async Task SalvarMetadadosFoto(object informacoesFoto)
{
    var caminhoMetadados = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "metadados-fotos.json");
    
    List<object> fotos;
    
    if (File.Exists(caminhoMetadados))
    {
        var jsonExistente = await File.ReadAllTextAsync(caminhoMetadados);
        fotos = System.Text.Json.JsonSerializer.Deserialize<List<object>>(jsonExistente) ?? new List<object>();
    }
    else
    {
        fotos = new List<object>();
    }
    
    fotos.Add(informacoesFoto);
    
    var json = System.Text.Json.JsonSerializer.Serialize(fotos, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(caminhoMetadados, json);
}

// Função auxiliar para salvar mensagens diárias
async Task SalvarMensagemDiaria(string idPerfil, object novaMensagem)
{
    try
    {
        var caminhoMensagens = Path.Combine(Directory.GetCurrentDirectory(), "uploads", $"mensagens-{idPerfil}.json");
        
        List<object> mensagens;
        
        if (File.Exists(caminhoMensagens))
        {
            var jsonExistente = await File.ReadAllTextAsync(caminhoMensagens);
            mensagens = System.Text.Json.JsonSerializer.Deserialize<List<object>>(jsonExistente) ?? new List<object>();
        }
        else
        {
            mensagens = new List<object>();
        }
        
        mensagens.Add(novaMensagem);
        
        var json = System.Text.Json.JsonSerializer.Serialize(mensagens, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(caminhoMensagens, json);
        
        Console.WriteLine($"💾 Mensagem salva no arquivo: {caminhoMensagens}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao salvar mensagem no arquivo: {ex.Message}");
        throw;
    }
}

// Função auxiliar para salvar foto de perfil
async Task SalvarFotoPerfil(string idPerfil, string urlFoto, string nomeArquivo)
{
    var caminhoFotosPerfis = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "fotos-perfis.json");
    
    Dictionary<string, object> fotosPerfis;
    
    if (File.Exists(caminhoFotosPerfis))
    {
        var jsonExistente = await File.ReadAllTextAsync(caminhoFotosPerfis);
        fotosPerfis = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonExistente) ?? new Dictionary<string, object>();
    }
    else
    {
        fotosPerfis = new Dictionary<string, object>();
    }
    
    fotosPerfis[idPerfil] = new
    {
        urlFoto,
        nomeArquivo,
        dataUpload = DateTime.UtcNow
    };
    
    var json = System.Text.Json.JsonSerializer.Serialize(fotosPerfis, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(caminhoFotosPerfis, json);
}

Console.WriteLine("🚀 Servidor iniciado em http://localhost:5000");
Console.WriteLine("📋 Endpoints disponíveis:");
Console.WriteLine("  - GET /api/perfis");
Console.WriteLine("  - GET /api/mensagens/{idPerfil}");
Console.WriteLine("  - POST /api/mensagens/{idPerfil}");
Console.WriteLine("  - POST /api/perfis/{idPerfil}/foto");
Console.WriteLine("  - GET /api/fotos");
Console.WriteLine("  - POST /api/fotos/enviar");
Console.WriteLine("  - DELETE /api/fotos/{idFoto}");

app.Run();