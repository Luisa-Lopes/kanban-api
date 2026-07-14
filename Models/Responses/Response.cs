

using Microsoft.AspNetCore.Http.HttpResults;

namespace Response.Models;

public class Response<T>
{
    public T? Dados {get; set;}

    public string Message {get; set;} = string.Empty;

    public bool Status {get; set;} = true;
    
    public int StatusCode { get; set; }

}