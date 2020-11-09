module Main

open Fable.Core.JsInterop

//importAll "../styles/main.scss"

open Elmish
open Elmish.React
open Elmish.Debug
open Elmish.UrlParser
open Elmish.HMR

// App
Program.mkProgram App.init App.update App.render
|> Program.toNavigable (parseHash App.pageParser) App.urlUpdate 
#if DEBUG
|> Program.withDebugger
#endif
|> Program.withReactSynchronous "feliz-app"
|> Program.run