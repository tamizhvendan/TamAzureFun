#load "Suave.Newtonsoft.Json.fsx"
open Suave
open Suave.Successful
open Suave.Operators
open Suave.Filters
open System
open Suave.Newtonsoft.Json

type Person = {
  Id : Guid
  Name : string
  Email : string
}

let createPerson person = 
  let newPerson = {person with Id = Guid.NewGuid()}
  newPerson

let getPeople () = [
  {Id = Guid.NewGuid(); Name = "john"; Email = "j@g.co"}
  {Id = Guid.NewGuid(); Name = "mark"; Email = "m@g.co"}]
     
let getPersonById id =
  {Id = Guid.Parse(id); Name = "john"; Email = "j@g.co"}
  |> ToJson ok

let deletePersonById id =
  sprintf "person %s deleted" id |> OK

let app = 
  choose [
    path "/people" >=> choose [
      POST >=> MapJson created createPerson
      GET >=> ToJson ok (getPeople ())
      PUT >=> MapJson accepted id
    ]
    GET >=> pathScan "/people/%s" getPersonById
    DELETE >=> pathScan "/people/%s" deletePersonById
  ]
  
