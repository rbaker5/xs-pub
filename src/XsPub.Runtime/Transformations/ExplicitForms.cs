using System.Xml.Schema;
using XsPub.Library.Xml.Schema;

namespace XsPub.Runtime.Transformations;

class ExplicitForms : ITransformation
{
    public ExplicitForms()
    {
    }


    #region ITransformation Members

    public void GatherData(XsSchema schema)
    {
        
    }

    public void IndependentTransform(XsSchema schema)
    {
        if (schema.AttributeFormDefault == XmlSchemaForm.None)
            schema.AttributeFormDefault = XmlSchemaForm.Unqualified;

        if (schema.ElementFormDefault == XmlSchemaForm.None)
            schema.ElementFormDefault = XmlSchemaForm.Unqualified;
    }

    public bool DependentTransform(XsSchema schema)
    {
        return false;
    }

    #endregion
}
