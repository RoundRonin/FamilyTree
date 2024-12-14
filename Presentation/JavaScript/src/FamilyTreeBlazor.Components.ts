import * as d3 from 'd3';

export function renderTree(data: any, elementId: string) {
    const svg = d3.select(`#${elementId}`).append("svg")
        .attr("width", 600)
        .attr("height", 400);

    const root = d3.hierarchy(data);
    const treeLayout = d3.tree().size([400, 200]);
    treeLayout(root);

    const nodes = svg.append("g")
        .selectAll("g")
        .data(root.descendants())
        .enter().append("g")
        .attr("transform", d => `translate(${d.y},${d.x})`);

    nodes.append("circle").attr("r", 5);
    nodes.append("text").attr("dy", 3).attr("x", d => d.children ? -8 : 8)
        .style("text-anchor", d => d.children ? "end" : "start")
        .text(d => d.data.name);
}
