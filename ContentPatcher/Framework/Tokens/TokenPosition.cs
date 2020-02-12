using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ContentPatcher.Framework.Tokens
{
    /// <summary>A tile position that can hold tokens for its values.</summary>
    internal class TokenPosition : IContextual
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The underlying contextual values.</summary>
        private readonly AggregateContextual Contextuals = new AggregateContextual();


        /*********
        ** Accessors
        *********/
        /// <summary>The X coordinate value.</summary>
        public ITokenString X { get; }

        /// <summary>The Y coordinate value.</summary>
        public ITokenString Y { get; }

        /// <summary>Whether the instance may change depending on the context.</summary>
        public bool IsMutable => this.Contextuals.IsMutable;

        /// <summary>Whether the instance is valid for the current context.</summary>
        public bool IsReady => this.Contextuals.IsReady;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="x">The X coordinate value.</param>
        /// <param name="y">The Y coordinate value.</param>
        public TokenPosition(IManagedTokenString x, IManagedTokenString y)
        {
            this.X = x ?? throw new ArgumentNullException(nameof(x));
            this.Y = y ?? throw new ArgumentNullException(nameof(y));

            this.Contextuals
                .Add(x)
                .Add(y);
        }

        /// <summary>Try to create a <see cref="xTile.Dimensions.Location"/> value.</summary>
        /// <param name="position">The parsed value, if valid.</param>
        /// <param name="error">An error phrase indicating why the value can't be constructed.</param>
        /// <returns>Returns whether the value was successfully created.</returns>
        public bool TryGetLocation(out xTile.Dimensions.Location position, out string error)
        {
            if (
                !this.TryGetNumber(this.X, nameof(this.X), out int x, out error)
                || !this.TryGetNumber(this.Y, nameof(this.Y), out int y, out error)
            )
            {
                position = xTile.Dimensions.Location.Origin;
                return false;
            }

            position = new xTile.Dimensions.Location(x, y);
            return true;
        }

        /// <summary>Update the instance when the context changes.</summary>
        /// <param name="context">Provides access to contextual tokens.</param>
        /// <returns>Returns whether the instance changed.</returns>
        public bool UpdateContext(IContext context)
        {
            return this.Contextuals.UpdateContext(context);
        }

        /// <summary>Get the token names used by this patch in its fields.</summary>
        public IEnumerable<string> GetTokensUsed()
        {
            return this.Contextuals.GetTokensUsed();
        }

        /// <summary>Get diagnostic info about the contextual instance.</summary>
        public IContextualState GetDiagnosticState()
        {
            return this.Contextuals.GetDiagnosticState();
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Parse a number field.</summary>
        /// <param name="raw">The raw field to parse.</param>
        /// <param name="name">The field name for errors.</param>
        /// <param name="parsed">The parsed number, if the input was valid.</param>
        /// <param name="error">An error phrase indicating why the value can't be parsed, if applicable.</param>
        protected bool TryGetNumber(ITokenString raw, string name, out int parsed, out string error)
        {
            if (!raw.IsReady)
            {
                parsed = -1;
                error = $"{name} is not ready";
                return false;
            }

            if (!int.TryParse(raw.Value, out parsed))
            {
                error = $"{name} value '{raw.Value}' can't be parsed as an integer";
                return false;
            }

            error = null;
            return true;
        }
    }
}
