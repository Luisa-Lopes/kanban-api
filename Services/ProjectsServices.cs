

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManager.Data;
using Request.Models;
using Response.Models;
using Tables.Models;

namespace Service;


public class ProjectsService
{
    
    private readonly AppDbContext _dbContext;

    public ProjectsService (AppDbContext dbContext)
    {
         _dbContext = dbContext;
    }


   public async Task<List<ProjectsResponse>> GetProjects()
{
    return await _dbContext.Projects
        .Select(r => new ProjectsResponse
        {
            id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Owner = r.Owner
        })
        .ToListAsync();
}

public async Task<ProjectsResponse> CreateProject(
    ProjectsRequest request)
{
    var project = new Projects
    {
        Name = request.Name,
        Description = request.Description,
        Owner = request.Owner
    };

    _dbContext.Projects.Add(project);

    await _dbContext.SaveChangesAsync();

    return new ProjectsResponse
    {
        id = project.Id,
        Name = project.Name,
        Description = project.Description,
        Owner = project.Owner
    };
}

public async Task<ProjectsResponse?> UpdateProject(int id, ProjectsRequest request)
{
    var project = await _dbContext.Projects.FindAsync(id);
    if (project is null)
    {
        return null;
    }

    project.Name = request.Name;
    project.Description = request.Description;
    project.Owner = request.Owner;

    await _dbContext.SaveChangesAsync();

    return new ProjectsResponse
    {
        id = project.Id,
        Name = project.Name,
        Description = project.Description,
        Owner = project.Owner
    };
}

public async Task<bool> DeleteProject(int id)
{
    var project = await _dbContext.Projects.FindAsync(id);
    if (project is null)
    {
        return false;
    }

    _dbContext.Projects.Remove(project);
    await _dbContext.SaveChangesAsync();

    return true;
}

}