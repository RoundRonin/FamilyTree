﻿let draggingOn = false; // Initial value
let editMode = false; // Initial value

let blazorClickHandlerReference = null;

function registerBlazorClickHandler(blazorClickHandler) {
    blazorClickHandlerReference = blazorClickHandler;
    console.log("Blazor click handler registered");
}

function setDraggingOn(value) {
    draggingOn = value;
}
function setEditMode(value) {
    editMode = value;
}

let currentNodes = [];
let currentLinks = [];
let simulation; // Define simulation outside of renderD3Graph

function updateGraph(newNodes, newLinks) {
    console.log("Updating graph with new nodes and links...");
    console.log("New Nodes:", newNodes);
    console.log("New Links:", newLinks);

    if (!simulation) {
        console.error("Simulation is not initialized. Make sure renderD3Graph is called first.");
        return;
    }

    // Add new nodes
    newNodes.forEach(node => {
        if (!currentNodes.find(n => n.id === node.id)) {
            currentNodes.push(node);
        }
    });

    // Add new links, checking that both source and target nodes exist
    newLinks.forEach(link => {
        if (currentNodes.find(n => n.id === link.source)
            && currentNodes.find(n => n.id === link.target)
            && !currentLinks.find(l => l.sourse === link.source && l.target === link.target)) {
            currentLinks.push(link);
        } else {
            console.error(`Node not found: ${!currentNodes.find(n => n.id === link.source) ? link.source : link.target}`);
        }
    });

    console.log("Current Nodes after update:", currentNodes);
    console.log("Current Links after update:", currentLinks);

    // Recalculate the simulation with the new data
    simulation.nodes(currentNodes);
    simulation.force("link").links(currentLinks);
    simulation.alpha(1).restart();
}

async function renderD3Graph(nodes, links, componentName, containerWidth) {
    console.log("Rendering D3 Graph...");
    console.log("Initial Nodes:", nodes);
    console.log("Initial Links:", links);

    currentNodes = [...nodes];
    currentLinks = [...links];

    const container = d3.select("#tree-container");
    const width = container.node().getBoundingClientRect().width || window.innerWidth;
    const height = container.node().getBoundingClientRect().height || window.innerHeight;
    const levelHeight = height / 5;

    const multiplier = 3;

    container.selectAll("*").remove(); // Clear the container to avoid duplicates

    const svg = container.append("svg")
        .attr("width", "100%")
        .attr("height", "100%")
        .attr("viewBox", `0 0 ${width} ${height}`)
        .call(d3.zoom().on("zoom", (event) => {
            svg.attr("transform", event.transform);
        }));

    const linkGroup = svg.append("g").attr("class", "links"); // Group for links
    const nodeGroup = svg.append("g").attr("class", "nodes"); // Group for nodes

    simulation = d3.forceSimulation(currentNodes) // Assign to the global variable
        .force("link", d3.forceLink(currentLinks).id(d => d.id)
            .distance(d => {
                if (d.relationshipType === 1) return 50 * multiplier;
                if (d.relationshipType === 0) return 100 * multiplier;
                return 200 * multiplier;
            })
            .strength(d => {
                if (d.relationshipType === 1) return 1;
                if (d.relationshipType === 0) return 0.7;
                return 0.3;
            })
        )
        .force("charge", d3.forceManyBody().strength(50))
        .force("collide", d3.forceCollide().radius(containerWidth / 2 + 20).strength(1))
        .force("y", d3.forceY(d => (d.treeDepth + 1) * levelHeight).strength(3))
        .force("x", d3.forceX(width / 2).strength(0.1))
        .on("tick", ticked);

    function ticked() {
        const link = linkGroup.selectAll('line') // Use linkGroup for links
            .data(currentLinks)
            .join('line')
            .attr('stroke', '#acacac')
            .attr("stroke-width", 5);

        const node = nodeGroup.selectAll('foreignObject') // Use nodeGroup for nodes
            .data(currentNodes)
            .join(
                enter => enter.append('foreignObject')
                    .attr('width', containerWidth)
                    .attr('height', 120)
                    .attr('x', d => d.x - containerWidth / 2)
                    .attr('y', d => d.y - 60)
                    .html(d => `<div id="person-card-container-${d.id}" class="blazor-component" onclick="invokeBlazorClick('${d.id}')"></div>`)
                    .call(d3.drag()
                        .filter(() => draggingOn)
                        .on("start", dragstarted)
                        .on("drag", dragged)
                        .on("end", dragended)), // Conditionally apply drag behavior
                update => update
                    .attr('x', d => d.x - containerWidth / 2)
                    .attr('y', d => d.y - 60)
            );

        link.attr('x1', d => d.source.x)
            .attr('y1', d => d.source.y)
            .attr('x2', d => d.target.x)
            .attr('y2', d => d.target.y);

        currentNodes.forEach(node => {
            let container = document.getElementById(`person-card-container-${node.id}`);
            if (!container.childElementCount) {
                const parameters = { Person: node.person };
                Blazor.rootComponents.add(container, componentName, parameters)
                    .then(() => console.log(`Component inserted for ${node.name}`))
                    .catch(err => console.error(`Error inserting component for ${node.name}: ${err}`));
            }
        });
    }

    function dragstarted(event, d) {
        if (!event.active) simulation.alphaTarget(0.3).restart();
        d.fx = d.x;
        d.fy = d.y;
    }

    function dragged(event, d) {
        d.fx = event.x;
        d.fy = event.y;
    }

    function dragended(event, d) {
        if (!event.active) simulation.alphaTarget(0);
        d.fx = null;
        d.fy = null;
    }
}


function invokeBlazorClick(personId) {
    console.log(`Clicked on person with id: ${personId}`);
    if (!draggingOn && !editMode) {
        console.log("Passing the click to dotNet");
        //Blazor.BlazorClickHandler(personId);
        blazorClickHandlerReference.invokeMethodAsync('BlazorClickHandler', personId)
            .then(() => console.log("BlazorClickHandler invoked successfully"))
            .catch(err => console.error("Error invoking BlazorClickHandler:", err));
    } else {
        console.log("Turn dragging off to click");
    }
}
