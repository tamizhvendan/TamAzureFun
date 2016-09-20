open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Text
open Suave.Json
open Suave.Http
open Suave
open Suave.Operators

let toJson<'T> (o: 'T) =
  let settings = new JsonSerializerSettings()
  settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
  JsonConvert.SerializeObject(o, settings)
  |> Encoding.UTF8.GetBytes

let fromJson<'T> (bytes : byte []) =
  let json = Encoding.UTF8.GetString bytes
  JsonConvert.DeserializeObject(json, typeof<'T>) :?> 'T

let mapJsonWith<'TIn, 'TOut> (deserializer:byte[] -> 'TIn) (serializer:'TOut->byte[]) webpart f =
  request(fun r ->
    f (deserializer r.rawForm)
    |> serializer
    |> webpart
    >=> Writers.setMimeType "application/json")

let MapJson<'T1,'T2> webpart = mapJsonWith<'T1,'T2> fromJson toJson webpart
let ToJson webpart x  = toJson x |> webpart >=> Writers.setMimeType "application/json"   