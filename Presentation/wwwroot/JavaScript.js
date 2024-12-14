// Logger Interface (Abstract Class)
class LoggerInterface {
    log(message, explanation) {
        throw new Error("Method 'log()' must be implemented.");
    }

    error(message, explanation) {
        throw new Error("Method 'log()' must be implemented.");
    }

    turnOn() {
        throw new Error("Method 'turnOn()' must be implemented.");
    }

    turnOff() {
        throw new Error("Method 'turnOff()' must be implemented.");
    }
}

// ConsoleLogger Implementation
class ConsoleLogger extends LoggerInterface {
    constructor() {
        super();
        this.isEnabled = true;
    }

    log(message, explanation = "") {
        if (this.isEnabled) {
            console.log(message, explanation);
        }
    }

    error(message, explanation = "") {
        if (this.isEnabled) {
            console.error(message, explanation);
        }
    }

    turnOn() {
        this.isEnabled = true;
    }

    turnOff() {
        this.isEnabled = false;
    }
}
class D3GraphManager {
    constructor(logger, componentName, placeholderName) {
        this.logger = logger || new ConsoleLogger(); // Default to ConsoleLogger if no logger provided
        this.draggingOn = false; // Initial value
        this.editMode = false; // Initial value
        this.blazorClickHandlerReference = null;

        this.currentNodes = [];
        this.currentLinks = [];
        this.parentNodeId = null;

        this.svgWidth;
        this.svgHeight;
        this.nodeGroup;
        this.linkGroup;
        this.simulation; // Define simulation outside of renderD3Graph

        this.placeholder = false;
        this.containerWidth = 180;
        this.componentName = componentName;
        this.placeholderName = placeholderName;

        this.placeholderNode = {
            x: 0,
            y: 0,
            id: 0,
            treeDepth: 0,
            componentName: this.placeholderName
        };
    }

    // Public Methods
    registerBlazorClickHandler(blazorClickHandler) {
        this.blazorClickHandlerReference = blazorClickHandler;
        this.logger.log("Blazor click handler registered");
    }

    setDraggingOn(value) {
        this.draggingOn = value;
    }

    setEditMode(value) {
        this.editMode = value;
    }

    async renderD3Graph(nodes, links, containerWidth) {
        this.logger.log("Rendering D3 Graph...");
        this.logger.log("Initial Nodes:", nodes);
        this.logger.log("Initial Links:", links);

        // Add the name
        this.currentNodes = nodes.map(n => ({
            ...n,
            componentName: this.componentName
        }));
        this.currentLinks = [...links];
        this.containerWidth = containerWidth;

        this._initializeGraph();
    }

    updateGraph(newNodes, newLinks) {
        this.logger.log("Updating graph with new nodes and links...");
        this.logger.log("New Nodes:", newNodes);
        this.logger.log("New Links:", newLinks);

        // Add the name
        this.currentNodes = newNodes.map(n => ({
            ...n,
            componentName: this.componentName
        }));

        this._updateGraphData(newNodes, newLinks);
    }

    invokeBlazorClick(personId) {
        this.parentNodeId = personId;
        if (!this.draggingOn && !this.editMode) {
            this._invokeBlazorClickHandler(personId);
        } else {
            this.logger.log("Turn dragging off to click");
        }
    }

    insertComponents(componentName) {
        this.componentName = componentName;
        this._insertBlazorComponents();
    }

