module App.StatsCompression
open App.BitConverterExtensions

let compress input =
        let nonEmptyByteMap = input |> Array.map (fun b -> b <> 0uy) |> BitConverter.FromBooleans
        let compressed = [| [| byte input.Length |] ;nonEmptyByteMap; input |> Array.filter (fun b -> b <> 0uy) |] |> Array.concat
        compressed
    
let decompress (input:byte[]) =
    let targetLength = int input.[0]
    let mapLength = targetLength/8
    let nonEmptyBytes = input.[1..mapLength] |> BitConverter.ToBooleans
    let result =
        let mutable compressedByteIndex = 0
        seq { 0 .. targetLength-1 }
        |> Seq.map (fun i ->
                if nonEmptyBytes.[i] then
                    let b = input.[1 + mapLength + compressedByteIndex]
                    compressedByteIndex <- compressedByteIndex+1
                    b
                else 0uy
        )
        |> Array.ofSeq
    result
