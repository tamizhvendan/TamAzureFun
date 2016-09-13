#load "SuaveAdapter.fsx"
#load "app.fsx"
open SuaveAdapter
open App
open System.Net.Http
open Suave

let Run (req : HttpRequestMessage, log : TraceWriter) = 
  RunWebPartWithPathAsync "url" app req |> Async.RunSynchronously