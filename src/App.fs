module App

open System
open Browser
open Browser.Types
open Fable.Core
open Fable.Core.JS
open Feliz
open Elmish
open Fulma.Extensions.Wikiki

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
    
    let roughBarChart() =
        Html.div [
            prop.className "pie-chart-container"
            prop.style [
                style.display.flex
            ]
            prop.children [
                Html.div [
                    prop.style [
                        style.width (length.percent 50)
                    ]
                    prop.children [
                        RoughViz.pieChart [
                            pieChart.colors [| "cyan"; "hotpink"  |]
                            pieChart.title (sprintf "Impostor Wins - %.2f%%" impostorWinPercent)
                            pieChart.data impostorWinStats
                            pieChart.roughness 2
                            pieChart.highlight color.lightGreen
                            pieChart.height 350
                        ]
                    ]
                ]
                Html.div [
                    prop.style [
                        style.width (length.percent 50)
                    ]
                    prop.children [
                        RoughViz.pieChart [
                            pieChart.title (sprintf "Crewmate Wins - %.2f%%" crewmateWinPercent)
                            pieChart.data crewmateWinStats
                            pieChart.roughness 2
                            pieChart.highlight color.lightGreen
                            pieChart.height 350
                        ]
                    ]
                ]
            ]          
        ]
    
    let killsDeaths () =
        Html.div [
            prop.className "pie-chart-container"
            prop.style [
                //style.width (length.percent 50)                
            ]
            prop.children [
                RoughViz.pieChart [
                    pieChart.title (sprintf "Kills/Deaths Ratio - %.2f" kdRatio)
                    pieChart.data kds
                    pieChart.roughness 2
                    pieChart.highlight color.lightGreen
                    pieChart.height 350
                ]
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
            ("Times Impostor", state.TimesImpostor |> float)
            ("Times Crewmate", state.TimesCrewmate |> float)
            ("Games Started", state.GamesStarted |> float)
            ("Games Finished", state.GamesFinished |> float)
            ("Crewmate Vote Wins", state.CrewmateVoteWins |> float)
            ("Crewmate Task Wins", state.CrewmateTaskWins |> float)
            ("Impostor Vote Wins", state.ImpostorVoteWins |> float)
            ("Impostor Kill Wins", state.ImpostorKillWins |> float)
            ("Impostor Sabotage Wins", state.ImpostorSabotageWins |> float)
        ]
    
    let barChart =
        React.functionComponent(fun () ->
            RoughViz.horizontalBarChart [
                barChart.title "All Stats"
                barChart.data statsData
                barChart.roughness 1
                barChart.color color.lightBlue
                barChart.stroke color.lightCyan
                barChart.axisFontSize 18
                //barChart.fillStyle.crossHatch
                barChart.highlight color.lightGreen
            ])
        
    let winsBreakdown() =
        Html.div [
            prop.className "pie-chart-container"
            prop.style [
                style.display.flex
            ]
            prop.children [
                Html.div [
                    prop.style [
                        style.width (length.percent 50)
                    ]
                    prop.children [
                        RoughViz.pieChart [
                            pieChart.colors [| "cyan"; "hotpink"; "orange"  |]
                            pieChart.title "Impostor Wins Breakdown"
                            pieChart.data impostorWinsBreakdown
                            pieChart.roughness 2
                            pieChart.highlight color.lightGreen
                            pieChart.height 350
                        ]
                    ]
                ]
                Html.div [
                    prop.style [
                        style.width (length.percent 50)
                    ]
                    prop.children [
                        RoughViz.pieChart [
                            pieChart.title "Crewmate Wins Breakdown"
                            pieChart.data crewmateWinsBreakdown
                            pieChart.roughness 2
                            pieChart.highlight color.lightGreen
                            pieChart.height 350
                        ]
                    ]
                ]
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
                Html.span "You have been an impostor "
                Html.span  [
                    prop.text (state.TimesImpostor)
                    prop.style [
                        style.color.hotPink
                    ]
                ]
                Html.span " times of "
                Html.span  [
                    prop.text (state.GamesStarted)
                    prop.style [
                        style.color.cyan
                    ]
                ]
                Html.span " ("
                Html.span  [
                    prop.text (sprintf "%.2f%%" percentageTimesImpostor)
                    prop.style [
                        style.color.hotPink
                    ]
                ]
                Html.span ")"
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
                Html.span "You have finished "
                Html.span  [
                    prop.text (state.GamesFinished)
                    prop.style [
                        style.color.cyan
                    ]
                ]
                Html.span " of "
                Html.span  [
                    prop.text (state.GamesStarted)
                    prop.style [
                        style.color.orange
                    ]
                ]
                Html.span " games ("
                Html.span  [
                    prop.text (sprintf "%.2f%%" percentageComplete)
                    prop.style [
                        style.color.cyan
                    ]
                ]
                Html.span ")"
            ]
        ]
        
    let susFactor () =
        let hoverText = "(Times Ejected / Number of Kills) x 10"
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
                    prop.alt hoverText
                ]
                Html.span  [
                    prop.text (sprintf "%.2f" susFactor)
                    prop.alt hoverText
                    prop.style [
                        style.color.hotPink
                    ]
                ]
            ]
        ]
        
    let copyButton () =
        Html.button [
            prop.className "button is-boxed"
            prop.text "Step 1; Copy stats file path"
            prop.onClick (fun _ -> copyToClipboard() )
        ]
    
    let handleFile (ev: Browser.Types.Event) =
        try
            let reader = FileReader.Create()
            let file = ev.target?files?(0)
            reader.onload <- fun evt ->
                    let r = evt.target?result
                    let bytes = r |> createUInt8Array
                    // todo validate file
                    let statsBytes = bytes.[1..72]
                    dispatch (StatsUploaded statsBytes) 
                    ()
            reader.readAsArrayBuffer(file)
            ()
        with
        | ex -> ()
        
    
    let upload () =
        Html.div [
            prop.className "file is-boxed"
            prop.style [ style.justifyContent.center ]
            prop.children [
                Html.label [
                    prop.className "file-label"
                    prop.children [
                         Html.input [
                            prop.className "file-input"
                            prop.type' "file"
                            prop.name "resume"
                            prop.onChange (fun ev -> handleFile ev)
                         ]
                         Html.span [
                            prop.className "file-cta"
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
                                    prop.text "Step 2; Load stats file"
                                ]
                            ]
                        ]
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
                                    Html.a [
                                        prop.href "https://www.buymeacoffee.com/slang"
                                        prop.target.blank
                                        prop.style [
                                            style.float'.right
                                        ]
                                        prop.children [
                                            Html.img [
                                                prop.src "https://cdn.buymeacoffee.com/buttons/v2/default-green.png"
                                                prop.alt "Buy Me A Coffee"
                                                prop.style [
                                                    style.height (length.px 40)
                                                    style.width (length.px 144)
                                                ]
                                            ]
                                        ]
                                    ]
                                    Html.div [
                                        prop.className "container has-text-centered"
                                        prop.children [
                                            Html.h1 [
                                                prop.className "title"
                                                prop.style [
                                                    style.fontSize (length.rem 3)
                                                ]
                                                prop.text "Among Us Stats"
                                            ]
                                        ]
                                    ]
                                ]]
                            ]
                    ]
                    match state.Page with
                    | Home ->
                        Html.div [
                            prop.style [
                                style.display.flex
                            ]
                            prop.children [
                                Html.div [
                                    prop.style [
                                        style.textAlign.center
                                        style.width (length.percent 50)
                                    ]
                                    prop.children [
                                        Html.div [
                                            prop.style [
                                                style.width (length.percent 100)
                                            ]
                                            prop.children [
                                                copyButton()
                                            ]
                                        ]
                                        Html.img [
                                            prop.style [
                                                style.paddingTop (length.px 30)
                                            ]
                                            prop.src "/img/aucyan.png"
                                        ]
                                    ]
                                ]
                                Html.div [
                                    prop.style [
                                        style.textAlign.center
                                        style.width (length.percent 50)
                                    ]
                                    prop.children [
                                        Html.div [
                                            prop.style [
                                                style.width (length.percent 100)
                                            ]
                                            prop.children [
                                                upload()
                                            ]
                                        ]
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
                    | GameStats _ ->
                        // todo this is just a hack for now
                        if state.GamesStarted > 0 then
                            roughBarChart()
                            Divider.divider []
                            winsBreakdown()
                            Divider.divider []
                            susFactor()
                            Divider.divider []
                            timesImpostor()
                            Divider.divider []
                            killsDeaths()
                            Divider.divider []
                            quitter()
                            Divider.divider []
                ]
            ]
            Html.footer [
                Html.p [
                    prop.children [
                        Html.span "Built by "
                        Html.a [
                            prop.text "Stu"
                            prop.href "https://stu.dev"
                        ]
                    ]
                ]
            ]
    ]]
