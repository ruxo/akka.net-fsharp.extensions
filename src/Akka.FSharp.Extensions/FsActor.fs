namespace Akka.FSharp.Extensions

open Akka.Actor

type FsActor<'Message>() =
    inherit UntypedActor()
    
    let mutable default_handler :'Message -> unit = fun _ -> ()

    member _.Self = ActorBase.Context.Self
    member _.Sender = ActorBase.Context.Sender

    member _.SetDefaultHandler handler = default_handler <- handler

    member this.FsBecome(handler :'Message -> unit) =
        base.Become(UntypedReceive(this.MakeWrapper handler))

    member _.FsUnbecomeStacked() = base.UnbecomeStacked()

    override this.OnReceive message =
        match message with
        | :? 'Message as msg -> default_handler msg
        | _ -> this.Unhandled message

    member this.MakeWrapper :('Message -> unit) -> (obj -> unit) = fun handler ->
        function
        | :? 'Message as msg -> handler msg
        | msg -> this.FsUnhandled msg

    member _.FsUnhandled message = base.Unhandled message
