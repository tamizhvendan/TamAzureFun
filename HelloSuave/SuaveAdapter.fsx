module SuaveAdapter
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open Suave.Http

let SuaveHttpMethod (httpMethod : System.Net.Http.HttpMethod) =
  match httpMethod.Method with
  | "GET" -> HttpMethod.GET
  | "POST" -> HttpMethod.POST
  | "PUT" -> HttpMethod.PUT
  | "DELETE" -> HttpMethod.DELETE
  | x -> HttpMethod.OTHER x

let SuaveHeaders (headers : HttpRequestHeaders) =
  headers
  |> Seq.map (fun h -> (h.Key, h.Value |> Seq.head))
  |> Seq.toList

let SuaveRawForm (content : System.Net.Http.HttpContent) = async {
  let! content = content.ReadAsByteArrayAsync() |> Async.AwaitTask
  return content
}

let SuaveRawQuery (requestUri : System.Uri) =
  if requestUri.Query.Length > 1 then
    requestUri.Query.Substring(1)
  else
    ""

let NetHeaderValue key (headers : HttpRequestHeaders) =
    headers
    |> Seq.tryFind (fun h -> h.Key = key)
    |> Option.map (fun h -> h.Value |> Seq.head)

let SuaveRequest (req : HttpRequestMessage) = async {
  let! content = SuaveRawForm req.Content
  let host = defaultArg (NetHeaderValue "Host" req.Headers) ""
  return {HttpRequest.empty with
            url = req.RequestUri            
            ``method`` = SuaveHttpMethod req.Method
            headers = SuaveHeaders req.Headers
            rawForm = content
            rawQuery = SuaveRawQuery req.RequestUri
            host = host}
}
  

let NetStatusCode = function
| HttpCode.HTTP_200 -> HttpStatusCode.OK
| HttpCode.HTTP_201 -> HttpStatusCode.Created
| HttpCode.HTTP_400 -> HttpStatusCode.BadRequest
| HttpCode.HTTP_404 -> HttpStatusCode.NotFound
| _ -> HttpStatusCode.Ambiguous

let NetHttpResponseMessage httpResult =
  let content = function
  | Bytes c -> c
  | _ -> Array.empty
  let res = new HttpResponseMessage()
  res.Content <- new ByteArrayContent(content httpResult.content)
  res.StatusCode <- NetStatusCode httpResult.status
  res

let SuaveContext httpRequest = async {
  let! suaveReq = SuaveRequest httpRequest
  return { HttpContext.empty with request = suaveReq}
}   

let SuaveRunAsync app suaveContext = async {
  let! res = app suaveContext
  match res with
  | Some ctx ->
    return (NetHttpResponseMessage ctx.response, ctx)
  | _ ->
    let res = new HttpResponseMessage()
    res.Content <- new ByteArrayContent(Array.empty)
    res.StatusCode <- HttpStatusCode.NotFound
    return res,suaveContext 
}

let RunWebPartAsync app httpRequest = async {
  let! suaveContext = SuaveContext httpRequest
  return! SuaveRunAsync app suaveContext
}

let RunWebPartWithPathAsync headerName app httpRequest = async {
  let! suaveContext = SuaveContext httpRequest
  match NetHeaderValue headerName httpRequest.Headers with
  | Some url ->
    let ctx = {suaveContext with request = {suaveContext.request with url = new System.Uri(url)}}
    return! SuaveRunAsync app ctx
  | _ -> return! SuaveRunAsync app suaveContext
}