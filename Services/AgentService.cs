
using AgentTools;
using Google.GenAI;
using Google.GenAI.Types;

namespace Service;

public class AgentService : IAgentService
{
    private readonly Client _client;
    private readonly IConfiguration _configuration;
    private readonly ProjectTools _projectTools;

    public AgentService(Client client, IConfiguration configuration, ProjectTools projectTools)
    {
        _client = client;
        _configuration = configuration;
        _projectTools = projectTools;
    }

     private FunctionDeclaration GetProjectsFunction()
    {
        return new FunctionDeclaration
        {
            Name = "get_projects",

            Description = """
                Obtém a lista de projetos aos quais o usuário
                autenticado possui acesso.

                Use esta função quando o usuário perguntar quais
                projetos possui ou quiser consultar seus projetos.

                 Os projetos retornados contêm informações internas,
                como seus identificadores. Esses identificadores são
                destinados ao uso interno do agente e não devem ser
                exibidos ao usuário.
                """
        };
    }


    public async Task<string> SendMessageAsync(string message)
    {

        GenerateContentConfig config = new (){
                SystemInstruction = new Content
                {
                Parts =
                [
                    new Part
                    {
                        Text = """
                            Você é o Managerzinho, um agente de inteligência
                            artificial integrado a um sistema de gerenciamento
                            de projetos desenvolvido em .NET.

                            Sua função é auxiliar os usuários no gerenciamento
                            de projetos, sprints e tarefas.

                            Responda sempre em português do Brasil.
                            Seja claro, objetivo e profissional.

                             Utilize as ferramentas disponíveis quando precisar
                             consultar informações reais do sistema.

                             Ao apresentar informações ao usuário:
                                - Não exiba IDs internos.
                                - Não revele informações técnicas internas desnecessárias.
                        """
                    }
                ]
            },
            Tools =
            [
                new Tool
                {
                    FunctionDeclarations =
                    [
                        GetProjectsFunction()
                    ]
                }
            ]
        };

        var response = await _client.Models.GenerateContentAsync(
            model: _configuration["Gemini:Model"] ?? "gemini-3.6-flash",
            contents: message,
            config: config
        );

       // Verifica se o Gemini pediu para executar alguma função
    var functionCall = response
        .Candidates?
        .FirstOrDefault()?
        .Content?
        .Parts?
        .FirstOrDefault(p => p.FunctionCall != null)?
        .FunctionCall;

    if (functionCall == null)
    {
        return response.Text ?? string.Empty;
    }

    // Executa a função solicitada pelo Gemini
    if (functionCall.Name == "get_projects")
    {
        var result = await _projectTools.GetProjects();

        // Envia o resultado da função de volta para o Gemini
        var functionResponsePart = Part.FromFunctionResponse(
            name: functionCall.Name,
            response: new Dictionary<string, object>
            {
                ["result"] = result
            }
        );

        var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts =
                [
                    new Part
                    {
                        Text = message
                    }
                ]
            },

            // Resposta anterior do Gemini contendo o FunctionCall
            response.Candidates![0].Content!,

            // Resultado da execução da nossa função
            new Content
            {
                Role = "user",
                Parts =
                [
                    functionResponsePart
                ]
            }
        };

        var finalResponse = await _client.Models.GenerateContentAsync(
            model: "gemini-3.7-flash" ?? "gemini-3.6-flash",
            contents: contents,
            config: config
        );

        return finalResponse.Text ?? string.Empty;
    }

    return response.Text ?? string.Empty;
    }
}