module rec Rough

open Fable.Core
open Browser.Types

let createCanvas (element: HTMLCanvasElement) : Rough.RoughCanvas = JsInterop.import "createCanvas" "./createRough.js"
let newSeed () : float = JsInterop.import "newSeed" "./createRough.js"

module Canvas =
    type Config = Core.Config
    type Options = Core.Options
    type ResolvedOptions = Core.ResolvedOptions
    type Drawable = Core.Drawable
    type Point = Geometry.Point

    type [<AllowNullLiteral>] RoughCanvas =
        abstract draw: drawable: Drawable -> unit
        abstract getDefaultOptions: unit -> ResolvedOptions
        abstract line: x1: float * y1: float * x2: float * y2: float * ?options: Options -> Drawable
        abstract rectangle: x: float * y: float * width: float * height: float * ?options: Options -> Drawable
        abstract ellipse: x: float * y: float * width: float * height: float * ?options: Options -> Drawable
        abstract circle: x: float * y: float * diameter: float * ?options: Options -> Drawable
        abstract linearPath: points: ResizeArray<Point> * ?options: Options -> Drawable
        abstract polygon: points: ResizeArray<Point> * ?options: Options -> Drawable
        abstract arc: x: float * y: float * width: float * height: float * start: float * stop: float * ?closed: bool * ?options: Options -> Drawable
        abstract curve: points: ResizeArray<Point> * ?options: Options -> Drawable
        abstract path: d: string * ?options: Options -> Drawable

module Core =
    type Point = Geometry.Point
    type Random = Math.Random

    type [<AllowNullLiteral>] IExports =
        abstract SVGNS: obj

    type [<AllowNullLiteral>] Config =
        abstract options: Options option with get, set

    type [<AllowNullLiteral>] DrawingSurface =
        abstract width: U2<float, SVGAnimatedLength> with get, set
        abstract height: U2<float, SVGAnimatedLength> with get, set

    type [<AllowNullLiteral>] Options =
        abstract maxRandomnessOffset: float option with get, set
        abstract roughness: float option with get, set
        abstract bowing: float option with get, set
        abstract stroke: string option with get, set
        abstract strokeWidth: float option with get, set
        abstract curveFitting: float option with get, set
        abstract curveTightness: float option with get, set
        abstract curveStepCount: float option with get, set
        abstract fill: string option with get, set
        abstract fillStyle: string option with get, set
        abstract fillWeight: float option with get, set
        abstract hachureAngle: float option with get, set
        abstract hachureGap: float option with get, set
        abstract simplification: float option with get, set
        abstract dashOffset: float option with get, set
        abstract dashGap: float option with get, set
        abstract zigzagOffset: float option with get, set
        abstract seed: float option with get, set
        abstract combineNestedSvgPaths: bool option with get, set
        abstract strokeLineDash: ResizeArray<float> option with get, set
        abstract strokeLineDashOffset: float option with get, set
        abstract fillLineDash: ResizeArray<float> option with get, set
        abstract fillLineDashOffset: float option with get, set
        abstract disableMultiStroke: bool option with get, set
        abstract disableMultiStrokeFill: bool option with get, set

    type [<AllowNullLiteral>] ResolvedOptions =
        inherit Options
        abstract maxRandomnessOffset: float with get, set
        abstract roughness: float with get, set
        abstract bowing: float with get, set
        abstract stroke: string with get, set
        abstract strokeWidth: float with get, set
        abstract curveFitting: float with get, set
        abstract curveTightness: float with get, set
        abstract curveStepCount: float with get, set
        abstract fillStyle: string with get, set
        abstract fillWeight: float with get, set
        abstract hachureAngle: float with get, set
        abstract hachureGap: float with get, set
        abstract dashOffset: float with get, set
        abstract dashGap: float with get, set
        abstract zigzagOffset: float with get, set
        abstract seed: float with get, set
        abstract combineNestedSvgPaths: bool with get, set
        abstract randomizer: Random option with get, set
        abstract disableMultiStroke: bool with get, set
        abstract disableMultiStrokeFill: bool with get, set

    type [<StringEnum>] [<RequireQualifiedAccess>] OpType =
        | Move
        | BcurveTo
        | LineTo

    type [<StringEnum>] [<RequireQualifiedAccess>] OpSetType =
        | Path
        | FillPath
        | FillSketch

    type [<AllowNullLiteral>] Op =
        abstract op: OpType with get, set
        abstract data: ResizeArray<float> with get, set

    type [<AllowNullLiteral>] OpSet =
        abstract ``type``: OpSetType with get, set
        abstract ops: ResizeArray<Op> with get, set
        abstract size: Point option with get, set
        abstract path: string option with get, set

    type [<AllowNullLiteral>] Drawable =
        abstract shape: string with get, set
        abstract options: ResolvedOptions with get, set
        abstract sets: ResizeArray<OpSet> with get, set

    type [<AllowNullLiteral>] PathInfo =
        abstract d: string with get, set
        abstract stroke: string with get, set
        abstract strokeWidth: float with get, set
        abstract fill: string option with get, set

