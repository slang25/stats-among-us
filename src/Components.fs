module Components

open System
open Browser
open Fable.Core
open Fable.Core.JS
open Feliz
open Elmish
open Feliz.RoughViz

[<ReactComponent>]
let roughDivider() =
    let context = React.useRef<Browser.Types.CanvasRenderingContext2D option> None
    let seed, _ = React.useState (Rough.newSeed())
    let canvasId, _ = React.useState (Random().Next(0, 10000))
    
    let rerendered, setRerendered = React.useState(false)
    let parentWidth, setParentWidth = React.useState(None)
    let observer = React.useRef(Interop.createResizeObserver(fun entries ->
            for entry in entries do setParentWidth(Some entry.contentRect.width)
        )
    )
    
    React.useEffect((fun () ->
        match context.current with
        | None -> ()
        | Some context ->
            let canvas = context.canvas
            let w = window.getComputedStyle(canvas.parentElement).width |> parseFloat
            canvas.width <- w
            let c = Rough.createCanvas(canvas)
            let options = c.getDefaultOptions()
            options.stroke <- color.white
            options.roughness <- 2.
            options.seed <- seed
            c.line(0.,15., w, 15., options) |> ignore
            observer.current.observe canvas.parentElement
        
        React.createDisposable(fun () ->
            match context.current with
            | None -> ()
            | Some context ->
                let element = context.canvas
                observer.current.unobserve element.parentElement
            )
        ),
        [| box rerendered |]
    )
    
    React.useEffect((fun () ->
            parentWidth |> Option.iter (fun _ -> setRerendered(not rerendered))
        ),
        [| box parentWidth |]
    )

    Html.canvas [
        prop.key (sprintf "divider-canvas-%d" canvasId)
        prop.ref (fun x -> 
            let canvas = x :?> Browser.Types.HTMLCanvasElement
            if canvas |> isNull |> not then 
                context.current <- Some(canvas.getContext_2d())
        )
        prop.style [
            style.flexShrink 0
            style.flexGrow 1
        ]
        
        prop.height (30)
    ]