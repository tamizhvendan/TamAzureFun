#load "SuaveAdapter.fsx"
#load "app.fsx"
open SuaveAdapter
open App
open System.Net.Http
open Suave

let Run (req : HttpRequestMessage, log : TraceWriter) = 
  let res, ctx = RunWebPartWithPathAsync app req |> Async.RunSynchronously
  ctx |> sprintf "%A" |> log.Info
  res