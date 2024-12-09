using System.Collections.Generic;

namespace DockerComposeBuilder.Model.Infrastructure;

public class ValueCollection<T>(
    IEnumerable<T> collection
) : List<T>(collection), IValueCollection<T>
    where T : class;
