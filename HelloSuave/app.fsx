open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters

let app = 
  choose [
      GET >=> OK "GET test"
      POST >=> OK "POST test"
      PUT >=> OK "PUT test"
      DELETE >=> OK "DELETE test"]
  
