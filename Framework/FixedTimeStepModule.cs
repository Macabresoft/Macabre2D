namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A module which updates at a fixed time step.
    /// </summary>
    /// <seealso cref="BaseModule"/>
    public class FixedTimeStepModule : BaseUpdateableModule, IUpdateableModule {
        private readonly bool _hasPostUpdate;
        private readonly bool _hasPreUpdate;
        private float _postTimePassed;
        private float _preTimePassed;

        [DataMember]
        private float _timeStep = 1f / 30f;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedTimeStepModule"/> class.
        /// </summary>
        public FixedTimeStepModule() {
            var currentType = this.GetType();
            var postMethodInfo = currentType.GetMethod(nameof(FixedPostUpdate));
            this._hasPostUpdate = postMethodInfo.DeclaringType == currentType;

            var preMethodInfo = currentType.GetMethod(nameof(FixedPreUpdate));
            this._hasPreUpdate = preMethodInfo.DeclaringType == currentType;

            if (!this._hasPreUpdate && !this._hasPostUpdate) {
                throw new NotImplementedException($"{this.GetType().Name} must override either a {nameof(FixedPreUpdate)} or a a {nameof(FixedPostUpdate)}. You've made a useless class otherwise.");
            }
        }

        /// <summary>
        /// Gets the time step.
        /// </summary>
        /// <value>The time step.</value>
        /// <exception cref="NotSupportedException">Time step must be greater than 0.</exception>
        public float TimeStep {
            get {
                return this._timeStep;
            }
            set {
                if (value >= 0f) {
                    this._timeStep = value;
                }
                else {
                    throw new NotSupportedException("Time step must be greater than 0.");
                }
            }
        }

        /// <summary>
        /// Updates after the normal update for a scene at fixed intervals.
        /// </summary>
        public virtual void FixedPostUpdate() {
            return;
        }

        /// <summary>
        /// Updates before the normal update for a scene at fixed intervals.
        /// </summary>
        public virtual void FixedPreUpdate() {
            return;
        }

        /// <inheritdoc/>
        public override void PostUpdate(FrameTime frameTime) {
            if (this._hasPostUpdate) {
                this._postTimePassed += (float)frameTime.SecondsPassed;

                while (this._postTimePassed >= this._timeStep) {
                    this.FixedPostUpdate();
                    this._postTimePassed -= this._timeStep;
                }
            }
        }

        /// <inheritdoc/>
        public override void PreInitialize() {
            if (this._timeStep <= 0f) {
                throw new NotSupportedException("Time step must be greater than 0.");
            }
        }

        /// <inheritdoc/>
        public override void PreUpdate(FrameTime frameTime) {
            if (this._hasPreUpdate) {
                this._preTimePassed += (float)frameTime.SecondsPassed;

                while (this._preTimePassed >= this._timeStep) {
                    this.FixedPreUpdate();
                    this._preTimePassed -= this._timeStep;
                }
            }
        }
    }
}