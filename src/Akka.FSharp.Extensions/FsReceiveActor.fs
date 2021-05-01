namespace Akka.FSharp.Extensions

open Akka.Actor

type FsReceiveActor() =
    inherit ReceiveActor()

    member _.Sender = ActorBase.Context.Sender