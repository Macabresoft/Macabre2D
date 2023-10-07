namespace Macabresoft.Macabre2D.Tests.Framework;

using System;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;

public class TestableBoundable : Entity, IBoundable {
    public event EventHandler BoundingAreaChanged;
    public BoundingArea BoundingArea { get; set; }

    public void InvokeBoundingAreaChanged() {
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}