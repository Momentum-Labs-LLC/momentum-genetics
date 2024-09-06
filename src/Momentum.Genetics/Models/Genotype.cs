using System.Text;
using Momentum.Genetics.Interfaces;

namespace Momentum.Genetics.Models
{
    /// <summary>
    /// A pair of alleles located at a specific locus.
    /// </summary>
    public class Genotype : IGenotype
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the locus identifier.
        /// </summary>
        public Guid LocusId { get; set; }

        /// <summary>
        /// Gets or sets the more dominant allele.
        /// </summary>
        public Allele DominantAllele { get; set; }

        /// <summary>
        /// Gets or sets the less dominant allele. Can be the same as the first allele, different, or unknown.
        /// </summary>
        public Allele? OtherAllele { get; set; }

        public bool? IsHomozygous 
        { 
            get 
            { 
                bool? result = null;

                if(DominantAllele != null && OtherAllele != null)
                {
                    result = DominantAllele.Ordinal == OtherAllele.Ordinal;
                } // end if

                return result;
            } // end get
        }

        public Genotype() { } // end method

        public Genotype(Allele firstAllele, Allele? secondAllele = null)
        {
            if(firstAllele == null)
            {
                throw new ArgumentNullException(nameof(firstAllele));
            } // end if

            if(secondAllele == null || secondAllele.Ordinal >= firstAllele.Ordinal)
            {
                DominantAllele = firstAllele;
                OtherAllele = secondAllele;
            }
            else
            {
                DominantAllele = secondAllele;
                OtherAllele = firstAllele;
            } // end if
        } // end method

        public override string ToString()
        {
            var builder = new StringBuilder();
            if(DominantAllele != null)
            {
                builder.Append(DominantAllele.ToString());
            }
            else
            {
                builder.Append("_");
            } // end if

            builder.Append("/");

            if(OtherAllele != null)
            {
                builder.Append(OtherAllele.ToString());
            }
            else
            {
                builder.Append("_");
            } // end if

            return builder.ToString();
        } // end method
    } // end class
} // end namespace