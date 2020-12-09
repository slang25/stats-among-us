namespace BitConverterExtensions

module BitConverter = 
    let pow2 y = 1 <<< y
    // convert booleans to bytes in a space efficient way
    let FromBooleans (bools:bool []) =
        seq {
            let b = ref 0uy
            for i=0 to bools.Length-1 do
                let rem = (i  % 8)
                if rem = 0 && i<> 0 then 
                    yield !b
                    b := 0uy
                if bools.[i] then
                    b := !b + (byte (pow2 rem))
            yield !b
        } |> Array.ofSeq
    // to booleans only works for bytes created with FromBooleans
    let ToBooleans (bytes:byte []) = 
        bytes
        |> Array.map (fun b -> Array.init 8 (fun i -> ((pow2 i) &&& int b) > 0))
        |> Array.concat