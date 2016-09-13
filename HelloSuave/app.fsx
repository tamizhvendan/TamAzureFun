module App
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters

let app = 
  choose [
    path "/test2" >=> choose [
      GET >=> OK "GET test2"
      POST >=> OK "POST test2"
      PUT >=> OK "PUT test2"
      DELETE >=> OK "DELETE test2"
    ]
    path "/test" >=> choose [
      GET >=> OK "GET test"
      POST >=> OK "POST test"
      PUT >=> OK "PUT test"
      DELETE >=> OK "DELETE test"
    ]
  ]
                    