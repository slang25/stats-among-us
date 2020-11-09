module App.Base62
open System
open System.Numerics
open System.Text

let toBase62 (d: bigint) =
    let d = int d
    match d with
    | v when v < 10 -> (char)(int '0' + d)
    | v when v < 36 -> (char)(int 'A' + d - 10)
    | v when v < 62 -> (char)(int 'a' + d - 36)
    | _ -> failwith "Cannot encode digit 'd' to base 62."
    
let fromBase62 (c: char) =
    match c with
    | v when c >= 'a' && v <= 'z' -> 36 + int c - int 'a'
    | v when c >= 'A' && v <= 'Z' -> 10 + int c - int 'A'
    | v when c >= '0' && v <= '9' -> int c - int '0'
    | _ ->  failwith "Cannot decode char 'c' from base 62."
    |> bigint

let encode (input:byte[]) =
    let b = bigint input
    let sb = StringBuilder()
    let rec convert (current:bigint) (sb:StringBuilder) =
        if current = 0I then () else
        let (div,rem) = BigInteger.DivRem(current, 62I)
        sb.Append(rem |> toBase62) |> ignore
        convert div sb
    convert b sb
    sb.ToString() |> Seq.rev |> String.Concat

let decode (input:string) =
    let b:bigint =
        input
        |> Seq.fold (fun (current:bigint) c -> current * 62I + (fromBase62 c)) bigint.Zero
    b.ToByteArray()