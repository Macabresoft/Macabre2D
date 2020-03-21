namespace Macabre2D.Engine.Windows.Models {

    using Macabre2D.Framework;
    using System;

    public sealed class ObservableVersion : NotifyPropertyChanged {
        private int _build;
        private int _major;
        private int _minor;
        private int _revision;

        public ObservableVersion(int major, int minor) : this(major, minor, 0) {
        }

        public ObservableVersion(int major, int minor, int build) : this(major, minor, build, 0) {
        }

        public ObservableVersion(int major, int minor, int build, int revision) {
            this._major = major;
            this._minor = minor;
            this._build = build;
            this._revision = revision;
        }

        public ObservableVersion(string versionString) {
            var version = new Version(versionString);
            this.SetFromVersion(version);
        }

        public ObservableVersion(Version version) {
            this.SetFromVersion(version);
        }

        public int Build {
            get {
                return this._build;
            }

            set {
                this.Set(ref this._build, value);
            }
        }

        public int Major {
            get {
                return this._major;
            }

            set {
                this.Set(ref this._major, value);
            }
        }

        public int Minor {
            get {
                return this._minor;
            }

            set {
                this.Set(ref this._minor, value);
            }
        }

        public int Revision {
            get {
                return this._revision;
            }

            set {
                this.Set(ref this._revision, value);
            }
        }

        private void SetFromVersion(Version version) {
            this.Build = version.Build;
            this.Major = version.Major;
            this.Minor = version.Minor;
            this.Revision = version.Revision;
        }
    }
}