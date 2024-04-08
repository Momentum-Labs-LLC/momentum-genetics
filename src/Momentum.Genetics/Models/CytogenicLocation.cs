using System.Reflection.Metadata;

namespace Momentum.Genetics.Models
{
    public class CytogenicLocation
    {
        private const string Q_ARM = "q"; // long arm
        private const string P_ARM = "p"; // short arm
        private const string NEAR_TERMINAL = "ter";
        private const string NEAR_CENTROMERE = "cen";
        private const string RANGE_CHARACTER = "-";

        /// <summary>
        /// Gets or sets the chromosome.
        /// </summary>
        public string Chromosome { get; set; }

        /// <summary>
        /// Gets or sets a numerical value indicating distance from the centromere.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets if the position is actually a range of bands.
        /// </summary>
        public bool IsRange { get; set; }

        /// <summary>
        /// Gets or sets if the position is on the short (p arm), or long (q arm) section of the chromosome.
        /// </summary>
        public bool OnShortArm { get; set; }

        public CytogenicLocation() {} // end method

        /// <summary>
        /// Create a cytogenic location from a string representing cytogenic location.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CytogenicLocation(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            } // end if

            // if there is XXpXX the locus is on the short arm of the chromosome.
            this.OnShortArm = value.Contains(P_ARM, StringComparison.OrdinalIgnoreCase);

            var pieces = value.Split(new string[] { Q_ARM, P_ARM, RANGE_CHARACTER }, StringSplitOptions.RemoveEmptyEntries);

            this.IsRange = pieces.Count() > 2;
            this.Chromosome = pieces.First();
            
            if(this.IsRange)
            {
                this.Position = string.Join(RANGE_CHARACTER, pieces.Skip(1));
            }
            else
            {
                this.Position = pieces.Last();
            } // end if
        } // end method

        /// <summary>
        /// Write a cytogenic location string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var armCharacter = Q_ARM;
            if(this.OnShortArm)
            {
                armCharacter = P_ARM;
            } // end if

            var position = Position;
            if(this.IsRange)
            {
                position = position.Replace(RANGE_CHARACTER, $"{RANGE_CHARACTER}{armCharacter}");
            } // end if

            return $"{Chromosome}{armCharacter}{position}";
        } // end method
    } // end class
} // end namespace