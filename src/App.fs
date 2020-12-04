module App

open System
open Browser
open Fable.Core
open Fable.Core.JS
open Feliz
open Elmish
open Feliz.Bulma.Tooltip

type Page = Home | GameStats of string

open Elmish.UrlParser

let pageParser : Parser<Page->Page,Page> =
  oneOf
    [ map Home (s "")
      map GameStats (s "stats" </> str) ]

type State = { BodiesReported: int
               Emergencies: int
               Tasks: int
               AllTasks: int
               SabotagesFixed: int
               ImpostorKills: int
               TimesKilled: int
               TimesEjected: int
               CrewmateStreak: int
               TimesImpostor: int
               TimesCrewmate: int
               GamesStarted: int
               GamesFinished: int
               CrewmateVoteWins: int
               CrewmateTaskWins: int
               ImpostorVoteWins: int
               ImpostorKillWins: int
               ImpostorSabotageWins: int
               Page: Page}

type Msg =
    | StatsUploaded of byte[]
    | DisplayStats of string
    
open Elmish.Navigation

let toHash = 
    function
    | Home -> "#"
    | GameStats encodedStats -> "#stats/" + encodedStats

let urlUpdate (result:Option<Page>) model =
  match result with
  | Some (GameStats encodedStats) ->
      { model with Page = (GameStats encodedStats) }, Cmd.ofMsg (DisplayStats encodedStats)
  | Some page ->
      { model with Page = page }, [] 
  | None ->
      console.error("Error parsing url")
      ( model, Navigation.modifyUrl (toHash model.Page) )      

let init result =
    {
        BodiesReported = 0
        Emergencies = 0
        Tasks = 0
        AllTasks = 0
        SabotagesFixed = 0
        ImpostorKills = 0
        TimesKilled = 0
        TimesEjected = 0
        CrewmateStreak = 0
        TimesImpostor = 0
        TimesCrewmate = 0
        GamesStarted = 0
        GamesFinished = 0
        CrewmateVoteWins = 0
        CrewmateTaskWins = 0
        ImpostorVoteWins = 0
        ImpostorKillWins = 0
        ImpostorSabotageWins = 0
        Page = Home
    } |> urlUpdate result

let processGameBytes (b:byte[]) =
    let zeros = FSharp.Collections.Array.create (4 - b.Length % 4) 0uy
    let b = FSharp.Collections.Array.append b zeros
    let bodiesReported = BitConverter.ToInt32(b, 0)
    let emergencies = BitConverter.ToInt32(b, 4)
    let tasks = BitConverter.ToInt32(b, 8)
    let allTasks = BitConverter.ToInt32(b, 12)
    let sabotagesFixed = BitConverter.ToInt32(b, 16)
    let impostorKills = BitConverter.ToInt32(b, 20)
    let timesKilled = BitConverter.ToInt32(b, 24)
    let timesEjected = BitConverter.ToInt32(b, 28)
    let crewmateStreak = BitConverter.ToInt32(b, 32)
    let timesImpostor = BitConverter.ToInt32(b, 36)
    let timesCrewmate = BitConverter.ToInt32(b, 40)
    let gamesStarted = BitConverter.ToInt32(b, 44)
    let gamesFinished = BitConverter.ToInt32(b, 48)
    let crewmateVoteWins = BitConverter.ToInt32(b, 52)
    let crewmateTaskWins = BitConverter.ToInt32(b, 56)
    let impostorKillWins = BitConverter.ToInt32(b, 60)
    let impostorVoteWins = BitConverter.ToInt32(b, 64)
    let impostorSabotageWins = BitConverter.ToInt32(b, 68)
    
    {| BodiesReported = bodiesReported
       Emergencies = emergencies
       Tasks = tasks
       AllTasks = allTasks
       SabotagesFixed = sabotagesFixed
       ImpostorKills = impostorKills
       TimesKilled = timesKilled
       TimesEjected = timesEjected
       CrewmateStreak = crewmateStreak
       TimesImpostor = timesImpostor
       TimesCrewmate = timesCrewmate
       GamesStarted = gamesStarted
       GamesFinished = gamesFinished
       CrewmateVoteWins = crewmateVoteWins
       CrewmateTaskWins = crewmateTaskWins
       ImpostorVoteWins = impostorVoteWins
       ImpostorKillWins = impostorKillWins
       ImpostorSabotageWins = impostorSabotageWins |}
        
open Feliz.RoughViz