    // Private Methods
    _initializeGraph() {
        const container = d3.select("#tree-container");
        this.svgWidth = container.node().getBoundingClientRect().width || window.innerWidth; // Set global parameter
        this.svgHeight = container.node().getBoundingClientRect().height || window.innerHeight; // Set global parameter
        const levelHeight = this.svgHeight / 5;

        const multiplier = 3;

        container.selectAll("*").remove(); // Clear the container to avoid duplicates

        const svg = container.append("svg")
            .attr("width", "100%")
            .attr("height", "100%")
            .attr("viewBox", `0 0 ${this.svgWidth} ${this.svgHeight}`);

        const g = svg.append("g");

        this.linkGroup = g.append("g").attr("class", "links");
        this.nodeGroup = g.append("g").attr("class", "nodes");

        const zoom = d3.zoom()
            .scaleExtent([0.1, 10])
            .on("zoom", (event) => {
                g.attr("transform", event.transform); // Apply zoom and pan to the 'g' element
            });

        svg.call(zoom);

        if (this.currentNodes.length === 0 && this.currentLinks.length === 0) {
            // ADD PLACEHOLDER
            this.currentNodes.push(this.placeholderNode);
            this.placeholder = true;
        }

        this.logger.log("Current nodes", this.currentNodes);
        this.simulation = d3.forceSimulation(this.currentNodes) // Assign to the global variable
            .force("link", d3.forceLink(this.currentLinks).id(d => d.id)
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
            .force("collide", d3.forceCollide().radius(this.containerWidth / 2 + 20).strength(1))
            .force("y", d3.forceY(d => (d.treeDepth + 1) * levelHeight).strength(3))
            .force("x", d3.forceX(this.svgWidth / 2).strength(0.1))
            .on("tick", () => this._ticked());
    }

    _updateGraphData(newNodes, newLinks) {
        if (!this.simulation) {
            this.logger.log("Simulation is not initialized. Make sure renderD3Graph is called first.");
            return;
        }

        const State = {
            TREE: "Tree",
            NOTREE: "NoTree",
            KEEPPLACEHOLDER: "KeepPlaceholder"
        }

        const nodesToRemove = this.currentNodes.filter(n => !newNodes.find(nn => nn.id === n.id));
        nodesToRemove.forEach(node => {
            const container = document.getElementById(`${node.componentName}-container-${node.id}`);
            if (container && container.querySelector('.blazor-component')) {
                Blazor.rootComponents.remove(container)
                    .then(() => this.logger.log(`Component removed for ${node.name}`))
                    .catch(err => this.logger.error(`Error removing component for ${node.name}: ${err}`));
            }
        });

        this.currentNodes = this.currentNodes.filter(n => !nodesToRemove.includes(n));
        const linksToRemove = this.currentLinks.filter(l => !newLinks.find(nl => nl.source === l.source && nl.target === l.target));
        this.currentLinks = this.currentLinks.filter(l => !linksToRemove.includes(l));

        let setState;
        if (this.placeholder === true && this.currentNodes.length === 0) {
            setState = State.KEEPPLACEHOLDER;
        } else if (this.placeholder === true && this.currentNodes.length > 0) {
            setState = State.TREE;
            this.currentNodes.filter(n => this.currentNodes.find(cn => cn.componentName === this.placeholderName));
            this.placeholder === false;
        } else if (this.placeholder === false && this.currentNodes.length === 0) {
            setState = State.NOTREE;
        } else {
            setState = State.TREE;
        }

        switch (setState) {
            case State.KEEPPLACEHOLDER:
                this.logger.log("Placeholder preserved")
                break;
            case State.NOTREE:
                this.placeholderNode = {
                    x: this.svgWidth / 2,
                    y: this.svgHeight / 2,
                    id: 0,
                    componentName: this.placeholderName
                };

                this.logger.log(`PlaceholderNode set: ${JSON.stringify(this.placeholderNode)}`);

                this.currentNodes = [this.placeholderNode];
                this.placeholder = true;
                this._updateSimulation();
                this._insertBlazorComponents();
                break;
            case State.TREE:
                this.placeholder = false;
                const parentNode = this.currentNodes.find(n => n.id === this.parentNodeId);
                newNodes.forEach(node => {
                    if (!this.currentNodes.find(n => n.id === node.id)) {
                        if (parentNode) {
                            node.x = parentNode.x;
                            node.y = parentNode.y;
                        }
                        this.currentNodes.push(node);
                    }
                });

                newLinks.forEach(link => {
                    if (this.currentNodes.find(n => n.id === link.source)
                        && this.currentNodes.find(n => n.id === link.target)
                        && !this.currentLinks.find(l => l.source === link.source && l.target === link.target)) {
                        this.currentLinks.push(link);
                    } else {
                        this.logger.error(`Node not found: ${!this.currentNodes.find(n => n.id === link.source) ? link.source : link.target}`);
                    }
                });

                this._updateSimulation();
                break;
            default:
                this.logger.error("No such scenario");
                break;
        }

        this.logger.log("Current Nodes after update:", this.currentNodes);
        this.logger.log("Current Links after update:", this.currentLinks);
    }

    _updateSimulation() {
        this.simulation.nodes(this.currentNodes);
        this.simulation.force("link").links(this.currentLinks);
        this.simulation.alpha(1).restart();
    }

    _ticked() {
        const link = this.linkGroup.selectAll('line') // Use linkGroup for links
            .data(this.currentLinks)
            .join('line')
            .attr('stroke', '#acacac')
            .attr("stroke-width", 5);

        const node = this.nodeGroup.selectAll('foreignObject') // Use nodeGroup for nodes
            .data(this.currentNodes, d => d.id) // Use a key function to bind data
            .join(
                enter => enter.append('foreignObject')
                    .attr('width', this.containerWidth)
                    .attr('height', 120)
                    .attr('x', d => d.x - this.containerWidth / 2)
                    .attr('y', d => d.y - 60)
                    .html(d => `<div id="${d.componentName}-container-${d.id}" class="blazor-component" onclick="invokeBlazorClick('${d.id}')"></div>`)
                    .call(d3.drag()
                        .filter(() => this.draggingOn)
                        .on("start", (event, d) => this._dragstarted(event, d))
                        .on("drag", (event, d) => this._dragged(event, d))
                        .on("end", (event, d) => this._dragended(event, d))), // Conditionally apply drag behavior
                update => update
                    .attr('x', d => d.x - this.containerWidth / 2)
                    .attr('y', d => d.y - 60),
                exit => exit.remove() // Remove old nodes
            );

        link.attr('x1', d => d.source.x)
            .attr('y1', d => d.source.y)
            .attr('x2', d => d.target.x)
            .attr('y2', d => d.target.y);
    }

    _dragstarted(event, d) {
        if (!event.active) this.simulation.alphaTarget(0.3).restart();
        d.fx = d.x;
        d.fy = d.y;
    }

    _dragged(event, d) {
        d.fx = event.x;
        d.fy = event.y;
    }

    _dragended(event, d) {
        if (!event.active) this.simulation.alphaTarget(0);
        d.fx = null;
        d.fy = null;
    }

    _invokeBlazorClickHandler(personId) {
        if (this.placeholder === true) return;
        this.logger.log(`Clicked on person with id: ${personId}`);
        this.blazorClickHandlerReference.invokeMethodAsync('BlazorClickHandler', personId)
            .then(() => this.logger.log("BlazorClickHandler invoked successfully"))
            .catch(err => this.logger.error("Error invoking BlazorClickHandler:", err));
    }

    _insertBlazorComponents() {
        this.logger.log(this.currentNodes);

        this.currentNodes.forEach(node => {
            let container = document.getElementById(`${node.componentName}-container-${node.id}`);
            if (!container) {
                this.logger.log(`No container with id ${node.componentName}-container-${node.id}`);
            } else if (container && !container.childElementCount) {
                if (node.componentName === this.componentName) {
                    const parameters = { Person: node.person };
                    Blazor.rootComponents.add(container, node.componentName, parameters)
                        .then(() => this.logger.log(`Component inserted for ${node.name}`))
                        .catch(err => this.logger.error(`Error inserting component for ${node.name}: ${err}`));
                } else {
                    Blazor.rootComponents.add(container, node.componentName, {})
                        .then(() => this.logger.log(`Component inserted for ${node.placeholderName}`))
                        .catch(err => this.logger.error(`Error inserting component for ${node.placeholderName}: ${err}`));
                }
            }
        });
    }
}

let consoleLogger = new ConsoleLogger();
//consoleLogger.turnOff();
let d3GraphManager = new D3GraphManager(consoleLogger, "person-card", "init-card");

function registerBlazorClickHandler(blazorClickHandler) {
    d3GraphManager.registerBlazorClickHandler(blazorClickHandler);
}

function setDraggingOn(value) {
    d3GraphManager.setDraggingOn(value);
}

function setEditMode(value) {
    d3GraphManager.setEditMode(value);
}

async function renderD3Graph(nodes, links, containerWidth) {
    await d3GraphManager.renderD3Graph(nodes, links, containerWidth);
}

function updateGraph(newNodes, newLinks) {
    d3GraphManager.updateGraph(newNodes, newLinks);
}

function invokeBlazorClick(personId) {
    d3GraphManager.invokeBlazorClick(personId);
}

function insertComponents(componentName) {
    d3GraphManager.insertComponents(componentName);
}

