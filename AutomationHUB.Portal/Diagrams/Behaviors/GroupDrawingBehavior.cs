using System;
using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Behaviors;
using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Models.Base;
using AutomationHUB.Portal.Diagrams.Models;

namespace AutomationHUB.Portal.Diagrams.Behaviors;

public class GroupDrawingBehavior : Behavior
{
    readonly int _gridSize;

    double _x0, _y0;
    GroupModel _preview = default!;
    private SelectionBehavior? _oldBehavior;
    private bool panning;

    public GroupDrawingBehavior(Diagram diagram, SelectionBehavior? oldBehavior) : base(diagram)
    {
        _oldBehavior = oldBehavior;
        panning = Diagram.Options.AllowPanning;
        Diagram.Options.AllowPanning = false;
        _gridSize = Diagram.Options.GridSize ?? throw new ArgumentNullException(nameof(diagram.Options.GridSize));
        Diagram.PointerDown += OnPointerDown;
        Diagram.PointerMove += OnPointerMove;
    }


    // 1) Mouse down: start point (snapped) + preview group
    public async void OnPointerDown(Model? model, PointerEventArgs e)
    {
        if (e.Button != (long)MouseEventButton.Left) return;

        if (_preview == null)
        {
            // 1) Screen → World
            var point = Diagram.GetRelativeMousePoint(e.ClientX, e.ClientY);
            var worldX = (point.X - Diagram.Pan.X) / Diagram.Zoom;
            var worldY = (point.Y - Diagram.Pan.Y) / Diagram.Zoom;

            // 2) im World-Raum auf das Grid runden
            _x0 = Math.Floor(worldX / _gridSize) * _gridSize;
            _y0 = Math.Floor(worldY / _gridSize) * _gridSize;


            _preview = new StationModel([])
            {
                Title = "Preview",
                Position = new Blazor.Diagrams.Core.Geometry.Point(_x0, _y0)
            };
                       
            Diagram.Groups.Add(_preview);
            Diagram.Refresh();
        }
        else
        {
            Diagram.UnregisterBehavior<GroupDrawingBehavior>();
            if (_oldBehavior != null)
            {
                Diagram.RegisterBehavior(new SelectionBehavior(Diagram));
            }
            Diagram.Options.AllowPanning = panning;
        }
    }

    // 2) Mouse move: resize the preview – always snapped to grid
    public void OnPointerMove(Model? model, PointerEventArgs e)
    {
        if (_preview == null) return;

        var point = Diagram.GetRelativeMousePoint(e.ClientX, e.ClientY);
        var cx = Math.Round(point.X / _gridSize) * _gridSize;
        var cy = Math.Round(point.Y / _gridSize) * _gridSize;

        var x = Math.Min(_x0, cx);
        var y = Math.Min(_y0, cy);
        var w = Math.Abs(cx - _x0);
        var h = Math.Abs(cy - _y0);

        _preview.Position = new Blazor.Diagrams.Core.Geometry.Point(x, y);
        _preview.Size = new Blazor.Diagrams.Core.Geometry.Size(w, h);
        _preview.Refresh();  // repaint     
    }

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
    }
}
