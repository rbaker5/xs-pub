using System;
using XsPub.Library.Xml.Schema;

namespace XsPub.Runtime.Transformations;

class AllToSequence : ITransformation
{
    public void GatherData(XsSchema schema)
    {
        
    }

    public void IndependentTransform(XsSchema schema)
    {
    }

    public bool DependentTransform(XsSchema schema)
    {
        bool transformPerfored = false;
        foreach (var all in schema.Descendents.OfType<XsAll>().ToList())
        {
            all.ReplaceWith(new XsSequence(all.Items.ToList()));
            transformPerfored = true;
        }
        return transformPerfored;
    }
}
