namespace Macabre2D.Engine.Windows.Services {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.ServiceInterfaces;

    public sealed class StatusService : NotifyPropertyChanged, IStatusService {
        private float _primaryGridSize;
        private float _secondaryGridSize;
        private float _viewHeight;
        private float _viewWidth;

        public float PrimaryGridSize {
            get {
                return this._primaryGridSize;
            }

            set {
                this.Set(ref this._primaryGridSize, value);
            }
        }

        public float SecondaryGridSize {
            get {
                return this._secondaryGridSize;
            }

            set {
                this.Set(ref this._secondaryGridSize, value);
            }
        }

        public float ViewHeight {
            get {
                return this._viewHeight;
            }

            set {
                this.Set(ref this._viewHeight, value);
            }
        }

        public float ViewWidth {
            get {
                return this._viewWidth;
            }

            set {
                this.Set(ref this._viewWidth, value);
            }
        }
    }
}