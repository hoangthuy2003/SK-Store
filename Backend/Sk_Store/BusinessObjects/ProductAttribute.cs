using System;
using System.Collections.Generic;


namespace BusinessObjects;

public partial class ProductAttribute
{
    public int AttributeId { get; set; }

    public int ProductId { get; set; }

    public string AttributeName { get; set; } = null!;

    public string AttributeValue { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
