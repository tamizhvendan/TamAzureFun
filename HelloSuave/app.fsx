module App
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters

let app = 
  path "/test2" >=> choose [
                      GET >=> OK "GET"
                      POST >=> OK "POST"
                      PUT >=> OK "PUT"
                      DELETE >=> OK "DELETE"
                    ]