#load "SuaveAdapter.fsx"
#load "app.fsx"
open SuaveAdapter
open App
open System.Net.Http
open Suave

let Run (req : HttpRequestMessage, log : TraceWriter) = 
  let res, _ = RunWebPartWithPathAsync app req |> Async.RunSynchronously
  res