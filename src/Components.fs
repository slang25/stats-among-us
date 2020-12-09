module Components

open System
open Browser
open Color
open Fable.Core
open Fable.Import
open Feliz
open Elmish
open Feliz.RoughViz

module internal Interop =
    [<Emit("new ResizeObserver($0)")>]
    let createResizeObserver(handler: IObserverEntry array -> unit) : IResizeObserver = jsNative

[<ReactComponent>]
let RoughDivider() =
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
            let w = window.getComputedStyle(canvas.parentElement).width |> JS.parseFloat
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
    
    


[<ReactComponent>]
let Avatar() =
    let playerColors =
        [|
            ("#C51111", "#7A0838")
            ("#132ED1", "#09158E")
            ("#117F2D", "#0A4D2E")
            ("#ED54BA", "#AB2BAD")
            ("#EF7D0D", "#B33E15")
            ("#F5F557", "#C38823")
            ("#3F474E", "#1E1F26")
            ("#D6E0F0", "#8394BF")
            ("#6B2FBB", "#3B177C")
            ("#71491E", "#5E2615")
            ("#38FEDC", "#24A8BE")
            ("#50EF39", "#15A742")
        |]
    
    let canvasRef = React.useRef<Browser.Types.CanvasRenderingContext2D option> None
    let imageRef = React.useRef<Browser.Types.HTMLImageElement option> None
    let hatRef = React.useRef<Browser.Types.HTMLImageElement option> None
    let skinRef = React.useRef<Browser.Types.HTMLImageElement option> None
    let hat, setHat = React.useState(1)
    let skin, _ = React.useState(5)
    
    let waitForLoad (image: Browser.Types.HTMLImageElement) = promise {
        if not image.complete then
            do! Promise.create(fun ok er ->
                               image.onload <- (fun _ -> ok())
                               image.onerror <- (fun e -> er(failwith "Error loading image"))
                              )
    }

    React.useEffect((fun () ->
        promise {
            match canvasRef.current, imageRef.current, hatRef.current, skinRef.current with
            | Some context, Some image, Some hatImage, Some skinImage  ->
                let! _ = Promise.all [|
                    image |> waitForLoad
                    hatImage |> waitForLoad
                    skinImage |> waitForLoad
                |]
                
                let canvas = context.canvas
                canvas.width <- image.width
                canvas.height <- image.height
                context.clearRect(0., 0., canvas.width, canvas.height)
                context.drawImage(U3.Case1 image, 0., 0.)
                
                let drawHat() =
                    let hatOffsets = [| 45; 25; 50; 37; 33; 60; 70; 20; 27; 35; 41; 52; 35; 29; 40; 49; 34; 40; 25; 52; 55; 46; 41; 49; 46; 36; 44; 59; 44; 39; 30; 32; 37; 26; 61; 40; 43; 26; 50; 51; 37; 44; 30; 22; 40; 42; 8; 29; 32; 36; 28; 22; 39; 42; 24; 30; 47; 27; 52; 44; 26; 44; 48; 47; 42; 48; 50; 32; 44; 38; 56; 19; 27; 30; 42; 43; 60; 34; 10; 45; 50; 33; 13; 2; 40; 32; 32; 55; 22; 999; 26; 29; 43 |]
                    
                    let hatY = 17 - hatOffsets.[(hat-1)] |> float
                    let y = if hatY > 0. then 0. else -hatY
                    
                    context.drawImage(U3.Case1 hatImage, 0., y, hatImage.width, hatImage.height, (canvas.width / 2.) - (hatImage.width / 2.) + 2., JS.Math.max(hatY, 0.), hatImage.width, hatImage.height)
                    ()
                
                let playerColorChoice = 4
                let data = context.getImageData(0., 0., image.width, image.height)
                for i in 0..4..data.data.Length-1 do
                    let r = data.data.[i]
                    let g = data.data.[i+1]
                    let b = data.data.[i+2]
                    if r <> 255uy || g <> 255uy || b <> 255uy then
                        let pixelColor =
                            Color.Create("#000000")
                                .mix(Color.Create(playerColors.[playerColorChoice] |> snd), (b |> float) / 255.)
                                .mix(Color.Create(playerColors.[playerColorChoice] |> fst), (r |> float) / 255.)
                                .mix(Color.Create("#9acad5"), (g |> float) / 255.)
                                
                        data.data.[i] <- (pixelColor.red() |> uint8)
                        data.data.[i+1] <- (pixelColor.green() |> uint8)
                        data.data.[i+2] <- (pixelColor.blue() |> uint8)
                        ()
                context.putImageData(data, 0., 0.)
                let backgroundHats = [38; 3; 5; 14; 28]
                if (backgroundHats |> List.contains (hat-1)) then
                    context.globalCompositeOperation <- "destination-over"
                
                drawHat()
                context.globalCompositeOperation <- "source-over"
                context.drawImage(U3.Case1 skinImage, 25., 46.)
                
                ()
            | _ -> ()
        } |> Promise.start), [| hat :> obj |]
    )
    Html.div [
        Html.img [
            prop.style [ style.display.none ]
            prop.src "/img/avatar.png"
            prop.ref (fun x -> 
                let image = x :?> Browser.Types.HTMLImageElement
                if image |> isNull |> not then 
                    imageRef.current <- Some(image)
            )
        ]
        Html.img [
            prop.style [ style.display.none ]
            prop.src (sprintf "/img/hats/%i.png" hat)
            prop.ref (fun x -> 
                let image = x :?> Browser.Types.HTMLImageElement
                if image |> isNull |> not then 
                    hatRef.current <- Some(image)
            )
        ]
        Html.img [
            prop.style [ style.display.none ]
            prop.src (sprintf "/img/skins/%i.png" skin)
            prop.ref (fun x -> 
                let image = x :?> Browser.Types.HTMLImageElement
                if image |> isNull |> not then 
                    skinRef.current <- Some(image)
            )
        ]
        Html.canvas [
            prop.ref (fun x -> 
                let canvas = x :?> Browser.Types.HTMLCanvasElement
                if canvas |> isNull |> not then
                    canvasRef.current <- Some(canvas.getContext_2d())
            )
        ]
        Html.button [
            prop.onClick (fun _ -> setHat (hat + 1))
            prop.text "Click here"
        ]
    ]