let update (msg: Msg) (state: State) =
    match msg with
    | StatsUploaded bytes ->
        let encodedStats = bytes |> App.StatsCompression.compress |> App.Base62.encode
        state, Navigation.newUrl (sprintf "#stats/%s" encodedStats)
    | DisplayStats encodedStats ->
        let bytes = encodedStats |> App.Base62.decode |> App.StatsCompression.decompress
        let res = processGameBytes bytes
        { state with
            BodiesReported = res.BodiesReported
            Emergencies = res.Emergencies
            Tasks = res.Tasks
            AllTasks = res.AllTasks
            SabotagesFixed = res.SabotagesFixed
            ImpostorKills = res.ImpostorKills
            TimesKilled = res.TimesKilled
            TimesEjected = res.TimesEjected
            CrewmateStreak = res.CrewmateStreak
            TimesImpostor = res.TimesImpostor
            TimesCrewmate = res.TimesCrewmate
            GamesStarted = res.GamesStarted
            GamesFinished = res.GamesFinished
            CrewmateVoteWins = res.CrewmateVoteWins
            CrewmateTaskWins = res.CrewmateTaskWins
            ImpostorVoteWins = res.ImpostorVoteWins
            ImpostorKillWins = res.ImpostorKillWins
            ImpostorSabotageWins = res.ImpostorSabotageWins}, Cmd.none

open Fable.Core.JsInterop
open Feliz.Bulma

