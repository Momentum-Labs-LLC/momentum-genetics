using System.Text;

namespace Momentum.Genetics.Models
{
    /// <summary>
    /// A pair of alleles located at a specific locus.
    /// </summary>
    /// <typeparam name="TLocus"></typeparam>
    public class Genotype<TAllele, TLocus>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
    {
        /// <summary>
        /// Gets or sets the more dominant allele.
        /// </summary>
        public TAllele? DominantAllele { get; set; }

        /// <summary>
        /// Gets or sets the less dominant allele. Can be the same as the first allele, different, or unknown.
        /// </summary>
        public TAllele? OtherAllele { get; set; }

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

        public Genotype(TAllele firstAllele, TAllele? secondAllele = null)
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