module Generator =
    type Config = Core.Config
    type Options = Core.Options
    type Drawable = Core.Drawable
    type OpSet = Core.OpSet
    type ResolvedOptions = Core.ResolvedOptions
    type PathInfo = Core.PathInfo
    type Point = Geometry.Point

    type [<AllowNullLiteral>] IExports =
        abstract RoughGenerator: RoughGeneratorStatic

    type [<AllowNullLiteral>] RoughGenerator =
        abstract defaultOptions: ResolvedOptions with get, set
        abstract line: x1: float * y1: float * x2: float * y2: float * ?options: Options -> Drawable
        abstract rectangle: x: float * y: float * width: float * height: float * ?options: Options -> Drawable
        abstract ellipse: x: float * y: float * width: float * height: float * ?options: Options -> Drawable
        abstract circle: x: float * y: float * diameter: float * ?options: Options -> Drawable
        abstract linearPath: points: ResizeArray<Point> * ?options: Options -> Drawable
        abstract arc: x: float * y: float * width: float * height: float * start: float * stop: float * ?closed: bool * ?options: Options -> Drawable
        abstract curve: points: ResizeArray<Point> * ?options: Options -> Drawable
        abstract polygon: points: ResizeArray<Point> * ?options: Options -> Drawable
        abstract path: d: string * ?options: Options -> Drawable
        abstract opsToPath: drawing: OpSet -> string
        abstract toPaths: drawable: Drawable -> ResizeArray<PathInfo>

    type [<AllowNullLiteral>] RoughGeneratorStatic =
        [<Emit "new $0($1...)">] abstract Create: ?config: Config -> RoughGenerator
        abstract newSeed: unit -> float

module Geometry =

    type [<AllowNullLiteral>] IExports =
        abstract rotatePoints: points: ResizeArray<Point> * center: Point * degrees: float -> unit
        abstract rotateLines: lines: ResizeArray<Line> * center: Point * degrees: float -> unit
        abstract lineLength: line: Line -> float
        abstract lineIntersection: a: Point * b: Point * c: Point * d: Point -> Point option
        abstract isPointInPolygon: points: ResizeArray<Point> * x: float * y: float -> bool
        abstract doIntersect: p1: Point * q1: Point * p2: Point * q2: Point -> bool

    type Point =
        float * float

    type Line =
        Point * Point

    type [<AllowNullLiteral>] Rectangle =
        abstract x: float with get, set
        abstract y: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set

module Math =

    type [<AllowNullLiteral>] IExports =
        abstract randomSeed: unit -> float
        abstract Random: RandomStatic

    type [<AllowNullLiteral>] Random =
        abstract next: unit -> float

    type [<AllowNullLiteral>] RandomStatic =
        [<Emit "new $0($1...)">] abstract Create: seed: float -> Random

