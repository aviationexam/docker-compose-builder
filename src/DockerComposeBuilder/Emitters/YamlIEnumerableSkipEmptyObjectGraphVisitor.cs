using System;
using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace DockerComposeBuilder.Emitters;

public sealed class YamlIEnumerableSkipEmptyObjectGraphVisitor : ChainedObjectGraphVisitor
{
    public YamlIEnumerableSkipEmptyObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
        : base(nextVisitor)
    {
    }

    public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context,
        ObjectSerializer serializer)
    {
        var retVal = false;

        if (value.Value == null)
        {
            return false;
        }

        if (value.Value is IEnumerable enumerableObject)
        {
            var enumerator = enumerableObject.GetEnumerator();
            using var _ = enumerator as IDisposable;
            // We have a collection
            if (enumerator.MoveNext()) // Returns true if the collection is not empty.
            {
                // Don't skip this item - serialize it as normal.
                retVal = base.EnterMapping(key, value, context, serializer);
            }

            // Else we have an empty collection and the initialized return value of false is correct.
        }
        else
        {
            // Not a collection, normal serialization.
            retVal = base.EnterMapping(key, value, context, serializer);
        }

        return retVal;
    }
}
