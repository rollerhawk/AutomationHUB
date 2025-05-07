using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class NATSDomainAttribute() : Attribute
{
    /// <summary>
    /// The entity topic to which the message will be published.
    /// </summary>
    public required string Domain { get; set; }
}

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class NATSEntityAttribute() : Attribute
{
}

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class NATSEntityIdAttribute() : Attribute
{
}
