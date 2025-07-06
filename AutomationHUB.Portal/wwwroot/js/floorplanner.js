// floorplanner.js

window.floorplannerInterop = {
    dotNetHelper: null,

    init: function (dotNetHelper) {
        this.dotNetHelper = dotNetHelper;
        initPaletteDrag(this);
    }
};

window.initPaletteDrag = (interop) => {
    interact('.palette-item')
        .draggable({
            listeners: {
                start(event) {
                    const ghost = event.target.cloneNode(true);
                    Object.assign(ghost.style, {
                        position: 'fixed',
                        pointerEvents: 'none',
                        opacity: '1.0',
                        zIndex: 9999,
                        transition: 'transform 0.2s ease',
                        transformOrigin: 'left center'
                        /*transform: 'translate(-50%, -50%)'*/
                    });
                    ghost.classList.add('drag-ghost');
                    document.body.appendChild(ghost);
                    event.interaction.ghost = ghost;
                },
                move(event) {
                    const ghost = event.interaction.ghost;
                    ghost.style.left = event.pageX + 'px';
                    ghost.style.top = event.pageY + 'px';

                    // Get the element under the cursor
                    const elUnder = document.elementFromPoint(event.clientX, event.clientY);
                    const isOverStation = elUnder?.closest('.station-node');

                    // Apply transform if over station
                    if (isOverStation) {
                        ghost.style.transform = 'scale(0.9)';
                    } else {
                        ghost.style.transform = 'scale(1.0)';
                    }
                },
                end(event) {
                    console.log("initPaletteDrag end Event");
                    const ghost = event.interaction.ghost;
                    const x = event.pageX;
                    const y = event.pageY;

                    const elUnder = document.elementFromPoint(event.clientX, event.clientY);
                    const canvas = elUnder?.closest('#myCanvas');         // canvas detection
                    const station = elUnder?.closest('.station-node');     // station detection

                    const zones = document.querySelectorAll('.station-node');
                    console.log("gefundene station-nodes:", zones);

                    const itemId = event.target.id || null;

                    if (interop.dotNetHelper) {
                        if (station) {
                            const rect = station.getBoundingClientRect();
                            console.log("Rufe OnDroppedInStation");                     
                            interop.dotNetHelper
                                .invokeMethodAsync('OnDroppedInStation', itemId, (station.id || null), rect.left, rect.top);
                        } else if (canvas) {
                            console.log("Rufe OnGhostDropped");
                            interop.dotNetHelper
                                .invokeMethodAsync('OnGhostDropped', itemId, event.clientX, event.clientY);
                        }
                    }

                    // Cleanup
                    if (ghost) {
                        document.body.removeChild(ghost);
                        event.interaction.ghost = null;
                    }
                }
            },
            inertia: false,
            autoScroll: true
        });
};
