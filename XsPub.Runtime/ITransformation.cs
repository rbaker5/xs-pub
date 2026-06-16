using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Runtime
{
    public interface ITransformation
    {
        /// <summary>
        /// Before any modifications are made, each transformation is given the opportunity
        /// to collect data about the input file.
        /// </summary>
        void GatherData(XsSchema schema);

        /// <summary>
        /// Performs a transformation which does not depend upon modifications made by other transformations.
        /// </summary>
        /// <remarks>
        /// Transformations that do not need to act upon the output of other
        /// transformations can be performed more efficiently if performed first.  All
        /// Independent Transformations are applied before Dependent Transformations.
        /// </remarks>
        void IndependentTransform(XsSchema schema);

        /// <summary>
        /// Performs a transformation which might be need to act upon the output of other transforms.
        /// </summary>
        /// <remarks>
        /// Dependent transforms are executed in round robin until all are satisfied with completeness.
        /// </remarks>
        /// <returns>True if a transform was performed</returns>
        bool DependentTransform(XsSchema schema);
    }
}
