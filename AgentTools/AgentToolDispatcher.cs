using AgentTools;
using System.Text.Json;

public class AgentToolDispatcher
{
    private readonly ProjectTools _projectTools;
    private readonly ProjectSprintTools _projectSprintTools;

     private readonly ProjectMembersTools _projectMembersTools;

    public AgentToolDispatcher(
        ProjectTools projectTools,
        ProjectSprintTools projectSprintTools,
        ProjectMembersTools projectMembersTools)
    {
        _projectTools = projectTools;
        _projectSprintTools = projectSprintTools;
        _projectMembersTools = projectMembersTools; 
    }

    public async Task<object> ExecuteAsync(
        string functionName,
        Dictionary<string, object>? arguments = null)
    {
        arguments ??= new Dictionary<string, object>();

        var tools = new Dictionary<string, Func<Task<object>>>
        {
            ["get_projects"] = async () =>
                await _projectTools.GetProjects(),
            ["get_project_sprint"] = async () =>
            {
                var projectName = GetStringArgument(arguments, "projectName");
                if (string.IsNullOrWhiteSpace(projectName))
                {
                    throw new InvalidOperationException(
                        "O nome do projeto é obrigatório para consultar as sprints.");
                }

                return await _projectSprintTools.GetProjectSprints(projectName);
            },
            ["get_project_members"] = async () =>
            {
                var projectName = GetStringArgument(arguments, "projectName");
                if (string.IsNullOrWhiteSpace(projectName))
                {
                    throw new InvalidOperationException(
                        "O nome do projeto é obrigatório para consultar as sprints.");
                }

                return await _projectMembersTools.GetProjectMembersByName(projectName);
            }
        };

        if (!tools.TryGetValue(functionName, out var tool))
        {
            throw new InvalidOperationException(
                $"Tool '{functionName}' não encontrada.");
        }

        return await tool();
    }

    private static string? GetStringArgument(
        Dictionary<string, object> arguments,
        string argumentName)
    {
        var argument = arguments.FirstOrDefault(pair =>
            string.Equals(
                pair.Key.Replace("_", string.Empty),
                argumentName,
                StringComparison.OrdinalIgnoreCase));

        return argument.Value switch
        {
            string value => value,
            JsonElement { ValueKind: JsonValueKind.String } value => value.GetString(),
            _ => null
        };
    }
}
