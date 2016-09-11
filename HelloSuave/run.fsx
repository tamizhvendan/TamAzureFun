open System.Net
open System.Net.Http.Headers
open System

let Run (req: HttpRequestMessage) =
    let response = new HttpResponseMessage()
    response.Content <- new StringContent(""" { "hello" : "world" } """)
    response.StatusCode <- HttpStatusCode.OK
    response.Content.Headers.ContentType <- MediaTypeHeaderValue("application/json")
    response