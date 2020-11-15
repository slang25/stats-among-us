import rough from 'roughjs/bundled/rough.esm.js'

export const createCanvas = (element, config) => rough.canvas(element, config)
export const newSeed = () => rough.newSeed()