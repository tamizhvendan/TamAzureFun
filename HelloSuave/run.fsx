#load "SuaveAdapter.fsx"
#load "app.fsx"
open SuaveAdapter
open App
open System.Net.Http
open Suave

let Run (req : HttpRequestMessage, log : TraceWriter) =  
  let logInfo (x : HttpContext) =
    sprintf "%A\n" x |> log.Info
  RunWebPartAsync app req logInfo |> Async.RunSynchronously