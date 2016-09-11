#load "app.fsx"
open App
open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Threading
open System.Threading.Tasks
open Suave.Http
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters

let ToSuaveHttpMethod (httpMethod : System.Net.Http.HttpMethod) = 
  match httpMethod.Method with
  | "GET" -> HttpMethod.GET
  | "POST" -> HttpMethod.POST
  | "PUT" -> HttpMethod.PUT
  | _ -> HttpMethod.OTHER "unknown"

let ToSuaveRequest (req : HttpRequestMessage) = 
  {HttpRequest.empty with 
    url = req.RequestUri
    ``method`` = ToSuaveHttpMethod req.Method
    }

let toHttpResponseMessage (httpResult : HttpResult) = 
  let content = function
  | Bytes c -> c
  | _ -> Array.empty
  let res = new HttpResponseMessage()
  res.Content <- new ByteArrayContent(content httpResult.content)
  res

let Run (req : HttpRequestMessage, log : TraceWriter) =  
  req.Method.Method
  |> sprintf "request type is %s"
  |> log.Info
  let ctx = { HttpContext.empty with request = ToSuaveRequest req}
  let res = system ctx |> Async.RunSynchronously
  match res with
  | Some ctx ->
    toHttpResponseMessage ctx.response
  | _ -> 
    let res = new HttpResponseMessage()
    res.Content <- new ByteArrayContent(Array.empty)
    res.StatusCode <- HttpStatusCode.NotFound
    res