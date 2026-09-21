using AgentTools;

public class AgentToolDispatcher
{
    private readonly ProjectTools _projectTools;

    public AgentToolDispatcher(ProjectTools projectTools)
    {
        _projectTools = projectTools;
    }

    public async Task<object> ExecuteAsync(string functionName)
    {
        var tools = new Dictionary<string, Func<Task<object>>>
        {
            ["get_projects"] = async () =>
                await _projectTools.GetProjects()
        };

        if (!tools.TryGetValue(functionName, out var tool))
        {
            throw new InvalidOperationException(
                $"Tool '{functionName}' não encontrada.");
        }

        return await tool();
    }
}