[<Emit("new Uint8Array($0)")>]
let createUInt8Array(x: 'a) : byte[]  = jsNative

let copyToClipboard () =
    navigator.clipboard.Value.writeText "%UserProfile%\AppData\LocalLow\Innersloth\Among Us\playerStats2" |> ignore

let render (state: State) (dispatch: Msg -> unit) =
    let impostorTotalWins =
        state.ImpostorKillWins
        + state.ImpostorSabotageWins
        + state.ImpostorVoteWins
        
    let impostorTotalLoses =
        state.TimesImpostor - impostorTotalWins
        
    let impostorWinPercent = (impostorTotalWins |> float) / (state.TimesImpostor |> float) * 100.
    
    let crewmateWins = state.CrewmateTaskWins + state.CrewmateVoteWins
    let crewmateLoses = state.TimesCrewmate - crewmateWins
    
    let crewmateWinPercent = (crewmateWins |> float) / (state.TimesCrewmate |> float) * 100.
    
    // Required because roughviz will crash with zero elements for some reason
    let filterZeros data =
        data |> List.filter (fun (_:string, v) -> v <> 0.)
    
    let impostorWinsBreakdown = [
        ("Kill", state.ImpostorKillWins |> float)
        ("Sabotage", state.ImpostorSabotageWins |> float)
        ("Vote", state.ImpostorVoteWins |> float) ] |> filterZeros
    
    let impostorWinStats = [
        ("Win", impostorTotalWins |> float)
        ("Lose", impostorTotalLoses |> float) ] |> filterZeros
    
    let crewmateWinsBreakdown = [
        ("Task", state.CrewmateTaskWins |> float)
        ("Vote",  state.CrewmateVoteWins |> float) ]  |> filterZeros
    
    let crewmateWinStats = [
        ("Win", crewmateWins |> float)
        ("Lose", crewmateLoses |> float) ] |> filterZeros
    
    let kdRatio = (state.ImpostorKills |> float) / (state.TimesKilled |> float)
    
    let kds = [
        ("Kills", state.ImpostorKills |> float)
        ("Deaths", state.TimesKilled |> float) ] |> filterZeros
    
    let percentageComplete = (state.GamesFinished |> float) / (state.GamesStarted |> float) * 100.
    
    let percentageTimesImpostor = (state.TimesImpostor |> float) / (state.GamesStarted |> float) * 100.
    
    let susFactor = (state.TimesEjected |> float) / (state.ImpostorKills |> float) * 10.
    
    let tableLegend (data:(string*float) list) (colors: string array) =
        Html.table [
            prop.style [
                style.marginLeft length.auto
                style.marginRight length.auto
                style.fontFamily "gaeguregular"
                style.fontSize (length.rem 2)
                style.lineHeight (length.em 1.2)
            ]
            prop.children [
                Html.tableBody (data |> List.mapi (fun i (l,v) ->
                    Html.tableRow [
                        prop.style [
                            style.color (colors.[i % colors.Length])
                        ]
                        prop.children [
                            Html.tableCell [
                                prop.style [
                                    style.paddingRight (length.px 20)
                                ]
                                prop.text l
                            ]
                            Html.tableCell [
                                prop.text v
                            ]
                        ]
                    ]))
            ]
        ]
    let defaultChartColors = [| color.coral
                                color.skyBlue
                                "#66c2a5"
                                color.tan
                                "#8da0cb"
                                "#e78ac3"
                                "#a6d854"
                                "#ffd92f"
                                color.coral
                                color.skyBlue
                                color.tan
                                color.orange |]
    
    let impostorColors = [| color.cyan; color.hotPink; color.orange |]
    let crewmateColors = [| "#8df580"; color.skyBlue |]    
        
    let winLoseCharts() =
        Bulma.columns [
            Bulma.column [
                RoughViz.pieChart [
                    pieChart.colors impostorColors
                    pieChart.title (sprintf "Impostor Wins - %.2f%%" impostorWinPercent)
                    pieChart.data impostorWinStats
                    pieChart.roughness 2
                    //pieChart.highlight color.white
                    pieChart.height 350
                    pieChart.legend false
                ]
                tableLegend impostorWinStats impostorColors
            ]
            Bulma.column  [
                RoughViz.pieChart [
                    pieChart.colors crewmateColors
                    pieChart.title (sprintf "Crewmate Wins - %.2f%%" crewmateWinPercent)
                    pieChart.data crewmateWinStats
                    pieChart.roughness 2
                    //pieChart.highlight color.white
                    pieChart.height 350
                    pieChart.legend false
                ]
                tableLegend crewmateWinStats crewmateColors
            ]
        ]
    
    let killsDeaths () =
        Bulma.columns [
            Bulma.column [
                RoughViz.pieChart [
                    pieChart.title (sprintf "Kills/Deaths Ratio - %.2f" kdRatio)
                    pieChart.data kds
                    pieChart.roughness 2
                    //pieChart.highlight color.white
                    pieChart.height 350
                    pieChart.legend false
                ]
                tableLegend kds defaultChartColors
            ]
        ]
    
    let statsData =
        [
            ("Bodies Reported", state.BodiesReported |> float)
            ("Emergencies Called", state.Emergencies |> float)
            ("Tasks Completed", state.Tasks |> float)
            ("All Tasks Completed", state.AllTasks |> float)
            ("Sabotages Fixed", state.SabotagesFixed |> float)
            ("Impostor Kills", state.ImpostorKills |> float)
            ("Times Killed", state.TimesKilled |> float)
            ("Times Ejected", state.TimesEjected |> float)
            ("Crewmate Streak", state.CrewmateStreak |> float)
        ]
    
    let winsBreakdown() =
        Bulma.columns [
            Bulma.column [
                RoughViz.pieChart [
                    pieChart.colors impostorColors
                    pieChart.title "Impostor Wins Breakdown"
                    pieChart.data impostorWinsBreakdown
                    pieChart.roughness 2
                    //pieChart.highlight color.white
                    pieChart.height 350
                    pieChart.legend false
                ]
                tableLegend impostorWinsBreakdown impostorColors
            ]
            Bulma.column [
                RoughViz.pieChart [
                    pieChart.colors crewmateColors
                    pieChart.title "Crewmate Wins Breakdown"
                    pieChart.data crewmateWinsBreakdown
                    pieChart.roughness 2
                    //pieChart.highlight color.white
                    pieChart.height 350
                    pieChart.legend false
                ]
                tableLegend crewmateWinsBreakdown crewmateColors
            ]
        ]
        
    let timesImpostor () =
        Html.h2 [
            prop.style [
                style.fontFamily "gaeguregular"
                style.fontSize (length.rem 3)
                style.fontWeight.bold
                style.textAlign.center
            ]
            prop.children [
                Html.text "You have been an impostor "
                Html.span  [
                    prop.text (state.TimesImpostor)
                    prop.style [
                        style.color.hotPink
                    ]
                ]
                Html.text " times of "
                Html.span  [
                    prop.text (state.GamesStarted)
                    prop.style [
                        style.color.cyan
                    ]
                ]
                Html.text " ("
                Html.span  [
                    prop.text (sprintf "%.2f%%" percentageTimesImpostor)
                    prop.style [
                        style.color.hotPink
                    ]
                ]
                Html.text ")"
            ]
        ]
        
    let quitter () =
        Html.h2 [
            prop.style [
                style.fontFamily "gaeguregular"
                style.fontSize (length.rem 3)
                style.fontWeight.bold
                style.textAlign.center
            ]
            prop.children [
                Html.text "You have finished "
                Html.span  [
                    prop.text (state.GamesFinished)
                    prop.style [
                        style.color.cyan
                    ]
                ]
                Html.text " of "
                Html.span  [
                    prop.text (state.GamesStarted)
                    prop.style [
                        style.color.orange
                    ]
                ]
                Html.text " games ("
                Html.span  [
                    prop.text (sprintf "%.2f%%" percentageComplete)
                    prop.style [
                        style.color.cyan
                    ]
                ]
                Html.text ")"
            ]
        ]
    
    let susFactorPart () =
        let hoverText = "(Times Ejected / Number of Kills) x 10"
        let twitterText = encodeURIComponent (sprintf "My Among Us #susfactor is %.2f, check out the rest of my stats here!" susFactor)
        let escapedUrl = encodeURIComponent window.location.href
        let twitterShareHref = sprintf "https://twitter.com/intent/tweet/?text=%s&url=%s" twitterText escapedUrl
        Html.div [
            prop.children [
                Html.div [
                    prop.children [
                        Html.h2 [
                            prop.style [
                                style.fontFamily "gaeguregular"
                                style.fontSize (length.rem 3)
                                style.fontWeight.bold
                                style.textAlign.center
                            ]
                            prop.children [
                                Html.span [
                                    prop.text "sus factor: "
                                    tooltip.text hoverText
                                ]
                                Html.span [
                                    prop.text (sprintf "%.2f" susFactor)
                                    prop.style [
                                        style.color.hotPink
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Html.div [
                    prop.style [
                        style.textAlign.center
                    ]
                    prop.children[
                        Html.a [                    
                            prop.ariaLabel "Share on Twitter"
                            prop.className "resp-sharing-button__link"
                            prop.href twitterShareHref
                            prop.rel "noopener"
                            prop.target "_blank"
                            prop.children [
                                Html.div [
                                    prop.classes [ "resp-sharing-button"; "resp-sharing-button--twitter"; "resp-sharing-button--large" ]
                                    prop.children [
                                        Html.div [
                                            prop.ariaHidden true
                                            prop.classes [ "resp-sharing-button__icon"; "resp-sharing-button__icon--solid" ]
                                            prop.children [
                                                Html.svg [
                                                    prop.viewBox (0, 0, 24, 24)
                                                    prop.xmlns "http://www.w3.org/2000/svg"
                                                    prop.children [
                                                        Html.path [
                                                            prop.d "M23.44 4.83c-.8.37-1.5.38-2.22.02.93-.56.98-.96 1.32-2.02-.88.52-1.86.9-2.9 1.1-.82-.88-2-1.43-3.3-1.43-2.5 0-4.55 2.04-4.55 4.54 0 .36.03.7.1 1.04-3.77-.2-7.12-2-9.36-4.75-.4.67-.6 1.45-.6 2.3 0 1.56.8 2.95 2 3.77-.74-.03-1.44-.23-2.05-.57v.06c0 2.2 1.56 4.03 3.64 4.44-.67.2-1.37.2-2.06.08.58 1.8 2.26 3.12 4.25 3.16C5.78 18.1 3.37 18.74 1 18.46c2 1.3 4.4 2.04 6.97 2.04 8.35 0 12.92-6.92 12.92-12.93 0-.2 0-.4-.02-.6.9-.63 1.96-1.22 2.56-2.14z"
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                        Html.text "Share on Twitter"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
        
    let otherNumbers () =
        Html.div [
            Html.h2 [
                prop.style [
                    style.fontFamily "gaeguregular"
                    style.fontSize (length.rem 3)
                    style.fontWeight.bold
                    style.textAlign.center
                ]
                prop.text "Your other stats:"
            ]
            tableLegend statsData defaultChartColors
        ]
        
        
    
    
    let copyButton () =
        Html.button [
            prop.className "button is-boxed"
            
            prop.onClick (fun _ -> copyToClipboard() )
            prop.children [
                Html.span [
                    prop.className "icon"
                    prop.style [
                        style.marginRight (length.em 0.5)
                    ]
                    prop.children [
                        Html.i [
                            prop.classes [ "fas"; "fa-clone" ]
                        ]
                    ]
                ]
                Html.text "Step 1: Copy stats file path"
            ]
        ]
    
    let handleFile (file: Browser.Types.File) =
        async {
            try
                // TODO Validate file
                let! arrayBuffer = Async.AwaitPromise(file.slice(1,73)?arrayBuffer())
                let statsBytes = createUInt8Array arrayBuffer
                dispatch (StatsUploaded statsBytes)
            with
            | ex -> ()
        } |> Async.StartImmediate
        
    
    let upload () =
        Html.div [
            prop.className "file"
            prop.style [ style.justifyContent.center ]
            prop.children [
                Html.label [
                    prop.className "file-label"
                    prop.children [
                         Html.input [
                            prop.className "file-input"
                            prop.type' "file"
                            prop.name "resume"
                            prop.onChange handleFile
                         ]
                         Html.span [
                            prop.className "button"
                            prop.tabIndex 0
                            prop.children [
                                Html.span [
                                    prop.className "file-icon"
                                    prop.children [
                                        Html.i [
                                            prop.className "fas fa-upload"
                                        ]
                                    ]
                                ]
                                Html.span [
                                    prop.className "file-label"
                                    prop.text "Step 2: Load stats file"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    
    let welcomeMessage() =
        Html.article [
            prop.className ["message"]
            prop.children [
                Html.div [
                    prop.className "message-body"
                    prop.children [
                        Html.text "This web app lets you view and share your Among Us stats from Windows, unfortunately loading from mobile is not supported"
                        Html.br []
                        Html.text "Your stats are stored in the following location: "
                        Html.strong """%UserProfile%\AppData\LocalLow\Innersloth\Among Us\playerStats2"""
                        Html.br []
                        Html.strong "Step 1: "
                        Html.text "Click the left button to copy this path to your clipboard"
                        Html.br []
                        Html.strong "Step 2: "
                        Html.text "Click the right button, in the file selector paste in the location of the file and click select"
                        Html.br []
                        Html.strong "Step 3: "
                        Html.text "Profit"
                    ]
                ]
            ]
        ]
    
    Html.div [
        prop.className "app-container"
        prop.children [
            Html.div [
                prop.className "app-content"
                prop.children [
                    Html.div [ prop.id "stars" ]
                    Html.div [ prop.id "stars2" ]
                    Html.div [ prop.id "stars3" ]
                    Html.div [
                        prop.className "hero is-white"
                        prop.children [
                            Html.div [
                                prop.className "hero-body"
                                prop.children [
                                    Html.div [
                                        prop.className "container has-text-centered"
                                        prop.children [
                                            Html.h1 [
                                                prop.className "title"
                                                prop.style [
                                                    style.fontSize (length.rem 3)
                                                ]
                                                prop.children [
                                                    Html.text "Among Us Stats"
                                                ]
                                                
                                            ]
                                        ]
                                    ]
                                ]]
                            ]
                    ]
                    match state.Page with
                    | Home ->
                        Html.div [
                            prop.className ["container"; "pb-6"]
                            prop.children [welcomeMessage()]
                        ]
                        Html.div [
                            prop.className ["container"]
                            prop.children [
                                Bulma.columns [
                                    Bulma.column [
                                        prop.style [
                                            style.display.flex
                                            style.flexDirection.column
                                            style.alignItems.center
                                        ]
                                        prop.children [
                                            copyButton()
                                            Html.img [
                                                prop.style [
                                                    style.paddingTop (length.px 30)
                                                ]
                                                prop.src "/img/aucyan.png"
                                            ]
                                        ]
                                    ]
                                    Bulma.column [
                                        prop.style [
                                            style.display.flex
                                            style.flexDirection.column
                                            style.alignItems.center
                                        ]
                                        prop.children [
                                            upload()
                                            Html.img [
                                                prop.style [
                                                    style.paddingTop (length.px 30)
                                                ]
                                                prop.src "/img/auorange.png"
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    | GameStats _ ->
                        Bulma.container [
                            // todo this is just a hack for now
                            if state.GamesStarted > 0 then
                                Components.roughDivider()
                                susFactorPart()
                                Components.roughDivider()
                                winLoseCharts()
                                Components.roughDivider()
                                winsBreakdown()
                                Components.roughDivider()
                                timesImpostor()
                                Components.roughDivider()
                                killsDeaths()
                                Components.roughDivider()
                                quitter()
                                Components.roughDivider()
                                otherNumbers()
                                Components.roughDivider()
                        ]
                ]
            ]
            
            Html.div [
                prop.style [
                    style.display.flex
                    style.alignItems.center
                    style.justifyContent.center
                    style.width (length.px 124)
                    style.height (length.px 64)
                    style.backgroundColor.white
                    style.color "#2f313d"
                    style.borderRadius (length.px 12)
                    style.position.fixedRelativeToWindow
                    style.right (length.px 18) 
                    style.bottom (length.px 18)
                    style.boxShadow (0, 4, 8, color.rgba(0, 0, 0, 0.4))
                    style.zIndex 9999
                    style.fontWeight 600
                    style.transitionProperty "all 0.2s ease 0s"
                    style.border (length.px 1, borderStyle.solid, "#d2d2d2")
                ]
                
                prop.children [
                    Html.p [
                        prop.children [
                            Html.text "Built by "
                            Html.a [
                                prop.text "Stu"
                                prop.href "https://stu.dev"
                                prop.style [ style.color "#18a9cd" ]
                            ]
                        ]
                    ]
                ]
            ]
    ]]
