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
                        transform: 'translate(-50%, -50%)'
                    });
                    ghost.classList.add('drag-ghost');
                    document.body.appendChild(ghost);
                    event.interaction.ghost = ghost;
                },
                move(event) {
                    const ghost = event.interaction.ghost;
                    ghost.style.left = event.pageX + 'px';
                    ghost.style.top = event.pageY + 'px';
                },
                end(event) {
                    const ghost = event.interaction.ghost;
                    // Maus-Position
                    const x = event.pageX;
                    const y = event.pageY;

                    // Canvas-Element finden (ersetze '#myCanvas' durch dein ID/Selector)
                    const elUnder = document.elementFromPoint(event.clientX, event.clientY);
                    const canvas = elUnder?.closest('#myCanvas');
                    if (canvas && interop.dotNetHelper) {
                        // relative Position im Canvas
                        const rect = canvas.getBoundingClientRect();
                        const relX = x - rect.left;
                        const relY = y - rect.top;
                        // palette-item ID oder sonstige Info
                        const itemId = event.target.id || null;

                        interop.dotNetHelper
                            .invokeMethodAsync('OnGhostDropped', itemId, relX, relY);
                    }

                    // Aufräumen
                    document.body.removeChild(ghost);
                    event.interaction.ghost = null;
                }
            },
            inertia: false,
            autoScroll: true
        });
};
