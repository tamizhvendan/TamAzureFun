module App
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters

let app = 
  choose [
    GET >=> OK "GET"
    POST >=> OK "POST"
    PUT >=> OK "PUT"
    DELETE >=> OK "DELETE"
  ]