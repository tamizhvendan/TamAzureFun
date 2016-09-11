open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open Suave.Http
open Suave
open Suave.Successful
open System.Threading
open System.Threading.Tasks
open Suave.Operators
open Suave.Filters

let ToSuaveHttpMethod (httpMethod : System.Net.Http.HttpMethod) = 
  match httpMethod.Method with
  | "GET" -> HttpMethod.GET
  | "POST" -> HttpMethod.POST
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

let app = GET >=> OK "hello"

let Run (req : HttpRequestMessage, log : TraceWriter) =  
  log.Info("hello log")
  let ctx = { HttpContext.empty with request = ToSuaveRequest req}
  let res = app ctx |> Async.RunSynchronously
  match res with
  | Some ctx ->
    toHttpResponseMessage ctx.response
  | _ -> 
    let res = new HttpResponseMessage()
    res.Content <- new ByteArrayContent(Array.empty)
    res.StatusCode <- HttpStatusCode.NotFound
    res