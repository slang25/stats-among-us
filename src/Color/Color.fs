module rec Color
open System
open Fable.Core

let [<Import("*","color")>] Color: ColorConstructorStatic = jsNative

type [<AllowNullLiteral>] IExports =
    abstract ColorConstructor: ColorConstructorStatic

type ColorParam =
    U5<Color, string, ResizeArray<float>, float, obj option>

type Color =
    Color<obj>

type [<AllowNullLiteral>] Color<'T> =
    abstract toString: unit -> string
    abstract toJSON: unit -> Color<'T>
    abstract string: ?places: float -> string
    abstract percentString: ?places: float -> string
    abstract array: unit -> ResizeArray<float>
    abstract ``object``: unit -> obj
    abstract unitArray: unit -> ResizeArray<float>
    abstract unitObject: unit -> ColorUnitObjectReturn
    abstract round: ?places: float -> Color
    abstract alpha: unit -> float
    abstract alpha: ``val``: float -> Color
    abstract red: unit -> float
    abstract red: ``val``: float -> Color
    abstract green: unit -> float
    abstract green: ``val``: float -> Color
    abstract blue: unit -> float
    abstract blue: ``val``: float -> Color
    abstract hue: unit -> float
    abstract hue: ``val``: float -> Color
    abstract saturationl: unit -> float
    abstract saturationl: ``val``: float -> Color
    abstract lightness: unit -> float
    abstract lightness: ``val``: float -> Color
    abstract saturationv: unit -> float
    abstract saturationv: ``val``: float -> Color
    abstract value: unit -> float
    abstract value: ``val``: float -> Color
    abstract chroma: unit -> float
    abstract chroma: ``val``: float -> Color
    abstract gray: unit -> float
    abstract gray: ``val``: float -> Color
    abstract white: unit -> float
    abstract white: ``val``: float -> Color
    abstract wblack: unit -> float
    abstract wblack: ``val``: float -> Color
    abstract cyan: unit -> float
    abstract cyan: ``val``: float -> Color
    abstract magenta: unit -> float
    abstract magenta: ``val``: float -> Color
    abstract yellow: unit -> float
    abstract yellow: ``val``: float -> Color
    abstract black: unit -> float
    abstract black: ``val``: float -> Color
    abstract x: unit -> float
    abstract x: ``val``: float -> Color
    abstract y: unit -> float
    abstract y: ``val``: float -> Color
    abstract z: unit -> float
    abstract z: ``val``: float -> Color
    abstract l: unit -> float
    abstract l: ``val``: float -> Color
    abstract a: unit -> float
    abstract a: ``val``: float -> Color
    abstract b: unit -> float
    abstract b: ``val``: float -> Color
    abstract keyword: unit -> string
    abstract keyword: ``val``: 'V -> Color<'V>
    abstract hex: unit -> string
    abstract hex: ``val``: 'V -> Color<'V>
    abstract rgbNumber: unit -> float
    abstract luminosity: unit -> float
    abstract contrast: color2: Color -> float
    abstract level: color2: Color -> ColorLevelReturn
    abstract isDark: unit -> bool
    abstract isLight: unit -> bool
    abstract negate: unit -> Color
    abstract lighten: ratio: float -> Color
    abstract darken: ratio: float -> Color
    abstract saturate: ratio: float -> Color
    abstract desaturate: ratio: float -> Color
    abstract whiten: ratio: float -> Color
    abstract blacken: ratio: float -> Color
    abstract grayscale: unit -> Color
    abstract fade: ratio: float -> Color
    abstract opaquer: ratio: float -> Color
    abstract rotate: degrees: float -> Color
    abstract mix: mixinColor: Color * ?weight: float -> Color
    abstract rgb: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract hsl: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract hsv: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract hwb: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract cmyk: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract xyz: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract lab: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract lch: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract ansi16: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract ansi256: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract hcg: [<ParamArray>] args: ResizeArray<float> -> Color
    abstract apple: [<ParamArray>] args: ResizeArray<float> -> Color

type [<AllowNullLiteral>] ColorUnitObjectReturn =
    abstract r: float with get, set
    abstract g: float with get, set
    abstract b: float with get, set
    abstract alpha: float option with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] ColorLevelReturn =
    | [<CompiledName "AAA">] AAA
    | [<CompiledName "AA">] AA
    | [<CompiledName "">] Empty

type [<AllowNullLiteral>] ColorConstructor =
    [<Emit "$0($1...)">] abstract Invoke: ?obj: 'T * ?model: obj -> Color<'T>
    abstract rgb: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract rgb: color: ColorParam -> Color
    abstract hsl: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract hsl: color: ColorParam -> Color
    abstract hsv: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract hsv: color: ColorParam -> Color
    abstract hwb: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract hwb: color: ColorParam -> Color
    abstract cmyk: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract cmyk: color: ColorParam -> Color
    abstract xyz: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract xyz: color: ColorParam -> Color
    abstract lab: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract lab: color: ColorParam -> Color
    abstract lch: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract lch: color: ColorParam -> Color
    abstract ansi16: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract ansi16: color: ColorParam -> Color
    abstract ansi256: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract ansi256: color: ColorParam -> Color
    abstract hcg: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract hcg: color: ColorParam -> Color
    abstract apple: [<ParamArray>] ``val``: ResizeArray<float> -> Color
    abstract apple: color: ColorParam -> Color

type [<AllowNullLiteral>] ColorConstructorStatic =
    [<Emit "new $0($1...)">] abstract Create: ?obj: 'T * ?model: obj -> Color
