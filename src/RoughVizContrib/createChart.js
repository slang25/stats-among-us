import RoughViz from 'rough-viz/dist/roughviz.min'

export const createBarChart = (config) => new RoughViz.Bar(config)

export const createPieChart = (config) => new RoughViz.Pie(config)

export const createHorizontalBarChart = (config) => new RoughViz.BarH(config)