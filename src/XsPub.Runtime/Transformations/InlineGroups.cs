using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using XsPub.Library.Xml;
using XsPub.Library.Xml.Schema;

namespace XsPub.Runtime.Transformations;

internal class InlineGroups : ITransformation
{

    /// <summary>
    /// Before any modifications are made, each transformation is given the opportunity
    /// to collect data about the input file.
    /// </summary>
    public void GatherData(XsSchema schema)
    {
        
    }

    /// <summary>
    /// Performs a transformation which 
    /// </summary>
    /// <remarks>
    /// Transformations that do not need to act upon the output of other
    /// transformations can be performed more efficiently if performed first.  All
    /// Independent Transformations are applied before Dependent Transformations.
    /// </remarks>
    public void IndependentTransform(XsSchema schema)
    {
        
    }

    public bool DependentTransform(XsSchema schema)
    {
        bool transformPerformed = false;
        var groupRefs = schema.Descendents.OfType<XsGroupRef>().ToList();
        foreach (var groupRef in groupRefs)
        {
            var group = schema.Groups[groupRef.RefName];
            if (group == null)
                throw new TransformationException(string.Format("Unable to inline group {0}, because the referenced group could not be found.", groupRef.RefName));

            // Assumptions: Parent is choice or sequence, and as such can host 
            // a group, choice or sequence.
            var sequence = group.Particle as XsSequence;
            // For sequences we can safely remove the extra sequence wrapper and 
            // place the items directly into the choice or sequence.
            if (sequence != null)
                groupRef.ReplaceWith(sequence.Items);
            else
                groupRef.ReplaceWith(group.Particle);
            transformPerformed = true;
        }
        return transformPerformed;
    }
}