module Renderer =
    type ResolvedOptions = Core.ResolvedOptions
    type Op = Core.Op
    type OpSet = Core.OpSet
    type Point = Geometry.Point

    type [<AllowNullLiteral>] IExports =
        abstract line: x1: float * y1: float * x2: float * y2: float * o: ResolvedOptions -> OpSet
        abstract linearPath: points: ResizeArray<Point> * close: bool * o: ResolvedOptions -> OpSet
        abstract polygon: points: ResizeArray<Point> * o: ResolvedOptions -> OpSet
        abstract rectangle: x: float * y: float * width: float * height: float * o: ResolvedOptions -> OpSet
        abstract curve: points: ResizeArray<Point> * o: ResolvedOptions -> OpSet
        abstract ellipse: x: float * y: float * width: float * height: float * o: ResolvedOptions -> OpSet
        abstract generateEllipseParams: width: float * height: float * o: ResolvedOptions -> EllipseParams
        abstract ellipseWithParams: x: float * y: float * o: ResolvedOptions * ellipseParams: EllipseParams -> EllipseResult
        abstract arc: x: float * y: float * width: float * height: float * start: float * stop: float * closed: bool * roughClosure: bool * o: ResolvedOptions -> OpSet
        abstract svgPath: path: string * o: ResolvedOptions -> OpSet
        abstract solidFillPolygon: points: ResizeArray<Point> * o: ResolvedOptions -> OpSet
        abstract patternFillPolygon: points: ResizeArray<Point> * o: ResolvedOptions -> OpSet
        abstract patternFillArc: x: float * y: float * width: float * height: float * start: float * stop: float * o: ResolvedOptions -> OpSet
        abstract randOffset: x: float * o: ResolvedOptions -> float
        abstract randOffsetWithRange: min: float * max: float * o: ResolvedOptions -> float
        abstract doubleLineFillOps: x1: float * y1: float * x2: float * y2: float * o: ResolvedOptions -> ResizeArray<Op>

    type [<AllowNullLiteral>] EllipseParams =
        abstract rx: float with get, set
        abstract ry: float with get, set
        abstract increment: float with get, set

    type [<AllowNullLiteral>] EllipseResult =
        abstract opset: OpSet with get, set
        abstract estimatedPoints: ResizeArray<Point> with get, set

module Rough =
    type Config = Core.Config
    type RoughCanvas = Canvas.RoughCanvas
    type RoughGenerator = Generator.RoughGenerator
    type RoughSVG = Svg.RoughSVG

    type [<AllowNullLiteral>] IExports =
        abstract canvas: canvas: HTMLCanvasElement * ?config: Config -> RoughCanvas
        abstract svg: svg: SVGSVGElement * ?config: Config -> RoughSVG
        abstract generator: ?config: Config -> RoughGenerator
        abstract newSeed: unit -> float

module Svg =
    type Config = Core.Config
    type Options = Core.Options
    type OpSet = Core.OpSet
    type ResolvedOptions = Core.ResolvedOptions
    type Drawable = Core.Drawable
    type RoughGenerator = Generator.RoughGenerator
    type Point = Geometry.Point

    type [<AllowNullLiteral>] IExports =
        abstract RoughSVG: RoughSVGStatic

    type [<AllowNullLiteral>] RoughSVG =
        abstract draw: drawable: Drawable -> SVGGElement
        abstract getDefaultOptions: unit -> ResolvedOptions
        abstract opsToPath: drawing: OpSet -> string
        abstract line: x1: float * y1: float * x2: float * y2: float * ?options: Options -> SVGGElement
        abstract rectangle: x: float * y: float * width: float * height: float * ?options: Options -> SVGGElement
        abstract ellipse: x: float * y: float * width: float * height: float * ?options: Options -> SVGGElement
        abstract circle: x: float * y: float * diameter: float * ?options: Options -> SVGGElement
        abstract linearPath: points: ResizeArray<Point> * ?options: Options -> SVGGElement
        abstract polygon: points: ResizeArray<Point> * ?options: Options -> SVGGElement
        abstract arc: x: float * y: float * width: float * height: float * start: float * stop: float * ?closed: bool * ?options: Options -> SVGGElement
        abstract curve: points: ResizeArray<Point> * ?options: Options -> SVGGElement
        abstract path: d: string * ?options: Options -> SVGGElement

    type [<AllowNullLiteral>] RoughSVGStatic =
        [<Emit "new $0($1...)">] abstract Create: svg: SVGSVGElement * ?config: Config -> RoughSVG
