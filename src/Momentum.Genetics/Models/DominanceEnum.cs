namespace Momentum.Genetics.Models
{
    public enum DominanceEnum
    {
        /// <summary>
        /// Dominant over all lower ranked alleles.
        /// </summary>
        Dominant = 0,

        /// <summary>
        /// Both alleles may be expressed.
        /// A red flower + white flower = red with white spots.
        /// </summary>
        Codominant = 1,

        /// <summary>
        /// Both alleles are combined to form a blended expression.
        /// A red flow + white flow = pink flower.
        /// </summary>
        Incomplete = 2,

        /// <summary>
        /// Recessive to all other alleles.
        /// </summary>
        Recessive = 3
    } // end enum
} // end namespace