namespace Akka.FSharp.Extensions

open System
open Akka.Actor

type FsReceiveActor() =
    inherit ReceiveActor()

    member _.Self = ActorBase.Context.Self
    member _.Sender = ActorBase.Context.Sender

    member _.FsReceive(handler :'T -> unit) = base.Receive<'T>(handler)

    member this.FsBecome(handler :unit -> unit) = base.Become(handler)

    member _.FsUnbecomeStacked() = base.UnbecomeStacked()

    member _.FsUnhandled message = base.Unhandled message