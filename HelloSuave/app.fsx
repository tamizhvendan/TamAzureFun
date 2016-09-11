open Suave.Http
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters

let system = choose [ POST >=> OK "POST"; GET >=> OK "GET"; PUT >=> OK "PUT"]